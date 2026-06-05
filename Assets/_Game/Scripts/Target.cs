using System;
using UnityEngine;

namespace Pully.Game
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D hitCollider;
        [SerializeField] private ParticleSystem hitParticles;
        [SerializeField] private ParticleSystem missParticles;
        
        public RulesetDefinition.Shape Shape { get; private set; }
        public Color TargetColor { get; private set; }
        public RulesetDefinition.Gesture RequiredGesture { get; private set; }
        public int BaseReward { get; private set; }
        public float SpawnTime { get; private set; }
        
        public event Action<Target, RulesetDefinition.Gesture> OnGestureAttempted;
        public event Action<Target> OnTargetExpired;
        
        private float lifetime;
        private bool isActive = false;
        private bool isExpired = false;
        
        public void Initialize(RulesetDefinition.Shape shape, Color color, RulesetDefinition.Gesture gesture, int baseReward, float targetLifetime)
        {
            Shape = shape;
            TargetColor = color;
            RequiredGesture = gesture;
            BaseReward = baseReward;
            lifetime = targetLifetime;
            SpawnTime = Time.time;
            isExpired = false;
            isActive = true;
            
            if (spriteRenderer != null)
                spriteRenderer.color = color;
            if (hitCollider != null)
                hitCollider.enabled = true;
            gameObject.SetActive(true);
            transform.localScale = Vector3.one;
        }
        
        private void Update()
        {
            if (!isActive || isExpired) return;
            if (Time.time - SpawnTime >= lifetime) Expire();
            
            float timeLeft = 1f - ((Time.time - SpawnTime) / lifetime);
            if (timeLeft < 0.3f && spriteRenderer != null)
            {
                float pulse = Mathf.PingPong(Time.time * 10f, 0.3f);
                spriteRenderer.transform.localScale = Vector3.one * (1f + pulse);
            }
        }
        
        public bool IsHitBy(Vector2 screenPos)
        {
            if (hitCollider == null || !isActive) return false;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            return hit.collider != null && hit.collider == hitCollider;
        }
        
        public void AttemptGesture(RulesetDefinition.Gesture gesture)
        {
            if (!isActive || isExpired) return;
            OnGestureAttempted?.Invoke(this, gesture);
            if (gesture == RequiredGesture) Hit(); else Miss();
        }
        
        private void Hit()
        {
            isActive = false;
            if (hitParticles != null) hitParticles.Play();
            gameObject.SetActive(false);
        }
        
        private void Miss()
        {
            if (missParticles != null) missParticles.Play();
        }
        
        private void Expire()
        {
            if (isExpired) return;
            isExpired = true;
            isActive = false;
            OnTargetExpired?.Invoke(this);
            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = 0f;
                spriteRenderer.color = c;
            }
            gameObject.SetActive(false);
        }
        
        public float GetTimeRemaining() => Mathf.Max(0, lifetime - (Time.time - SpawnTime));
    }
}
