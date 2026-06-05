using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Pully.Game;

namespace Pully.Tests.EditMode
{
    /// <summary>
    /// Unit tests for GestureRecognizer logic without requiring Unity Input System.
    /// </summary>
    public class GestureRecognizerTests
    {
        private GestureRecognizer gestureRecognizer;
        private RulesetDefinition ruleset;

        [SetUp]
        public void SetUp()
        {
            gestureRecognizer = new GameObject().AddComponent<GestureRecognizer>();
            ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            ruleset.doubleTapWindow = 0.3f;
            ruleset.longPressDuration = 0.5f;
            ruleset.swipeMinDistance = 50f;

            // Inject ruleset via reflection (simulating Bootstrap behavior)
            var field = gestureRecognizer.GetType().GetField("ruleset",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            field.SetValue(gestureRecognizer, ruleset);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gestureRecognizer.gameObject);
            Object.DestroyImmediate(ruleset);
        }

        [Test]
        public void GetRequiredGesture_ReturnsCorrectGesture_ForShape()
        {
            // Arrange
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = Color.cyan,
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 1
            });
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Square,
                color = Color.blue,
                requiredGesture = RulesetDefinition.Gesture.DoubleTap,
                baseReward = 3
            });

            // Act & Assert
            var gesture1 = gestureRecognizer.GetRequiredGesture(RulesetDefinition.Shape.Circle, Color.cyan);
            var gesture2 = gestureRecognizer.GetRequiredGesture(RulesetDefinition.Shape.Square, Color.blue);

            Assert.AreEqual(RulesetDefinition.Gesture.SingleTap, gesture1);
            Assert.AreEqual(RulesetDefinition.Gesture.DoubleTap, gesture2);
        }

        [Test]
        public void GetRequiredGesture_DefaultsToSingleTap_WhenNoMatch()
        {
            // Act
            var gesture = gestureRecognizer.GetRequiredGesture(RulesetDefinition.Shape.Star, Color.magenta);

            // Assert
            Assert.AreEqual(RulesetDefinition.Gesture.SingleTap, gesture);
        }

        [Test]
        public void ColorsMatch_DetectsSimilarColors()
        {
            // Arrange
            Color color1 = new Color(0.298f, 0.788f, 0.941f);
            Color color2 = new Color(0.2981f, 0.7881f, 0.9411f); // Slightly different
            Color color3 = new Color(1f, 0f, 0f); // Completely different

            var method = gestureRecognizer.GetType().GetMethod("ColorsMatch",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Act
            bool match1 = (bool)method.Invoke(gestureRecognizer, new object[] { color1, color2 });
            bool match2 = (bool)method.Invoke(gestureRecognizer, new object[] { color1, color3 });

            // Assert
            Assert.IsTrue(match1); // Similar colors match
            Assert.IsFalse(match2); // Different colors don't match
        }
    }
}
