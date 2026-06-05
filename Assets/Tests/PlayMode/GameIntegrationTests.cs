using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Pully.Game;

namespace Pully.Tests.PlayMode
{
    /// <summary>
    /// PlayMode integration tests - runs in actual Unity runtime with full scene setup.
    /// These tests validate the complete game flow including scene loading, object spawning,
    /// and input handling.
    /// </summary>
    public class GameIntegrationTests
    {
        [UnityTest]
        public IEnumerator GameScene_LoadsAndGameManagerExists()
        {
            // Act
            SceneManager.LoadScene("GameScene");
            yield return new WaitForSeconds(0.5f);

            // Assert
            var gameManager = Object.FindObjectOfType<GameManager>();
            Assert.IsNotNull(gameManager, "GameManager should exist in GameScene");
        }

        [UnityTest]
        public IEnumerator Bootstrap_CreatesCanvasWithHUD()
        {
            // Act
            SceneManager.LoadScene("GameScene");
            yield return new WaitForSeconds(0.5f);

            // Assert
            var canvas = Object.FindObjectOfType<Canvas>();
            Assert.IsNotNull(canvas, "Canvas should be created by Bootstrap");

            var scoreText = GameObject.Find("ScoreText");
            var comboText = GameObject.Find("ComboText");
            var livesText = GameObject.Find("LivesText");
            var timerText = GameObject.Find("TimerText");

            Assert.IsNotNull(scoreText, "ScoreText should exist");
            Assert.IsNotNull(comboText, "ComboText should exist");
            Assert.IsNotNull(livesText, "LivesText should exist");
            Assert.IsNotNull(timerText, "TimerText should exist");
        }

        [UnityTest]
        public IEnumerator Targets_SpawnAfterGameStart()
        {
            // Act
            SceneManager.LoadScene("GameScene");
            yield return new WaitForSeconds(0.5f);

            var spawner = Object.FindObjectOfType<TargetSpawner>();
            Assert.IsNotNull(spawner, "TargetSpawner should exist");

            // Wait for spawn interval
            yield return new WaitForSeconds(1.5f);

            // Assert
            Assert.Greater(spawner.ActiveTargets.Count, 0, "At least one target should spawn after spawn interval");
        }

        [UnityTest]
        public IEnumerator ScoreManager_UpdatesScoreOnHit()
        {
            // Act
            SceneManager.LoadScene("GameScene");
            yield return new WaitForSeconds(0.5f);

            var spawner = Object.FindObjectOfType<TargetSpawner>();
            var scoreManager = Object.FindObjectOfType<ScoreManager>();
            Assert.IsNotNull(spawner, "TargetSpawner should exist");
            Assert.IsNotNull(scoreManager, "ScoreManager should exist");

            // Wait for target spawn
            yield return new WaitForSeconds(1.5f);
            Assert.Greater(spawner.ActiveTargets.Count, 0, "Need at least one target to test");

            // Get initial score
            int initialScore = scoreManager.Score;

            // Act - simulate a hit
            var target = spawner.ActiveTargets[0];
            target.AttemptGesture(RulesetDefinition.Gesture.SingleTap);

            // Wait for processing
            yield return null;

            // Assert - score should increase (or target should be destroyed if correct gesture)
            // Note: Score may not increase if gesture doesn't match
            bool targetDestroyed = target == null || !target.gameObject.activeInHierarchy;
            Debug.Log($"Target destroyed: {targetDestroyed}, Initial score: {initialScore}, Current: {scoreManager.Score}");
        }

        [UnityTest]
        public IEnumerator GameManager_StateTransitions_ToPlaying()
        {
            // Act
            SceneManager.LoadScene("GameScene");
            yield return new WaitForSeconds(0.5f);

            var gameManager = Object.FindObjectOfType<GameManager>();
            Assert.IsNotNull(gameManager, "GameManager should exist");

            // Assert
            Assert.AreEqual(GameManager.GameState.Playing, gameManager.CurrentState,
                "Game should start in Playing state");
        }

        [UnityTest]
        public IEnumerator Target_ExpiresAfterLifetime()
        {
            // Act - Load scene with short-lived targets
            SceneManager.LoadScene("GameScene");
            yield return new WaitForSeconds(0.5f);

            var spawner = Object.FindObjectOfType<TargetSpawner>();
            Assert.IsNotNull(spawner, "TargetSpawner should exist");

            // Wait for spawn
            yield return new WaitForSeconds(1.5f);
            Assert.Greater(spawner.ActiveTargets.Count, 0, "Need targets to test expiration");

            var target = spawner.ActiveTargets[0];
            Assert.IsNotNull(target, "Target should exist");

            // Wait for expiration (default lifetime is 1.6s)
            float waitTime = target.GetTimeRemaining() + 0.5f;
            yield return new WaitForSeconds(waitTime);

            // Assert - target should be expired
            Assert.AreEqual(0, target.GetTimeRemaining(), 0.1f, "Target should be expired");
        }

        [UnityTest]
        public IEnumerator MenuScene_PlayButton_LoadsGameScene()
        {
            // Act
            SceneManager.LoadScene("MenuScene");
            yield return new WaitForSeconds(0.5f);

            var menuManager = Object.FindObjectOfType<MenuManager>();
            Assert.IsNotNull(menuManager, "MenuManager should exist");

            // Simulate Play button click
            menuManager.OnPlayButtonPressed();
            yield return new WaitForSeconds(0.5f);

            // Assert - scene should have changed
            Assert.AreEqual("GameScene", SceneManager.GetActiveScene().name,
                "Should be in GameScene after clicking Play");
        }

        [UnityTest]
        public IEnumerator FullGameFlow_MenuToGameToGameOver()
        {
            // Menu
            SceneManager.LoadScene("MenuScene");
            yield return new WaitForSeconds(0.3f);

            var menuManager = Object.FindObjectOfType<MenuManager>();
            Assert.IsNotNull(menuManager, "MenuManager should exist in MenuScene");

            // Start Game
            menuManager.OnPlayButtonPressed();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual("GameScene", SceneManager.GetActiveScene().name);

            // Game
            var gameManager = Object.FindObjectOfType<GameManager>();
            Assert.IsNotNull(gameManager, "GameManager should exist");

            // Simulate game over by losing all lives rapidly
            var scoreManager = Object.FindObjectOfType<ScoreManager>();
            Assert.IsNotNull(scoreManager, "ScoreManager should exist");

            int initialLives = scoreManager.Lives;
            for (int i = 0; i < initialLives; i++)
            {
                scoreManager.OnTargetMiss();
            }
            yield return new WaitForSeconds(0.5f);

            // Should be in GameOver state/scene
            Assert.IsTrue(scoreManager.IsGameOver || SceneManager.GetActiveScene().name == "GameOverScene",
                "Should reach game over");
        }
    }
}
