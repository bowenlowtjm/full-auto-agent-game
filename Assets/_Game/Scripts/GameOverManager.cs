using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Pully.Game
{
    public class GameOverManager : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI finalScoreText;
        [SerializeField] private TMPro.TextMeshProUGUI bestScoreText;
        [SerializeField] private GameObject newHighScoreText;
        
        private void Start()
        {
            int currentScore = PlayerPrefs.GetInt("Pully_CurrentScore", 0);
            int bestScore = PlayerPrefs.GetInt("Pully_BestScore", 0);
            
            bool isNewHighScore = currentScore > bestScore;
            if (isNewHighScore)
            {
                bestScore = currentScore;
                PlayerPrefs.SetInt("Pully_BestScore", bestScore);
                PlayerPrefs.Save();
            }
            
            if (finalScoreText != null) finalScoreText.text = currentScore.ToString();
            if (bestScoreText != null) bestScoreText.text = $"Best: {bestScore}";
            if (newHighScoreText != null) newHighScoreText.SetActive(isNewHighScore);
        }
        
        public void OnRetryButtonPressed() => SceneManager.LoadScene("GameScene");
        public void OnMenuButtonPressed() => SceneManager.LoadScene("MenuScene");
    }
}
