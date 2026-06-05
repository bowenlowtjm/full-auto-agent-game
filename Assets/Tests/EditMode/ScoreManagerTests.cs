using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Pully.Game;

namespace Pully.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ScoreManager - validates core scoring logic without Unity runtime.
    /// </summary>
    public class ScoreManagerTests
    {
        private ScoreManager scoreManager;
        private RulesetDefinition ruleset;

        [SetUp]
        public void SetUp()
        {
            scoreManager = new GameObject().AddComponent<ScoreManager>();
            ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;
            ruleset.lives = 3;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(scoreManager.gameObject);
            Object.DestroyImmediate(ruleset);
        }

        [Test]
        public void Initialize_SetsDefaultValues()
        {
            // Arrange
            var field = scoreManager.GetType().GetField("ruleset",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            field.SetValue(scoreManager, ruleset);

            // Act
            scoreManager.Initialize();

            // Assert
            Assert.AreEqual(0, scoreManager.Score);
            Assert.AreEqual(1f, scoreManager.Combo);
            Assert.AreEqual(3, scoreManager.Lives);
            Assert.IsFalse(scoreManager.IsGameOver);
        }

        [Test]
        public void OnTargetHit_IncreasesScoreAndCombo()
        {
            // Arrange
            SetupScoreManager();
            scoreManager.Initialize();
            int initialScore = scoreManager.Score;

            // Act
            scoreManager.OnTargetHit(1);

            // Assert
            Assert.Greater(scoreManager.Score, initialScore);
            Assert.Greater(scoreManager.Combo, 1f);
        }

        [Test]
        public void OnTargetMiss_DecreasesLivesAndResetsCombo()
        {
            // Arrange
            SetupScoreManager();
            scoreManager.Initialize();
            int initialLives = scoreManager.Lives;
            scoreManager.OnTargetHit(1); // Build combo first
            Assert.Greater(scoreManager.Combo, 1f);

            // Act
            scoreManager.OnTargetMiss();

            // Assert
            Assert.AreEqual(initialLives - 1, scoreManager.Lives);
            Assert.AreEqual(1f, scoreManager.Combo);
        }

        [Test]
        public void Combo_CapsAtMaxValue()
        {
            // Arrange
            SetupScoreManager();
            scoreManager.Initialize();
            float maxCombo = ruleset.comboCap;

            // Act - hit many targets to build combo
            for (int i = 0; i < 20; i++)
            {
                scoreManager.OnTargetHit(1);
            }

            // Assert
            Assert.LessOrEqual(scoreManager.Combo, maxCombo + 0.01f);
        }

        [Test]
        public void LivesReachZero_TriggersGameOver()
        {
            // Arrange
            SetupScoreManager();
            scoreManager.Initialize();
            bool gameOverFired = false;
            scoreManager.OnGameOver.AddListener(() => gameOverFired = true);

            // Act - miss all lives
            for (int i = 0; i < ruleset.lives; i++)
            {
                scoreManager.OnTargetMiss();
            }

            // Assert
            Assert.IsTrue(gameOverFired);
            Assert.IsTrue(scoreManager.IsGameOver);
        }

        [Test]
        [TestCase(1, 1.1f, 1)]   // Score + combo multiplier
        [TestCase(5, 1.1f, 5)]   // Higher base score
        [TestCase(1, 2.0f, 2)]   // Combo multiplier applied
        public void ScoreCalculation_IsCorrect(int baseReward, float combo, int expectedMinScore)
        {
            // Arrange
            SetupScoreManager();
            scoreManager.Initialize();

            // Build to target combo
            while (scoreManager.Combo < combo && scoreManager.Combo < ruleset.comboCap)
            {
                scoreManager.OnTargetHit(1);
            }

            int scoreBefore = scoreManager.Score;

            // Act
            scoreManager.OnTargetHit(baseReward);

            // Assert - score should increase by at least baseReward * combo
            int scoreIncrease = scoreManager.Score - scoreBefore;
            Assert.GreaterOrEqual(scoreIncrease, (int)(baseReward * combo) - 1);
        }

        private void SetupScoreManager()
        {
            var field = scoreManager.GetType().GetField("ruleset",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            field.SetValue(scoreManager, ruleset);
        }
    }
}
