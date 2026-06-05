using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pully.Game
{
    /// <summary>
    /// Bootstrapper to create the Ruleset ScriptableObject if it doesn't exist.
    /// Run this from Editor menu: Pully/Create Ruleset
    /// </summary>
#if UNITY_EDITOR
    using UnityEditor;
    
    public static class RulesetCreator
    {
        [MenuItem("Pully/Create Ruleset")]
        public static void CreateRuleset()
        {
            string path = "Assets/_Game/Data/";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            
            var ruleset = ScriptableObject.CreateInstance<RulesetDefinition>();
            
            // Setup default rules from spec/RULESET.md
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = new Color(0.298f, 0.788f, 0.941f), // #4CC9F0 - Green/Cyan-ish
                requiredGesture = RulesetDefinition.Gesture.SingleTap,
                baseReward = 1
            });
            
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Circle,
                color = new Color(1f, 0.267f, 0.267f), // #FF4444 - Red
                requiredGesture = RulesetDefinition.Gesture.LongPress,
                baseReward = 5
            });
            
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Square,
                color = new Color(0.263f, 0.38f, 0.933f), // #4361EE - Blue
                requiredGesture = RulesetDefinition.Gesture.DoubleTap,
                baseReward = 3
            });
            
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Triangle,
                color = new Color(0.969f, 0.145f, 0.522f), // #F72585 - Yellow/Pink-ish
                requiredGesture = RulesetDefinition.Gesture.SwipeTap,
                baseReward = 5
            });
            
            ruleset.rules.Add(new RulesetDefinition.TargetRule
            {
                shape = RulesetDefinition.Shape.Star,
                color = new Color(0.447f, 0.035f, 0.718f), // #7209B7 - Purple
                requiredGesture = RulesetDefinition.Gesture.TwoFingerTap,
                baseReward = 8
            });
            
            // Scoring params
            ruleset.comboStep = 1.1f;
            ruleset.comboCap = 5f;
            ruleset.lives = 3;
            ruleset.roundSeconds = 60f;
            ruleset.targetLifetime = 1.6f;
            ruleset.spawnIntervalStart = 1.2f;
            ruleset.spawnIntervalEnd = 0.6f;
            ruleset.maxConcurrentTargets = 4;
            ruleset.seed = 12345;
            
            AssetDatabase.CreateAsset(ruleset, path + "DefaultRuleset.asset");
            AssetDatabase.SaveAssets();
            
            Debug.Log("[RulesetCreator] Created DefaultRuleset at " + path);
            EditorGUIUtility.PingObject(ruleset);
        }
    }
#endif
}
