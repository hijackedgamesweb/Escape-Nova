#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Code.Scripts.Core.Systems.Astrarium;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(AstrariumDatabaseSO))]
public class AstrariumDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AstrariumDatabaseSO db = (AstrariumDatabaseSO)target;

        GUILayout.Space(20);
        if (GUILayout.Button("Auto-Find All Astrarium Entries", GUILayout.Height(40)))
        {
            FindAllEntries(db);
        }
    }

    private void FindAllEntries(AstrariumDatabaseSO db)
    {
        List<ScriptableObject> foundAssets = new List<ScriptableObject>();
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (so is IAstrariumEntry)
            {
                if (!foundAssets.Contains(so))
                {
                    foundAssets.Add(so);
                }
            }
        }
        db.allEntries = foundAssets;
        
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"[Astrarium] Database updated! Found {foundAssets.Count} entries.");
    }
}
#endif