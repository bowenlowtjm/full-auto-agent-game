using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Pully.Game;

namespace Pully.Tests.PlayMode
{
    /// <summary>
    /// PlayMode tests for gesture-to-score path.
    /// </summary>
    public class GestureToScoreTests
    {
        [UnityTest]
        public IEnumerator ScoreManager_SingleTapGesture_UpdatesScore()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;

            var go = new GameObject("ScoreManager");
            var sm = go.AddComponent<ScoreManager>();
            var field = typeof(ScoreManager).GetField("ruleset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(sm, ruleset);
            sm.Initialize();

            int initialScore = sm.Score;
            sm.OnTargetHit(1);
            yield return null;

            Assert.Greater(sm.Score, initialScore);
            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator ScoreManager_SequenceOfHits_IncreasesCombo()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;
            ruleset.comboStep = 1.1f;

            var go = new GameObject("ScoreManager");
            var sm = go.AddComponent<ScoreManager>();
            var field = typeof(ScoreManager).GetField("ruleset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(sm, ruleset);
            sm.Initialize();

            float initialCombo = sm.ComboMultiplier;
            sm.OnTargetHit(1);
            sm.OnTargetHit(1);
            yield return null;

            Assert.Greater(sm.ComboMultiplier, initialCombo);
            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator ScoreManager_WrongGesture_BreaksCombo()
        {
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.lives = 3;
            ruleset.comboStep = 1.1f;

            var go = new GameObject("ScoreManager");
            var sm = go.AddComponent<ScoreManager>();
            var field = typeof(ScoreManager).GetField("ruleset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(sm, ruleset);
            sm.Initialize();

            sm.OnTargetHit(1);
            sm.OnTargetHit(1);
            Assert.Greater(sm.ComboMultiplier, 1f);

            sm.OnTargetMiss();
            yield return null;

            Assert.AreEqual(1f, sm.ComboMultiplier);
            Object.Destroy(go);
        }
    }
}
