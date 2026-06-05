using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private RulesetDefinition ruleset;
        [SerializeField] private GestureRecognizer gestureRecognizer;
        [SerializeField] private TargetSpawner targetSpawner;
        [SerializeField] private ScoreManager scoreManager;
        
        [SerializeField] private TMPro.TextMeshProUGUI scoreText;
        [SerializeField] private TMPro.TextMeshProUGUI comboText;
        [SerializeField] private TMPro.TextMeshProUGUI livesText;
        [SerializeField] private TMPro.TextMeshProUGUI timerText;
        [SerializeField] private GameObject gameOverPanel;
        
        public GameState CurrentState { get; private set; }
        public float RoundTimer { get; private set; }
        
        public enum GameState { Menu, Playing, Paused, GameOver }
        
        public event Action<GameState> OnStateChanged;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            ValidateReferences();
        }
        
        private void ValidateReferences()
        {
            if (ruleset == null) Debug.LogError("[GameManager] Ruleset not assigned!");
            if (gestureRecognizer == null) Debug.LogError("[GameManager] GestureRecognizer not assigned!");
            if (targetSpawner == null) Debug.LogError("[GameManager] TargetSpawner not assigned!");
            if (scoreManager == null) Debug.LogError("[GameManager] ScoreManager not assigned!");
        }
        
        private void Start()
        {
            SubscribeToEvents();
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            TransitionToState(GameState.Playing);
            StartGame();
        }
        
        private void OnDestroy() => UnsubscribeFromEvents();
        
        private void Update()
        {
            if (CurrentState == GameState.Playing)
                UpdateTimer();
        }
        
        private void SubscribeToEvents()
        {
            if (gestureRecognizer != null)
            {
                gestureRecognizer.OnSingleTap += (p) => ProcessGesture(RulesetDefinition.Gesture.SingleTap, p);
                gestureRecognizer.OnDoubleTap += (p) => ProcessGesture(RulesetDefinition.Gesture.DoubleTap, p);
                gestureRecognizer.OnLongPress += (p) => ProcessGesture(RulesetDefinition.Gesture.LongPress, p);
                gestureRecognizer.OnSwipeTap += (p, d) => ProcessGesture(RulesetDefinition.Gesture.SwipeTap, p);
                gestureRecognizer.OnTwoFingerTap += (p) => ProcessGesture(RulesetDefinition.Gesture.TwoFingerTap, p);
            }
            if (targetSpawner != null)
            {
                targetSpawner.OnTargetSpawned += t => t.OnTargetExpired += HandleTargetExpired;
                targetSpawner.OnTargetDestroyed += HandleTargetDestroyed;
            }
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged.AddListener(OnScoreChanged);
                scoreManager.OnComboChanged.AddListener(OnComboChanged);
                scoreManager.OnLivesChanged.AddListener(OnLivesChanged);
                scoreManager.OnGameOver.AddListener(OnGameOver);
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            if (gestureRecognizer != null)
            {
                gestureRecognizer.OnSingleTap -= (p) => ProcessGesture(RulesetDefinition.Gesture.SingleTap, p);
                gestureRecognizer.OnDoubleTap -= (p) => ProcessGesture(RulesetDefinition.Gesture.DoubleTap, p);
                gestureRecognizer.OnLongPress -= (p) => ProcessGesture(RulesetDefinition.Gesture.LongPress, p);
                gestureRecognizer.OnSwipeTap -= (p, d) => ProcessGesture(RulesetDefinition.Gesture.SwipeTap, p);
                gestureRecognizer.OnTwoFingerTap -= (p) => ProcessGesture(RulesetDefinition.Gesture.TwoFingerTap, p);
            }
        }
        
        public void StartGame()
        {
            RoundTimer = ruleset.roundSeconds;
            scoreManager.Initialize();
            targetSpawner.StartRound();
            TransitionToState(GameState.Playing);
        }
        
        public void EndGame()
        {
            targetSpawner.StopRound();
            PlayerPrefs.SetInt("Pully_CurrentScore", scoreManager.Score);
            TransitionToState(GameState.GameOver);
        }
        
        private void UpdateTimer()
        {
            RoundTimer -= Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(RoundTimer).ToString();
            if (RoundTimer <= 0 || scoreManager.IsGameOver)
                EndGame();
        }
        
        private void ProcessGesture(RulesetDefinition.Gesture gesture, Vector2 screenPos)
        {
            if (CurrentState != GameState.Playing) return;
            Debug.Log($"[GameManager] Processing {gesture} at {screenPos}");
            Target target = targetSpawner.FindTargetAt(screenPos);
            if (target != null)
            {
                Debug.Log($"[GameManager] Hit target: {target.Shape} requiring {target.RequiredGesture}");
                target.AttemptGesture(gesture);
            }
            else
            {
                Debug.Log($"[GameManager] Miss - no target at {screenPos}");
            }
        }
        
        private void HandleTargetExpired(Target target) => scoreManager.OnTargetMiss();
        
        private void HandleTargetDestroyed(Target target)
        {
            if (target.GetTimeRemaining() <= 0)
                scoreManager.OnTargetMiss();
        }
        
        private void OnScoreChanged(int score)
        {
            if (scoreText != null) scoreText.text = score.ToString();
        }
        
        private void OnComboChanged(float combo)
        {
            if (comboText != null) comboText.text = $"x{combo:F1}";
        }
        
        private void OnLivesChanged(int lives)
        {
            if (livesText != null) livesText.text = $"{lives}/{ruleset.lives}";
        }
        
        private void OnGameOver()
        {
            EndGame();
            SceneManager.LoadScene("GameOverScene");
        }
        
        private void TransitionToState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
            if (gameOverPanel != null) gameOverPanel.SetActive(newState == GameState.GameOver);
        }
    }
}
