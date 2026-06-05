using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private TMPro.TextMeshProUGUI bestScoreText;
        
        private void Start()
        {
            int bestScore = PlayerPrefs.GetInt("Pully_BestScore", 0);
            if (bestScoreText != null) bestScoreText.text = $"Best: {bestScore}";
            ShowMainMenu();
        }
        
        public void OnPlayButtonPressed() => SceneManager.LoadScene("GameScene");
        public void OnCreditsButtonPressed()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(true);
        }
        public void OnBackButtonPressed() => ShowMainMenu();
        public void OnQuitButtonPressed() => Application.Quit();
        
        private void ShowMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (creditsPanel != null) creditsPanel.SetActive(false);
        }
    }
}
