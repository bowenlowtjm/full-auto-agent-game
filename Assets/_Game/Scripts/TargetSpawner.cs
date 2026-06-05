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
        [SerializeField] private Transform targetContainer;
        
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
            
            switch (shape)
            {
                case RulesetDefinition.Shape.Circle:
                    sr.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
                    go.AddComponent<CircleCollider2D>().radius = 0.5f;
                    break;
                default:
                    sr.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
                    go.AddComponent<BoxCollider2D>();
                    break;
            }
            
            sr.color = Color.white;
            go.AddComponent<Target>();
            return go;
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
            return hit.collider?.GetComponent<Target>();
        }
    }
}
