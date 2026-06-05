using UnityEditor;

public static class AutoRefresher
{
    public static void RefreshAssets()
    {
        // Forces Unity to look for changed files and compile them
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
}
