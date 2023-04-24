using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Tilemaps;
using System.Linq;
using Codice.Client.BaseCommands.Fileinfo;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class LevelEditor : EditorWindow
{
    private int matrixCount = 2;
    public int levelNumber = 1;
    public int width = 5;
    public int height = 5;
    public int tileSize = 64;
    private TileType selectedType = TileType.Empty;

    public Dictionary<Vector2Int, string> tileData = new Dictionary<Vector2Int, string>();
    private Dictionary<string, Texture2D> tileTextures = new Dictionary<string, Texture2D>();
    FirebaseFirestore db;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
    }

    private void OnEnable()
    {
        db = FirebaseFirestore.DefaultInstance;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tileData[new Vector2Int(x, y)] = TileType.Empty.ToString();
            }
        }
        // Load textures for each tile type
        foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
        {
            string texturePath = $"Assets/Art/EditorImages/{type.ToString()}.png";
            if (File.Exists(texturePath))
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                tileTextures[type.ToString()] = texture;
            }
        }
    }

    
    private List<Dictionary<Vector2Int, string>> tileDataList = new List<Dictionary<Vector2Int, string>>();

    private void OnGUI()
    {

        // Options for setting level size and tile size
        GUILayout.Label("Level Settings", EditorStyles.boldLabel);
        levelNumber = EditorGUILayout.IntField("Level Number", levelNumber);
        int editorWidth = EditorGUILayout.IntField("Width", width);
        int editorHeight = EditorGUILayout.IntField("Height", height);
        if (editorWidth != width || editorHeight != height)
        {
            tileDataList = new List<Dictionary<Vector2Int, string>>();
            ClearTiles();
        }
        width = editorWidth;
        height = editorHeight;
        tileSize = EditorGUILayout.IntField("Tile Size", tileSize);

        // Dropdown menu for selecting current tile type
        GUILayout.Label("Tile Types", EditorStyles.boldLabel);
        selectedType = (TileType)EditorGUILayout.EnumPopup("Current Type", selectedType);

        // Buttons for each tile
        GUILayout.Label("Level Design", EditorStyles.boldLabel);
        // Matrix Count field
        matrixCount = EditorGUILayout.IntField("Matrix Count", matrixCount);
        GUILayout.BeginHorizontal();
        for (int matrixIndex = 0; matrixIndex < matrixCount; matrixIndex++)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Matrix " + (matrixIndex + 1), EditorStyles.boldLabel);

            // Initialize tileData for the current matrix
            if (tileDataList.Count <= matrixIndex)
            {
                tileDataList.Add(new Dictionary<Vector2Int, string>());
            }
            Dictionary<Vector2Int, string> tileData = tileDataList[matrixIndex];

            for (int y = 0; y < height; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < width; x++)
                {
                    Vector2Int key = new Vector2Int(x + matrixIndex * width, y);
                    string currentType = TileType.Empty.ToString();
                    if (tileData.ContainsKey(key))
                    {
                        currentType = tileData[key].ToString();
                    }

                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                    buttonStyle.fixedWidth = tileSize;
                    buttonStyle.fixedHeight = tileSize;

                    GUIContent buttonContent = new GUIContent(GetTileTexture(currentType), "");

                    if (GUILayout.Button(buttonContent, buttonStyle))
                    {
                        tileData[key] = selectedType.ToString();
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        // Save, Load and Empty buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            SaveLevel();
        }
        if (GUILayout.Button("Load"))
        {
            LoadLevel();
        }
        if (GUILayout.Button("Empty"))
        {
            // Set type of all tiles to empty
            ClearTiles();
        }
        GUILayout.EndHorizontal();

        
    }

    private void ClearTiles()
    {
        // Clear tile data for all matrices
        foreach (Dictionary<Vector2Int, string> matrix in tileDataList)
        {
            foreach (Vector2Int key in matrix.Keys.ToList())
            {
                matrix[key] = TileType.Empty.ToString();
            }
        }
    }

    private Texture2D GetTileTexture(string type)
    {
        if (tileTextures.ContainsKey(type))
        {
            return tileTextures[type];
        }
        return null;
    }

    private void SaveLevel()
    {
        db = FirebaseFirestore.DefaultInstance;
        List<MatrixData> matrixDataList = new List<MatrixData>();
        for (int matrixIndex = 0; matrixIndex < matrixCount; matrixIndex++)
        {
            List<TileData> dataList = new List<TileData>();
            foreach (KeyValuePair<Vector2Int, string> pair in tileDataList[matrixIndex])
            {
                TileData data = new TileData(pair.Key, pair.Value);
                if (pair.Value != "Empty")
                    dataList.Add(data);
            }
            MatrixData matrixData = new MatrixData(matrixIndex, dataList);
            matrixDataList.Add(matrixData);
        }
        
        LevelData levelData = new LevelData(width, height, tileSize, matrixDataList, tileDataList);
        string jsonData = JsonUtility.ToJson(levelData, true);
        
        Dictionary<string, string> levelDic = new Dictionary<string, string>
        {
            { "Level " + levelNumber, jsonData }
        };
        db.Collection("CardinalChains").Document("Levels").SetAsync(levelDic);

        Debug.Log("Level saved");
    }
    private void LoadLevel()
    {
        DocumentReference docRef = db.Collection("CardinalChains").Document("Levels");
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> levelDic = snapshot.ToDictionary();
                string jsonData = (string)levelDic["Level " + levelNumber];
                LevelData levelData = JsonUtility.FromJson<LevelData>(jsonData);

                // Apply level settings
                width = levelData.width;
                height = levelData.height;
                tileSize = levelData.tileSize;
                matrixCount = levelData.matrixDataList.Count;

                // Apply tile data
                tileDataList.Clear();
                foreach (MatrixData matrixData in levelData.matrixDataList)
                {
                    Dictionary<Vector2Int, string> matrixTileData = new Dictionary<Vector2Int, string>();
                    foreach (TileData tileData in matrixData.tiles)
                    {
                        matrixTileData[tileData.pos] = tileData.type;
                    }
                    tileDataList.Add(matrixTileData);
                }

                Repaint();
                Debug.Log("Level loaded");
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
    }



}
