using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ProjectBaseStructure : MonoBehaviour
    {
        [MenuItem("Tools/Create Project Structure")]
        public static void CreateFolders()
        {
            string[] folders = new string[]
            {
                "Assets/Scripts/Code/Core/Managers",
                "Assets/Scripts/Code/Core/Utilities",
                "Assets/Scripts/Code/Core/Events",
                "Assets/Scripts/Code/Core/SaveLoad",
                "Assets/Scripts/Code/Core/SceneManagement",

                "Assets/Scripts/Code/Systems",
                "Assets/Scripts/Code/Patterns",
                
                "Assets/Scripts/Code/UI/Windows",
                "Assets/Scripts/Code/UI/HUD",
                "Assets/Scripts/Code/UI/Menus",
                "Assets/Scripts/Code/UI/Common",

                "Assets/Scripts/Code/Player",
                "Assets/Scripts/Code/Camera",
                "Assets/Scripts/Code/Input",
                
                "Assets/Scripts/Shaders",
                "Assets/Scripts/ScriptableObjects",
                
                "Assets/Level/Prefabs",
                "Assets/Level/Scenes",
                "Assets/Level/ScriptableObjects",
                "Assets/Art/Models",
                "Assets/Art/Sprites",
                "Assets/Art/Textures",
                "Assets/Art/Animations",
                "Assets/Art/Materials",
                "Assets/Audio/Music",
                "Assets/Audio/SFX",
                "Assets/UI/Sprites",
                "Assets/UI/Fonts",
                "Assets/UI/Prefabs",
                "Assets/Resources",
                "Assets/Plugins"
            };

            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    Debug.Log("Created: " + folder);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Project structure created successfully!");
        }
    }
}
