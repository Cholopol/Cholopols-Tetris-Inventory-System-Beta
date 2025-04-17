using ChosTIS.SaveLoadSystem;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad] // Ensure that classes are automatically initialized during script recompilation
public static class PlayModeDataCleaner
{
    static PlayModeDataCleaner()
    {
        // Register Play Mode status change event
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Trigger reset only when exiting Play Mode
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            ResetScriptableObjectData();
        }
    }

    private static void ResetScriptableObjectData()
    {
        // Load the target ScriptableObject resource
        InventoryData_SO data = AssetDatabase.LoadAssetAtPath<InventoryData_SO>("Assets/GameData/SlotItemData/InventoryData_SO.asset");
        if (data != null)
        {
            // Clear the list data
            data.inventoryItemList.Clear();
            // Mark data as dirty (needs to be saved)
            EditorUtility.SetDirty(data);
            // Force saving of resource modifications
            AssetDatabase.SaveAssets();
            Debug.Log("Temporary data has been reset and saved!");
        }
    }
}