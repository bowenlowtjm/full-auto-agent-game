using NUnit.Framework;
using Pully.Game;
using UnityEngine;

namespace Pully.Tests.EditMode
{
    /// <summary>
    /// EditMode tests for scoring and combo logic.
    /// </summary>
    public class ScoringTests
    {
        private ScoreManager CreateScoreManager(RulesetDefinition ruleset)
        {
            var go = new GameObject("ScoreManager");
            var sm = go.AddComponent<ScoreManager>();
            var field = typeof(ScoreManager).GetField("ruleset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(sm, ruleset);
            return sm;
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up all created GameObjects
            var objects = GameObject.FindObjectsOfType<ScoreManager>();
            foreach (var obj in objects)
            {
                if (obj != null && obj.gameObject != null)
                    Object.DestroyImmediate(obj.gameObject);
            }
        }

        [Test]
        public void ScoreManager_InitializesWithCorrectLives()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;

            var sm = CreateScoreManager(ruleset);
            sm.Initialize();

            Assert.AreEqual(3, sm.Lives);
            Assert.AreEqual(0, sm.Score);
            Assert.AreEqual(1f, sm.ComboMultiplier);
        }

        [Test]
        public void ScoreManager_CorrectHit_IncreasesScore()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;

            var sm = CreateScoreManager(ruleset);
            sm.Initialize();

            sm.OnTargetHit(10);
            Assert.AreEqual(10, sm.Score);
            Assert.AreEqual(1.1f, sm.ComboMultiplier);
        }

        [Test]
        public void ScoreManager_ComboIncreasesWithConsecutiveHits()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;

            var sm = CreateScoreManager(ruleset);
            sm.Initialize();

            sm.OnTargetHit(1);
            Assert.AreEqual(1.1f, sm.ComboMultiplier);

            sm.OnTargetHit(1);
            Assert.AreEqual(2.2f, sm.ComboMultiplier);
        }

        [Test]
        public void ScoreManager_ComboCapsAtMax()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;

            var sm = CreateScoreManager(ruleset);
            sm.Initialize();

            for (int i = 0; i < 10; i++)
                sm.OnTargetHit(1);

            Assert.AreEqual(5f, sm.ComboMultiplier);
        }

        [Test]
        public void ScoreManager_Miss_ResetsCombo()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;

            var sm = CreateScoreManager(ruleset);
            sm.Initialize();

            sm.OnTargetHit(1);
            sm.OnTargetHit(1);
            Assert.AreEqual(2.2f, sm.ComboMultiplier);

            sm.OnTargetMiss();
            Assert.AreEqual(1f, sm.ComboMultiplier);
        }

        [Test]
        public void ScoreManager_Miss_ReducesLives()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;

            var sm = CreateScoreManager(ruleset);
            sm.Initialize();

            sm.OnTargetMiss();
            Assert.AreEqual(2, sm.Lives);

            sm.OnTargetMiss();
            sm.OnTargetMiss();
            Assert.IsTrue(sm.IsGameOver);
        }
    }
}
