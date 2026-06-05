using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Pully.Game
{
    /// <summary>
    /// One-shot bootstrap that auto-creates all game systems.
    /// Attach this to an empty GameObject in GameScene.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private RulesetDefinition ruleset;
        
        private void Awake()
        {
            // Create or find GameManager
            var gm = FindObjectOfType<GameManager>();
            if (gm == null)
            {
                var gmGO = new GameObject("GameManager");
                gm = gmGO.AddComponent<GameManager>();
            }
            
            // Ensure ruleset exists
            if (ruleset == null)
            {
                ruleset = CreateDefaultRuleset();
            }
            
            // Wire up components
            var gestureRec = gm.GetComponent<GestureRecognizer>();
            if (gestureRec == null) gestureRec = gm.gameObject.AddComponent<GestureRecognizer>();
            
            var spawner = gm.GetComponent<TargetSpawner>();
            if (spawner == null) spawner = gm.gameObject.AddComponent<TargetSpawner>();
            
            var scoreMgr = gm.GetComponent<ScoreManager>();
            if (scoreMgr == null) scoreMgr = gm.gameObject.AddComponent<ScoreManager>();
            
            // Create target container
            var container = GameObject.Find("TargetContainer");
            if (container == null)
            {
                container = new GameObject("TargetContainer");
            }
            
            // Create Canvas if needed
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                // Create HUD
                CreateHUD(canvas);
            }
            
            // Use reflection to set private fields
            SetPrivateField(gm, "ruleset", ruleset);
            SetPrivateField(gm, "gestureRecognizer", gestureRec);
            SetPrivateField(gm, "targetSpawner", spawner);
            SetPrivateField(gm, "scoreManager", scoreMgr);
            SetPrivateField(spawner, "ruleset", ruleset);
            SetPrivateField(spawner, "gameCamera", Camera.main);
            SetPrivateField(spawner, "targetContainer", container.transform);
            SetPrivateField(scoreMgr, "ruleset", ruleset);
            SetPrivateField(gestureRec, "ruleset", ruleset);
            
            // Start the game
            gm.StartGame();
            
            Debug.Log("[GameBootstrap] ✅ Game initialized and running!");
        }
        
        private void CreateHUD(Canvas canvas)
        {
            // Score
            var scoreGO = new GameObject("ScoreText");
            scoreGO.transform.SetParent(canvas.transform, false);
            var scoreRect = scoreGO.AddComponent<RectTransform>();
            scoreRect.anchoredPosition = new Vector2(0, 350);
            scoreRect.sizeDelta = new Vector2(300, 80);
            var scoreText = scoreGO.AddComponent<TMPro.TextMeshProUGUI>();
            scoreText.text = "0";
            scoreText.fontSize = 48;
            scoreText.alignment = TMPro.TextAlignmentOptions.Center;
            scoreText.color = Color.white;
            
            // Combo
            var comboGO = new GameObject("ComboText");
            comboGO.transform.SetParent(canvas.transform, false);
            var comboRect = comboGO.AddComponent<RectTransform>();
            comboRect.anchoredPosition = new Vector2(0, 280);
            comboRect.sizeDelta = new Vector2(200, 60);
            var comboText = comboGO.AddComponent<TMPro.TextMeshProUGUI>();
            comboText.text = "x1.0";
            comboText.fontSize = 36;
            comboText.alignment = TMPro.TextAlignmentOptions.Center;
            comboText.color = Color.yellow;
            
            // Lives
            var livesGO = new GameObject("LivesText");
            livesGO.transform.SetParent(canvas.transform, false);
            var livesRect = livesGO.AddComponent<RectTransform>();
            livesRect.anchoredPosition = new Vector2(-250, 350);
            livesRect.sizeDelta = new Vector2(150, 60);
            var livesText = livesGO.AddComponent<TMPro.TextMeshProUGUI>();
            livesText.text = "3/3";
            livesText.fontSize = 36;
            livesText.alignment = TMPro.TextAlignmentOptions.Center;
            livesText.color = Color.red;
            
            // Timer
            var timerGO = new GameObject("TimerText");
            timerGO.transform.SetParent(canvas.transform, false);
            var timerRect = timerGO.AddComponent<RectTransform>();
            timerRect.anchoredPosition = new Vector2(250, 350);
            timerRect.sizeDelta = new Vector2(150, 60);
            var timerText = timerGO.AddComponent<TMPro.TextMeshProUGUI>();
            timerText.text = "60";
            timerText.fontSize = 36;
            timerText.alignment = TMPro.TextAlignmentOptions.Center;
            timerText.color = Color.cyan;
        }
        
        private RulesetDefinition CreateDefaultRuleset()
        {
            Debug.Log("[GameBootstrap] Creating default ruleset...");
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            
            // Circle - Single Tap
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = new Color(0.298f, 0.788f, 0.941f), // Cyan
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 1
            });
            
            // Circle (Red) - Long Press (Trap)
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = new Color(1f, 0.267f, 0.267f), // Red
                requiredGesture = RulesetDefinition.Gesture.LongPress,
                baseReward = 5
            });
            
            // Square - Double Tap
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Square,
                color = new Color(0.263f, 0.38f, 0.933f), // Blue
                requiredGesture = RulesetDefinition.Gesture.DoubleTap,
                baseReward = 3
            });
            
            // Triangle - Swipe
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Triangle,
                color = new Color(0.969f, 0.145f, 0.522f), // Pink
                requiredGesture = RulesetDefinition.Gesture.SwipeTap,
                baseReward = 5
            });
            
            // Star - Two Finger
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Star,
                color = new Color(0.447f, 0.035f, 0.718f), // Purple
                requiredGesture = RulesetDefinition.Gesture.TwoFingerTap,
                baseReward = 8
            });
            
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;
            ruleset.lives = 3;
            ruleset.roundSeconds = 60f;
            ruleset.targetLifetime = 1.6f;
            ruleset.spawnIntervalStart = 1.2f;
            ruleset.spawnIntervalEnd = 0.6f;
            ruleset.maxConcurrentTargets = 4;
            ruleset.doubleTapWindow = 0.3f;
            ruleset.longPressDuration = 0.5f;
            ruleset.swipeMinDistance = 50f;
            ruleset.seed = 12345;
            
            return ruleset;
        }
        
        private void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(target, value);
        }
    }
}
