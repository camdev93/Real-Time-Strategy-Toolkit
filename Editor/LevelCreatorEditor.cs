using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using System.Threading;
using System.Runtime.Hosting;
using System.CodeDom.Compiler;
using System.Collections.Specialized;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    Object _object;
    public override void OnInspectorGUI()
    {
        LevelCreator levelCreator = (LevelCreator)target;

        if (levelCreator.spawnPlayer)
        {
            _object = EditorGUILayout.ObjectField((GameObject)levelCreator.NavmeshPlayerObject, typeof(Object), true);
        }

        base.OnInspectorGUI();

        GUIContent content = new GUIContent();

        GUILayout.BeginHorizontal();

        content.text = "Generate New Level";
        content.tooltip = "This function will consider above parameters and generate a new random level";
        if (GUILayout.Button(content))
        {
            levelCreator.iterations = 0;
            levelCreator.objectOneInitialised = false;
            levelCreator.objectTwoInitialised = false;
            levelCreator.hasPlayerSpawned = false;
            LoadRequiredPrefab(levelCreator);
            levelCreator.CreateLevel();
        }

        content.text = "Feeling Lucky?";
        content.tooltip = "This function will consider above parameters except for values raleted to cover positions and set them as maximum values. Values will be randomised within a close range taking into account the specified maximum value and generate a new random level";
        if (GUILayout.Button(content))
        {
            levelCreator.iterations = 0;
            levelCreator.objectOneInitialised = false;
            levelCreator.objectTwoInitialised = false;
            levelCreator.hasPlayerSpawned = false;
            LoadRequiredPrefab(levelCreator);
            levelCreator.RandomiseParameters();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        content.text = "Save Configuration";
        content.tooltip = "Save level editor configuration in a .json file";
        if (GUILayout.Button(content))
        {
            SaveConfiguration(levelCreator);
        }

        content.text = "Load Configuration";
        content.tooltip = "Load level editor configuration from a .json file";
        if (GUILayout.Button(content))
        {
            LoadConfiguration(levelCreator);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        content.text = "Save As FBX";
        content.tooltip = "Save prefab of generated model in Prefabs project folder.";
        if (GUILayout.Button(content))
        {
            SavePrefab(levelCreator);
        }

        GUILayout.EndHorizontal();
    }

    // CONFIGURATION DATA MANAGEMENT
    public void SaveConfiguration(LevelCreator levelCreator)
    {
        ConfigurationData data = new ConfigurationData();

        data.levelLength = levelCreator.levelLength;
        data.levelWidth = levelCreator.levelWidth;
        data.maxIterations = levelCreator.maximumIterations;
        data.corridorWidth = levelCreator.corridorWidth;
        data.roomBottomCornerModifier = levelCreator.roomBottomCornerModifier;
        data.roomTopCornerModifier = levelCreator.roomTopCornerModifier;
        data.roomOffset = levelCreator.roomOffset;
        data.chanceOfCover = levelCreator.chanceOfCover;

        string jsonString = JsonUtility.ToJson(data);

        string path = EditorUtility.SaveFilePanelInProject("Save configuration data", "Untitled_Configuration", "json", "Pick a suitable file name for your config data", "Assets/BuildingGenerator/Editor/SavedConfigurations");
        
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, jsonString);
        }
    }

    public void LoadConfiguration(LevelCreator levelCreator)
    {
        string path = EditorUtility.OpenFilePanel("Load Level Configuration", "Assets/BuildingGenerator/Editor/SavedConfigurations", "json");

        if (!string.IsNullOrEmpty(path))
        {
            string jsonString = System.IO.File.ReadAllText(path);

            ConfigurationData data = JsonUtility.FromJson<ConfigurationData>(jsonString);

            levelCreator.levelLength = data.levelLength;
            levelCreator.levelWidth = data.levelWidth;
            levelCreator.maximumIterations = data.maxIterations;
            levelCreator.corridorWidth = data.corridorWidth;
            levelCreator.roomBottomCornerModifier = data.roomBottomCornerModifier;
            levelCreator.roomTopCornerModifier = data.roomTopCornerModifier;
            levelCreator.roomOffset = data.roomOffset;
            levelCreator.chanceOfCover = data.chanceOfCover;
        }
    }

    // PREFAB DATA MANAGEMENT
    public void SavePrefab(LevelCreator levelCreator)
    {
        // This ADD() function is called to check if user has FBX Exporter installed. if not it will install the package automatically.
        AddPackage.Add();

        string path = EditorUtility.SaveFilePanelInProject("Save level as a prefab", "Untitled_Level", "Prefab", "select a valid name for your prefab", "Assets/BuildingGenerator/Prefabs/SavedLevelPrefabs");

        if (!string.IsNullOrEmpty(path))
        {
            ModelExporter.ExportObject(path, levelCreator.gameObject);
        }
    }

    // use this function to load required objects from project folders
    public void LoadRequiredPrefab(LevelCreator levelCreator)
    {
        GameObject emptyFloor = Resources.Load("EMPTY_FLOOR")as GameObject;
        levelCreator.emptyParentFloor = emptyFloor;
    }
}