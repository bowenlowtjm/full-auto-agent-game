using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pully.Game
{
    /// <summary>
    /// Bootstrap for MenuScene - creates UI and wires buttons.
    /// </summary>
    public class MenuBootstrap : MonoBehaviour
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
            
            // Find or create panel
            var panel = GameObject.Find("MenuPanel");
            if (panel == null)
            {
                panel = new GameObject("MenuPanel");
                panel.transform.SetParent(canvas.transform, false);
                var panelRect = panel.AddComponent<RectTransform>();
                panelRect.anchorMin = Vector2.zero;
                panelRect.anchorMax = Vector2.one;
                panelRect.sizeDelta = Vector2.zero;
                var panelImg = panel.AddComponent<Image>();
                panelImg.color = new Color(0.1f, 0.1f, 0.18f);
            }
            
            // Get best score
            int bestScore = PlayerPrefs.GetInt("Pully_BestScore", 0);
            
            // Title
            CreateText(panel.transform, "PULLY", 72, new Color(0.91f, 0.27f, 0.38f), new Vector2(0, 400));
            
            // Best Score
            if (bestScore > 0)
                CreateText(panel.transform, $"Best: {bestScore}", 36, Color.white, new Vector2(0, 250));
            
            // Play Button
            CreateButton(panel.transform, "PLAY", new Vector2(0, 50), new Color(0.26f, 0.38f, 0.93f), () => SceneManager.LoadScene("GameScene"));
            
            // Instructions
            CreateText(panel.transform, "Tap the right shapes!", 28, Color.gray, new Vector2(0, -200));
            CreateText(panel.transform, "Cyan: Tap | Red: Hold | Blue: Double", 22, Color.gray, new Vector2(0, -250));
            CreateText(panel.transform, "Pink: Swipe | Purple: Two-Finger", 22, Color.gray, new Vector2(0, -280));
            
            Debug.Log("[MenuBootstrap] ✅ Menu ready");
        }
        
        private void CreateText(Transform parent, string text, int fontSize, Color color, Vector2 pos)
        {
            var go = new GameObject(text.Substring(0, Mathf.Min(10, text.Length)));
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(800, 100);
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
            rect.sizeDelta = new Vector2(300, 100);
            
            var img = go.AddComponent<Image>();
            img.color = color;
            
            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(() => onClick());
            
            // Text
            var txtGo = new GameObject("Text");
            txtGo.transform.SetParent(go.transform, false);
            txtGo.AddComponent<RectTransform>().sizeDelta = rect.sizeDelta;
            var txt = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
            txt.text = text;
            txt.fontSize = 40;
            txt.color = Color.white;
            txt.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }
}
