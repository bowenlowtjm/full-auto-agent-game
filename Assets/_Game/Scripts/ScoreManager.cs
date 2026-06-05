using UnityEngine;
using UnityEngine.Events;

namespace Pully.Game
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private RulesetDefinition ruleset;
        
        public int Score { get; private set; }
        public float ComboMultiplier { get; private set; } = 1f;
        public int ConsecutiveHits { get; private set; }
        public int Lives { get; private set; }
        public bool IsGameOver => Lives <= 0;
        
        public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
        public UnityEvent<float> OnComboChanged = new UnityEvent<float>();
        public UnityEvent<int> OnLivesChanged = new UnityEvent<int>();
        public UnityEvent OnGameOver = new UnityEvent();
        public UnityEvent OnHit = new UnityEvent();
        public UnityEvent OnMiss = new UnityEvent();
        
        private void Awake()
        {
            if (ruleset == null)
            {
                Debug.LogError("[ScoreManager] Ruleset not assigned!");
                enabled = false;
            }
        }
        
        public void Initialize()
        {
            Score = 0;
            ComboMultiplier = 1f;
            ConsecutiveHits = 0;
            Lives = ruleset.lives;
            OnScoreChanged?.Invoke(Score);
            OnComboChanged?.Invoke(ComboMultiplier);
            OnLivesChanged?.Invoke(Lives);
        }
        
        public void OnTargetHit(int baseReward)
        {
            ConsecutiveHits++;
            ComboMultiplier = Mathf.Min(ruleset.comboStep * ConsecutiveHits, ruleset.comboCap);
            int points = Mathf.RoundToInt(baseReward * ComboMultiplier);
            Score += points;
            OnScoreChanged?.Invoke(Score);
            OnComboChanged?.Invoke(ComboMultiplier);
            OnHit?.Invoke();
        }
        
        public void OnTargetMiss()
        {
            ConsecutiveHits = 0;
            ComboMultiplier = 1f;
            Lives--;
            OnComboChanged?.Invoke(ComboMultiplier);
            OnLivesChanged?.Invoke(Lives);
            OnMiss?.Invoke();
            if (Lives <= 0) OnGameOver?.Invoke();
        }
        
        public int GetExpectedScore(int baseReward) => Mathf.RoundToInt(baseReward * ComboMultiplier);
    }
}
