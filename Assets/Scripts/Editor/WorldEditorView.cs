using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class WorldEditorView : EditorWindow
{
    private WorldEditorViewModel viewModel;
    private Editor gameObjectEditor;

    private string worldName;
    private int meshSizeX = 100;
    private int meshSizeZ = 100;
    private int octaves = 3;
    private float heightMultiplier;
    private float elevationMultiplier;
    private Gradient gradient = new Gradient();
    private GameObject worldObject;

    private Mesh mesh;
    private GameObject preview;

    [MenuItem("Tools/World Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow(typeof(WorldEditorView));
        window.minSize = new Vector2(500,600);
    }

    private void OnEnable()
    {
        viewModel = new WorldEditorViewModel();
    }


    private void OnGUI()
    {
        GUILayout.Label("Set World Parameters", EditorStyles.boldLabel);

        worldName = EditorGUILayout.TextField("World Name", worldName);
        EditorGUILayout.BeginHorizontal();
        meshSizeX = EditorGUILayout.IntSlider("World Size X", meshSizeX, 100, 250);
        meshSizeZ = EditorGUILayout.IntSlider("World Size Z", meshSizeZ, 100, 250);
        EditorGUILayout.EndHorizontal();
        octaves = EditorGUILayout.IntField("Amount of Noise Layers", octaves);
        heightMultiplier = EditorGUILayout.FloatField("World Height", heightMultiplier);
        elevationMultiplier = EditorGUILayout.FloatField("Amount of Elevations", elevationMultiplier);
        gradient = EditorGUILayout.GradientField("World Color", gradient);
        worldObject = EditorGUILayout.ObjectField("World Object", worldObject, typeof(GameObject), false) as GameObject;

        
        if (CheckInputs())
        {
            //Creates preview window
            if (preview != null)
            {

                if (gameObjectEditor == null)
                {
                    gameObjectEditor = Editor.CreateEditor(preview);
                }

                gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), GUIStyle.none);
            }

            if (GUILayout.Button("Generate Mesh"))
            {
                preview = viewModel.GenerateMesh(worldName, meshSizeX, meshSizeZ, octaves, heightMultiplier, elevationMultiplier, gradient, worldObject);
                gameObjectEditor = null;
            }
            if (GUILayout.Button("Spawn World"))
            {
                viewModel.SpawnWorld();
            }
            if (GUILayout.Button("Save Mesh!"))
            {
                mesh = viewModel.GenerateMesh(worldName, meshSizeX, meshSizeZ, octaves, heightMultiplier, elevationMultiplier, gradient, worldObject).GetComponent<MeshFilter>().sharedMesh;
                AssetDatabase.CreateAsset(mesh, "Assets/Prefabs/" + worldName + ".asset");
            }
            
            
        }

        

    }

    private bool CheckInputs()
    {
        //Check for invalid input
        if (worldName == string.Empty)
        {
            EditorGUILayout.HelpBox("Please enter a name!", MessageType.Warning, true);
            return false;
        }
        if(worldName.Contains(' '))
        {
            EditorGUILayout.HelpBox("Spaces in the world name are not allowed!", MessageType.Warning, true);
            return false;
        }
        if (worldObject == null)
        {
            EditorGUILayout.HelpBox("Please choose an object!", MessageType.Warning, true);
            return false;
        }
        if(worldObject.GetComponent<MeshFilter>() == null)
        {
            EditorGUILayout.HelpBox("The Object requires a MeshFilter component!", MessageType.Warning, true);
            return false;
        }

        return true;
    }
}
