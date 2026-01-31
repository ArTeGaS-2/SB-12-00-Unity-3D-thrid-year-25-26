using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RuinsGeneratorMenu
{
    [MenuItem("Tools/Ruins/Generate")]
    public static void Generate()
    {
        var generator = Object.FindFirstObjectByType<RuinsGenerator>();
        if (generator == null)
        {
            Debug.LogError("RuinsGeneratorMenu: No RuinsGenerator found in the active scene.");
            return;
        }

        generator.Generate();
        EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
    }

    [MenuItem("Tools/Ruins/Clear")]
    public static void Clear()
    {
        var generator = Object.FindFirstObjectByType<RuinsGenerator>();
        if (generator == null)
        {
            Debug.LogError("RuinsGeneratorMenu: No RuinsGenerator found in the active scene.");
            return;
        }

        generator.ClearGenerated();
        EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
    }
}
