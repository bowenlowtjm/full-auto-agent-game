using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Pully.Editor
{
    public static class Builder
    {
        [MenuItem("Pully/Build Android APK")]
        public static void BuildAndroid()
        {
            string buildPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds");
            Directory.CreateDirectory(buildPath);
            
            string apkPath = Path.Combine(buildPath, $"Pully_{DateTime.Now:yyyyMMdd_HHmmss}.apk");
            
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/_Game/Scenes/MenuScene.unity",
                    "Assets/_Game/Scenes/GameScene.unity",
                    "Assets/_Game/Scenes/GameOverScene.unity"
                },
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };
            
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.pully.game");
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            
            Debug.Log($"[BUILD] Starting Android build to: {apkPath}");
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildResult result = report.summary.result;
            
            if (result == BuildResult.Succeeded)
            {
                Debug.Log($"[BUILD] SUCCESS: {apkPath}");
                long size = report.summary.totalSize / 1024 / 1024;
                Debug.Log($"[BUILD] Size: {size} MB");
            }
            else
            {
                Debug.LogError($"[BUILD] FAILED: {result}");
            }
        }
        
        // Command line build entry point
        public static void BuildAndroidCI()
        {
            Debug.Log("[BUILD] Running CI build...");
            
            // Configure Android settings
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.pully.game");
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            
            // Update project
            AssetDatabase.Refresh();
            
            // Build
            string buildPath = "Builds/Pully_CI.apk";
            Directory.CreateDirectory("Builds");
            
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/_Game/Scenes/MenuScene.unity",
                    "Assets/_Game/Scenes/GameScene.unity",
                    "Assets/_Game/Scenes/GameOverScene.unity"
                },
                locationPathName = buildPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };
            
            BuildReport report = BuildPipeline.BuildPlayer(options);
            
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("[BUILD] CI build succeeded: " + buildPath);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("[BUILD] CI build failed: " + report.summary.result);
                EditorApplication.Exit(1);
            }
        }
    }
}
