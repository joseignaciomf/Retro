#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class LevelPrefabBuilder
{
    [MenuItem("RetroRescue/Build Example Prefabs")]
    public static void BuildExamplePrefabs()
    {
        string prefabsDir = "Assets/Prefabs";
        string levelsDir = "Assets/Levels";
        if (!Directory.Exists(prefabsDir)) Directory.CreateDirectory(prefabsDir);
        if (!Directory.Exists(levelsDir)) Directory.CreateDirectory(levelsDir);
        AssetDatabase.Refresh();

        // --- Cassette Level Prefab ---
        var root = new GameObject("Level_Cassette_Example");
        var leftGear = new GameObject("LeftGear"); leftGear.transform.SetParent(root.transform); leftGear.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
        var rightGear = new GameObject("RightGear"); rightGear.transform.SetParent(root.transform); rightGear.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        var tapeGO = new GameObject("Tape"); tapeGO.transform.SetParent(root.transform); tapeGO.transform.localPosition = new Vector3(0f, -0.25f, 0f);
        var sr = tapeGO.AddComponent<SpriteRenderer>();

        var cassetteController = root.AddComponent<RetroRescue.Features.Cassette.CassetteController>();
        cassetteController.levelId = "REQ-M2-L1-CASSETTE";

        var input = root.AddComponent<RetroRescue.Features.Cassette.CassetteInput>();
        input.leftGearCenter = leftGear.transform;
        input.rightGearCenter = rightGear.transform;

        var tapeVisual = tapeGO.AddComponent<RetroRescue.Features.Cassette.TapeVisualController>();
        tapeVisual.levelId = cassetteController.levelId;

        string prefabPath = Path.Combine(prefabsDir, "Level_Cassette_Example.prefab");
        prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

        // Create LevelDataSO asset
        var levelData = ScriptableObject.CreateInstance<RetroRescue.Data.LevelDataSO>();
        levelData.levelID = cassetteController.levelId;
        levelData.orderIndex = 1;
        levelData.levelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        levelData.levelDurationSeconds = 15f;
        levelData.mechanicType = RetroRescue.Data.MechanicType.CASSETTE;
        levelData.difficultyModifier = cassetteController.rewindK;
        levelData.mainAudioEventPath = "event:/Music/CassetteLoop";

        string levelAssetPath = Path.Combine(levelsDir, "LevelData_Cassette01.asset");
        levelAssetPath = AssetDatabase.GenerateUniqueAssetPath(levelAssetPath);
        AssetDatabase.CreateAsset(levelData, levelAssetPath);

        // --- TV Tuning placeholder prefab ---
        var tvRoot = new GameObject("Level_TVTuning_Example");
        var tvFrame = new GameObject("TV_Frame"); tvFrame.transform.SetParent(tvRoot.transform);
        var screen = new GameObject("Screen"); screen.transform.SetParent(tvRoot.transform);
        var screenSr = screen.AddComponent<SpriteRenderer>();

        string tvPrefabPath = Path.Combine(prefabsDir, "Level_TVTuning_Example.prefab");
        tvPrefabPath = AssetDatabase.GenerateUniqueAssetPath(tvPrefabPath);
        PrefabUtility.SaveAsPrefabAsset(tvRoot, tvPrefabPath);

        var tvLevelData = ScriptableObject.CreateInstance<RetroRescue.Data.LevelDataSO>();
        tvLevelData.levelID = "REQ-M2-L2-TV";
        tvLevelData.orderIndex = 2;
        tvLevelData.levelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(tvPrefabPath);
        tvLevelData.levelDurationSeconds = 20f;
        tvLevelData.mechanicType = RetroRescue.Data.MechanicType.TV_TUNING;
        tvLevelData.difficultyModifier = 1.0f;
        tvLevelData.mainAudioEventPath = "event:/Music/TVTuning";

        string tvLevelAssetPath = Path.Combine(levelsDir, "LevelData_TV01.asset");
        tvLevelAssetPath = AssetDatabase.GenerateUniqueAssetPath(tvLevelAssetPath);
        AssetDatabase.CreateAsset(tvLevelData, tvLevelAssetPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GameObject.DestroyImmediate(root);
        GameObject.DestroyImmediate(tvRoot);

        EditorUtility.DisplayDialog("RetroRescue", "Prefabs y LevelData creados en Assets/Prefabs y Assets/Levels.", "OK");
    }
}
#endif
