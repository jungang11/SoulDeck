using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateFolders : EditorWindow
{
    private static string projectName = "PROJECT_NAME";

    [MenuItem("Assets/Create Default Folders")]
    private static void SetUpFolders()
    {
        CreateFolders window = GetWindow<CreateFolders>("Create Project Folders");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 250);
        window.ShowPopup();
    }

    private static void CreateAllFolders()
    {
        List<string> folders = new()
        {
            "Animations",
            "Audio",
            "Editor",
            "Materials",
            "Meshes",
            "Prefabs",
            "Scripts",
            "Scenes",
            "Shaders",
            "Textures",
            "UI",
            "ThirdParty",
        };

        foreach (string folder in folders)
        {
            if(!Directory.Exists("Assets/" + folder))
            {
                Directory.CreateDirectory("Assets/" + projectName + "/" + folder);
            }
        }
        
        List<string> uiFolders = new()
        {
            "Assets",
            "Fonts",
            "Icon"
        };

        foreach (string subfolder in uiFolders)
        {
            if(!Directory.Exists("Assets/" + projectName + "/UI/" + subfolder))
            {
                Directory.CreateDirectory("Assets/" + projectName + "/UI/" + subfolder);
            }
        }

        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Insert the Project Name used as the root folder");
        projectName = EditorGUILayout.TextField("Project Name: ", projectName);
        this.Repaint();
        GUILayout.Space(70);
        if(GUILayout.Button("Generate!"))
        {
            CreateAllFolders();
            this.Close();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
