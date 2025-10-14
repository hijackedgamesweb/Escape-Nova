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
                "Assets/Code/Scripts/Core/Managers",
                "Assets/Code/Scripts/Core/Utilities",
                "Assets/Code/Scripts/Core/Events",
                "Assets/Code/Scripts/Core/SaveLoad",
                "Assets/Code/Scripts/Core/SceneManagement",

                "Assets/Code/Scripts/Systems",
                "Assets/Code/Scripts/Patterns",
                
                "Assets/Code/Scripts/UI/Windows",
                "Assets/Code/Scripts/UI/HUD",
                "Assets/Code/Scripts/UI/Menus",
                "Assets/Code/Scripts/UI/Common",

                "Assets/Code/Scripts/Player",
                "Assets/Code/Scripts/Camera",
                "Assets/Code/Scripts/Input",
                
                "Assets/Code/Shaders",
                "Assets/Code/ScriptableObjects",
                
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
