using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pully.Game
{
    /// <summary>
    /// Bootstrap for GameOverScene - shows score, best score, retry/menu buttons.
    /// </summary>
    public class GameOverBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            var panel = GameObject.Find("GameOverPanel");
            if (panel == null)
            {
                panel = new GameObject("GameOverPanel");
                panel.transform.SetParent(canvas.transform, false);
                var panelRect = panel.AddComponent<RectTransform>();
                panelRect.anchorMin = Vector2.zero;
                panelRect.anchorMax = Vector2.one;
                panelRect.sizeDelta = Vector2.zero;
                var panelImg = panel.AddComponent<Image>();
                panelImg.color = new Color(0.1f, 0.1f, 0.18f);
            }
            
            // Load scores
            int currentScore = PlayerPrefs.GetInt("Pully_CurrentScore", 0);
            int bestScore = PlayerPrefs.GetInt("Pully_BestScore", 0);
            bool isNewBest = currentScore > bestScore && currentScore > 0;
            
            if (isNewBest)
            {
                PlayerPrefs.SetInt("Pully_BestScore", currentScore);
                PlayerPrefs.Save();
                bestScore = currentScore;
            }
            
            // Title
            CreateText(panel.transform, "GAME OVER", 56, new Color(0.91f, 0.27f, 0.38f), new Vector2(0, 300));
            
            // Score
            CreateText(panel.transform, $"Score: {currentScore}", 48, Color.white, new Vector2(0, 150));
            
            // Best
            CreateText(panel.transform, $"Best: {bestScore}", 36, Color.yellow, new Vector2(0, 80));
            
            // New Best badge
            if (isNewBest)
                CreateText(panel.transform, "NEW BEST!", 28, new Color(1f, 0.8f, 0.2f), new Vector2(0, 30));
            
            // Retry Button
            CreateButton(panel.transform, "RETRY", new Vector2(0, -100), new Color(0.26f, 0.38f, 0.93f), 
                () => SceneManager.LoadScene("GameScene"));
            
            // Menu Button
            CreateButton(panel.transform, "MENU", new Vector2(0, -220), new Color(0.5f, 0.5f, 0.5f), 
                () => SceneManager.LoadScene("MenuScene"));
            
            Debug.Log("[GameOverBootstrap] ✅ GameOver ready");
        }
        
        private void CreateText(Transform parent, string text, int fontSize, Color color, Vector2 pos)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(800, 80);
            var txt = go.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.color = color;
            txt.alignment = TMPro.TextAlignmentOptions.Center;
        }
        
        private void CreateButton(Transform parent, string text, Vector2 pos, Color color, System.Action onClick)
        {
            var go = new GameObject(text + "Button");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(250, 80);
            
            var img = go.AddComponent<Image>();
            img.color = color;
            
            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(() => onClick());
            
            var txtGo = new GameObject("Text");
            txtGo.transform.SetParent(go.transform, false);
            txtGo.AddComponent<RectTransform>().sizeDelta = rect.sizeDelta;
            var txt = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = text;
            txt.fontSize = 36;
            txt.color = Color.white;
            txt.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }
}
