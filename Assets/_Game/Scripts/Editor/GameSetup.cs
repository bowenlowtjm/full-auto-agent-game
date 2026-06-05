using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game.Editor
{
    /// <summary>
    /// Editor tools to bootstrap the full game setup.
    /// Run: Pully/Setup Game
    /// </summary>
    public static class GameSetup
    {
        [MenuItem("Pully/Setup Game")]
        public static void SetupFullGame()
        {
            SetupFolders();
            CreateRuleset();
            CreateTargetPrefabs();
            CreateMenuScene();
            CreateGameScene();
            CreateGameOverScene();
            SetupBuildSettings();
            
            Debug.Log("[GameSetup] ✅ Full game setup complete! Open MenuScene and hit Play.");
        }
        
        [MenuItem("Pully/Create Ruleset Only")]
        public static void CreateRuleset()
        {
            string dataPath = "Assets/_Game/Data/";
            if (!System.IO.Directory.Exists(dataPath))
                System.IO.Directory.CreateDirectory(dataPath);
            
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            
            // Circle - Green - Single Tap - Score 1
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = new Color(0.298f, 0.788f, 0.941f), // #4CC9F0
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 1
            });
            
            // Circle - Red - Long Press - Score 5 (trap)
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = new Color(1f, 0.267f, 0.267f), // #FF4444
                requiredGesture = RulesetDefinition.Gesture.LongPress,
                baseReward = 5
            });
            
            // Square - Blue - Double Tap - Score 3
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Square,
                color = new Color(0.263f, 0.38f, 0.933f), // #4361EE
                requiredGesture = RulesetDefinition.Gesture.DoubleTap,
                baseReward = 3
            });
            
            // Triangle - Yellow - Swipe - Score 5
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Triangle,
                color = new Color(0.969f, 0.145f, 0.522f), // #F72585
                requiredGesture = RulesetDefinition.Gesture.SwipeTap,
                baseReward = 5
            });
            
            // Star - Purple - Two Finger - Score 8
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Star,
                color = new Color(0.447f, 0.035f, 0.718f), // #7209B7
                requiredGesture = RulesetDefinition.Gesture.TwoFingerTap,
                baseReward = 8
            });
            
            // Balance settings
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
            
            AssetDatabase.CreateAsset(ruleset, dataPath + "DefaultRuleset.asset");
            AssetDatabase.SaveAssets();
            
            Debug.Log("[GameSetup] ✅ Created DefaultRuleset.asset");
        }
        
        private static void SetupFolders()
        {
            string[] folders = new[]
            {
                "Assets/_Game/Data",
                "Assets/_Game/Prefabs/Targets",
                "Assets/_Game/Scenes",
                "Assets/_Game/Sprites"
            };
            
            foreach (var folder in folders)
            {
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
            }
            AssetDatabase.Refresh();
        }
        
        private static void CreateTargetPrefabs()
        {
            string prefabPath = "Assets/_Game/Prefabs/Targets/";
            
            // Circle prefab
            var circle = new GameObject("CircleTarget");
            circle.AddComponent<SpriteRenderer>().sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
            circle.GetComponent<SpriteRenderer>().color = Color.white;
            var circleCol = circle.AddComponent<CircleCollider2D>();
            circleCol.radius = 0.5f;
            circle.AddComponent<Target>();
            PrefabUtility.SaveAsPrefabAsset(circle, prefabPath + "CircleTarget.prefab");
            Object.DestroyImmediate(circle);
            
            // Square prefab
            var square = new GameObject("SquareTarget");
            square.AddComponent<SpriteRenderer>().sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
            var squareCol = square.AddComponent<BoxCollider2D>();
            squareCol.size = new Vector2(1, 1);
            square.AddComponent<Target>();
            PrefabUtility.SaveAsPrefabAsset(square, prefabPath + "SquareTarget.prefab");
            Object.DestroyImmediate(square);
            
            // Triangle prefab (using circle sprite for now)
            var triangle = new GameObject("TriangleTarget");
            triangle.AddComponent<SpriteRenderer>().sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
            triangle.AddComponent<PolygonCollider2D>();
            triangle.AddComponent<Target>();
            PrefabUtility.SaveAsPrefabAsset(triangle, prefabPath + "TriangleTarget.prefab");
            Object.DestroyImmediate(triangle);
            
            // Star prefab
            var star = new GameObject("StarTarget");
            star.AddComponent<SpriteRenderer>().sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
            star.AddComponent<PolygonCollider2D>();
            star.AddComponent<Target>();
            PrefabUtility.SaveAsPrefabAsset(star, prefabPath + "StarTarget.prefab");
            Object.DestroyImmediate(star);
            
            AssetDatabase.SaveAssets();
            Debug.Log("[GameSetup] ✅ Created target prefabs");
        }
        
        private static void CreateMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Camera setup
            Camera.main.backgroundColor = new Color(0.1f, 0.1f, 0.18f);
            
            // Canvas
            var canvas = new GameObject("Canvas");
            var c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = 0;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            
            // MenuManager
            canvas.AddComponent<MenuManager>();
            
            // Main Menu Panel
            var menuPanel = new GameObject("MainMenuPanel");
            menuPanel.transform.SetParent(canvas.transform, false);
            var panelRect = menuPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            var panelImg = menuPanel.AddComponent<UnityEngine.UI.Image>();
            panelImg.color = new Color(0.1f, 0.1f, 0.18f, 1f);
            
            // Title
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(menuPanel.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 200);
            titleRect.sizeDelta = new Vector2(600, 100);
            var titleText = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleText.text = "PULLY";
            titleText.fontSize = 72;
            titleText.color = new Color(0.914f, 0.271f, 0.376f); // #E94560
            titleText.alignment = TMPro.TextAlignmentOptions.Center;
            
            // Play Button
            CreateButton(menuPanel.transform, "PlayButton", "PLAY", new Vector2(0, 50), new Color(0.263f, 0.38f, 0.933f));
            
            // Best Score
            var scoreObj = new GameObject("BestScore");
            scoreObj.transform.SetParent(menuPanel.transform, false);
            var scoreRect = scoreObj.AddComponent<RectTransform>();
            scoreRect.anchoredPosition = new Vector2(0, -50);
            scoreRect.sizeDelta = new Vector2(400, 50);
            var scoreText = scoreObj.AddComponent<TMPro.TextMeshProUGUI>();
            scoreText.text = "Best: 0";
            scoreText.fontSize = 36;
            scoreText.color = Color.white;
            scoreText.alignment = TMPro.TextAlignmentOptions.Center;
            // Mark as bestScoreText for MenuManager
            
            EditorSceneManager.SaveScene(scene, "Assets/_Game/Scenes/MenuScene.unity");
            Debug.Log("[GameSetup] ✅ Created MenuScene");
        }
        
        private static void CreateGameScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Camera
            Camera.main.backgroundColor = new Color(0.1f, 0.1f, 0.18f);
            Camera.main.orthographicSize = 5;
            
            // Create GameManager GO
            var gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
            var gestureRec = gm.AddComponent<GestureRecognizer>();
            var spawner = gm.AddComponent<TargetSpawner>();
            var scoreMgr = gm.AddComponent<ScoreManager>();
            
            // Target Container
            var targetContainer = new GameObject("TargetContainer");
            spawner.targetContainer = targetContainer.transform;
            
            // Canvas
            var canvas = new GameObject("Canvas");
            var c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            
            // HUD Panel
            var hud = new GameObject("HUD");
            hud.transform.SetParent(canvas.transform, false);
            var hudRect = hud.AddComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.sizeDelta = Vector2.zero;
            
            // Score Text
            CreateHUDText(hud.transform, "ScoreText", "ScoreText", new Vector2(0, 400), new Vector2(200, 80));
            
            // Combo Text
            CreateHUDText(hud.transform, "ComboText", "ComboText", new Vector2(0, 320), new Vector2(150, 60));
            
            // Lives Text
            CreateHUDText(hud.transform, "LivesText", "LivesText", new Vector2(-250, 400), new Vector2(150, 60));
            
            // Timer Text
            CreateHUDText(hud.transform, "TimerText", "TimerText", new Vector2(250, 400), new Vector2(150, 60));
            
            EditorSceneManager.SaveScene(scene, "Assets/_Game/Scenes/GameScene.unity");
            Debug.Log("[GameSetup] ✅ Created GameScene");
        }
        
        private static void CreateGameOverScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            Camera.main.backgroundColor = new Color(0.1f, 0.1f, 0.18f);
            
            // Canvas
            var canvas = new GameObject("Canvas");
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.AddComponent<GameOverManager>();
            
            // Game Over Panel
            var panel = new GameObject("GameOverPanel");
            panel.transform.SetParent(canvas.transform, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            // Title
            var title = new GameObject("Title");
            title.transform.SetParent(panel.transform, false);
            title.AddComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
            var titleTxt = title.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = "GAME OVER";
            titleTxt.fontSize = 56;
            titleTxt.color = new Color(0.914f, 0.271f, 0.376f);
            titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            
            // Final Score
            CreateHUDText(panel.transform, "FinalScoreText", "FinalScoreText", new Vector2(0, 50), new Vector2(300, 80));
            
            // Best Score
            CreateHUDText(panel.transform, "BestScoreText", "BestScoreText", new Vector2(0, -30), new Vector2(300, 60));
            
            // Retry Button
            CreateButton(panel.transform, "RetryButton", "RETRY", new Vector2(0, -150), new Color(0.263f, 0.38f, 0.933f));
            
            // Menu Button
            CreateButton(panel.transform, "MenuButton", "MENU", new Vector2(0, -250), new Color(0.5f, 0.5f, 0.5f));
            
            EditorSceneManager.SaveScene(scene, "Assets/_Game/Scenes/GameOverScene.unity");
            Debug.Log("[GameSetup] ✅ Created GameOverScene");
        }
        
        private static void SetupBuildSettings()
        {
            var scenes = new[]
            {
                "Assets/_Game/Scenes/MenuScene.unity",
                "Assets/_Game/Scenes/GameScene.unity",
                "Assets/_Game/Scenes/GameOverScene.unity"
            };
            
            var editorBuildSettingsScenes = new EditorBuildSettingsScene[scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                editorBuildSettingsScenes[i] = new EditorBuildSettingsScene(scenes[i], true);
            }
            EditorBuildSettings.scenes = editorBuildSettingsScenes;
            
            Debug.Log("[GameSetup] ✅ Build settings configured");
        }
        
        private static void CreateButton(Transform parent, string name, string text, Vector2 pos, Color color)
        {
            var btn = new GameObject(name);
            btn.transform.SetParent(parent, false);
            var rect = btn.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(250, 80);
            
            var img = btn.AddComponent<UnityEngine.UI.Image>();
            img.color = color;
            
            var button = btn.AddComponent<UnityEngine.UI.Button>();
            
            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btn.transform, false);
            txtObj.AddComponent<RectTransform>().sizeDelta = rect.sizeDelta;
            var txt = txtObj.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = text;
            txt.fontSize = 32;
            txt.color = Color.white;
            txt.alignment = TMPro.TextAlignmentOptions.Center;
        }
        
        private static void CreateHUDText(Transform parent, string name, string defText, Vector2 pos, Vector2 size)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            var txt = obj.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = defText == "ScoreText" ? "0" : defText == "ComboText" ? "x1.0" : defText == "LivesText" ? "3/3" : defText == "TimerText" ? "60" : defText == "FinalScoreText" ? "0" : "Best: 0";
            txt.fontSize = defText == "FinalScoreText" ? 56 : 36;
            txt.color = Color.white;
            txt.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }
}
