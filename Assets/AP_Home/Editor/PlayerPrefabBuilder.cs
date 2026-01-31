using UnityEditor;
using UnityEngine;

public static class PlayerPrefabBuilder
{
    [MenuItem("Tools/Save Player Prefab")]
    public static void SavePlayerPrefab()
    {
        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("PlayerPrefabBuilder: GameObject 'Player' not found in the active scene.");
            return;
        }

        const string prefabPath = "Assets/Prefabs/Player.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(player, prefabPath, InteractionMode.UserAction);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"PlayerPrefabBuilder: Saved prefab at {prefabPath}");
    }
}
