using System.Collections.Generic;
using UnityEngine;

namespace Pully.Game
{
    public class TargetSpawner : MonoBehaviour
    {
        [SerializeField] private RulesetDefinition ruleset;
        [SerializeField] private Camera gameCamera;
        
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private GameObject squarePrefab;
        [SerializeField] private GameObject trianglePrefab;
        [SerializeField] private GameObject starPrefab;
        public Transform targetContainer;
        
        [SerializeField] private float padding = 1f;
        
        private List<Target> activeTargets = new();
        private System.Random rng;
        private float spawnTimer = 0f;
        private float currentSpawnInterval;
        private float roundStartTime;
        
        public event System.Action<Target> OnTargetSpawned;
        public event System.Action<Target> OnTargetDestroyed;
        
        public IReadOnlyList<Target> ActiveTargets => activeTargets;
        
        private void Awake()
        {
            // Don't disable here - let Bootstrap set fields first
            // Validation happens in Start()
        }
        
        private void Start()
        {
            if (ruleset == null)
            {
                Debug.LogError("[TargetSpawner] Ruleset not assigned!");
                enabled = false;
                return;
            }
            if (gameCamera == null) gameCamera = Camera.main;
            rng = new System.Random(ruleset.seed);
            currentSpawnInterval = ruleset.spawnIntervalStart;
        }
        
        public void StartRound()
        {
            roundStartTime = Time.time;
            spawnTimer = 0f;
            activeTargets.Clear();
            rng = new System.Random(ruleset.seed);
        }
        
        public void StopRound()
        {
            foreach (var target in activeTargets)
                if (target != null) Destroy(target.gameObject);
            activeTargets.Clear();
        }
        
        private void Update()
        {
            float roundProgress = (Time.time - roundStartTime) / ruleset.roundSeconds;
            currentSpawnInterval = Mathf.Lerp(ruleset.spawnIntervalStart, ruleset.spawnIntervalEnd, roundProgress);
            spawnTimer += Time.deltaTime;
            
            if (spawnTimer >= currentSpawnInterval && activeTargets.Count < ruleset.maxConcurrentTargets)
            {
                spawnTimer = 0f;
                SpawnTarget();
            }
            activeTargets.RemoveAll(t => t == null || !t.gameObject.activeInHierarchy);
        }
        
        private void SpawnTarget()
        {
            int ruleIndex = rng.Next(0, ruleset.rules.Count);
            var rule = ruleset.rules[ruleIndex];
            GameObject prefab = GetPrefabForShape(rule.shape);
            
            Vector2 spawnPos = GetRandomSpawnPosition();
            GameObject targetObj = Instantiate(prefab, spawnPos, Quaternion.identity, targetContainer);
            Target target = targetObj.GetComponent<Target>();
            if (target == null) target = targetObj.AddComponent<Target>();
            
            target.Initialize(rule.shape, rule.color, rule.requiredGesture, rule.baseReward, ruleset.targetLifetime);
            target.OnGestureAttempted += HandleGestureAttempted;
            target.OnTargetExpired += HandleTargetExpired;
            activeTargets.Add(target);
            OnTargetSpawned?.Invoke(target);
        }
        
        private GameObject GetPrefabForShape(RulesetDefinition.Shape shape)
        {
            GameObject prefab = shape switch
            {
                RulesetDefinition.Shape.Circle => circlePrefab,
                RulesetDefinition.Shape.Square => squarePrefab,
                RulesetDefinition.Shape.Triangle => trianglePrefab,
                RulesetDefinition.Shape.Star => starPrefab,
                _ => null
            };
            
            // Fallback: create primitive if no prefab assigned
            if (prefab == null)
            {
                prefab = CreatePrimitiveTarget(shape);
            }
            
            return prefab;
        }
        
        private GameObject CreatePrimitiveTarget(RulesetDefinition.Shape shape)
        {
            var go = new GameObject($"{shape}Target");
            var sr = go.AddComponent<SpriteRenderer>();

            // Create simple colored shapes using procedural textures
            switch (shape)
            {
                case RulesetDefinition.Shape.Circle:
                    sr.sprite = CreateCircleSprite();
                    go.AddComponent<CircleCollider2D>().radius = 0.5f;
                    break;
                case RulesetDefinition.Shape.Square:
                    sr.sprite = CreateSquareSprite();
                    go.AddComponent<BoxCollider2D>();
                    break;
                case RulesetDefinition.Shape.Triangle:
                    sr.sprite = CreateTriangleSprite();
                    var poly = go.AddComponent<PolygonCollider2D>();
                    poly.points = new Vector2[] { new(0, 0.5f), new(-0.4f, -0.3f), new(0.4f, -0.3f) };
                    break;
                case RulesetDefinition.Shape.Star:
                    sr.sprite = CreateStarSprite();
                    go.AddComponent<CircleCollider2D>().radius = 0.5f;
                    break;
                default:
                    sr.sprite = CreateSquareSprite();
                    go.AddComponent<BoxCollider2D>();
                    break;
            }

            sr.color = Color.white;
            go.AddComponent<Target>();
            return go;
        }

        private Sprite CreateCircleSprite()
        {
            Texture2D tex = new(64, 64);
            Color[] pixels = new Color[64 * 64];
            Vector2 center = new(32, 32);
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                {
                    float dist = Vector2.Distance(new(x, y), center);
                    pixels[y * 64 + x] = dist < 30 ? Color.white : Color.clear;
                }
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }

        private Sprite CreateSquareSprite()
        {
            Texture2D tex = new(64, 64);
            Color[] pixels = new Color[64 * 64];
            for (int y = 4; y < 60; y++)
                for (int x = 4; x < 60; x++)
                    pixels[y * 64 + x] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }

        private Sprite CreateTriangleSprite()
        {
            Texture2D tex = new(64, 64);
            Color[] pixels = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                {
                    float py = y / 64f;
                    float px = x / 64f;
                    // Simple triangle shape
                    if (py > 0.2f && py < 0.9f - Mathf.Abs(px - 0.5f) * 1.4f)
                        pixels[y * 64 + x] = Color.white;
                }
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }

        private Sprite CreateStarSprite()
        {
            Texture2D tex = new(64, 64);
            Color[] pixels = new Color[64 * 64];
            Vector2 center = new(32, 32);
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                {
                    Vector2 pos = new(x, y);
                    float angle = Mathf.Atan2(pos.y - center.y, pos.x - center.x) * Mathf.Rad2Deg;
                    float dist = Vector2.Distance(pos, center);
                    // 5-point star shape
                    float starRadius = 28 * (1 + 0.5f * Mathf.Sin(angle * 5 * Mathf.Deg2Rad));
                    pixels[y * 64 + x] = dist < starRadius ? Color.white : Color.clear;
                }
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }
        
        private Vector2 GetRandomSpawnPosition()
        {
            float halfHeight = gameCamera.orthographicSize;
            float halfWidth = halfHeight * gameCamera.aspect;
            float x = (float)rng.NextDouble() * (halfWidth * 2 - padding * 2) - halfWidth + padding;
            float y = (float)rng.NextDouble() * (halfHeight * 2 - padding * 2) - halfHeight + padding;
            Vector2 pos = new(x, y);
            
            int attempts = 0;
            const float minDistance = 1.5f;
            while (attempts < 10)
            {
                bool tooClose = false;
                foreach (var target in activeTargets)
                    if (target != null && Vector2.Distance(target.transform.position, pos) < minDistance)
                    { tooClose = true; break; }
                if (!tooClose) break;
                x = (float)rng.NextDouble() * (halfWidth * 2 - padding * 2) - halfWidth + padding;
                y = (float)rng.NextDouble() * (halfHeight * 2 - padding * 2) - halfHeight + padding;
                pos = new(x, y);
                attempts++;
            }
            return pos;
        }
        
        private void HandleGestureAttempted(Target target, RulesetDefinition.Gesture gesture) { }
        private void HandleTargetExpired(Target target)
        {
            activeTargets.Remove(target);
            OnTargetDestroyed?.Invoke(target);
        }
        
        public Target FindTargetAt(Vector2 screenPos)
        {
            Ray ray = gameCamera.ScreenPointToRay(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            Debug.Log($"[TargetSpawner] Raycast from {ray.origin} direction {ray.direction} - hit: {(hit.collider != null ? hit.collider.name : "null")}");
            return hit.collider?.GetComponent<Target>();
        }
    }
}
