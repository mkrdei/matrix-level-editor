using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public GameObject tilePrefab;
    public int levelNumber = 1;
    public GameType gameType;

    void Start()
    {
        // Load the JSON file as a TextAsset from the Resources folder
        TextAsset jsonText = Resources.Load<TextAsset>("Levels/level " + levelNumber);

        // Parse the JSON data into a dictionary
        Dictionary<string, object> levelData = MiniJSON.Json.Deserialize(jsonText.text) as Dictionary<string, object>;

        // Get the width and height values from the level data
        int width = (int)(long)levelData["width"];
        int height = (int)(long)levelData["height"];

        // Calculate the middle point of the matrix
        int midX = width / 2;
        int midY = height / 2;

        // Get the matrix data list from the level data
        List<object> matrixDataList = levelData["matrixDataList"] as List<object>;

        // Loop through the matrix data list to generate the tile objects
        foreach (Dictionary<string, object> matrixData in matrixDataList)
        {
            // Get the tiles data from the matrix data
            List<object> tilesData = matrixData["tiles"] as List<object>;

            // Loop through the tiles data to generate the tile objects
            foreach (Dictionary<string, object> tileData in tilesData)
            {
                // Get the x and y values of the tile
                int x = (int)(long)tileData["x"];
                int y = (int)(long)tileData["y"];


                Vector3 tilePos = new Vector3(x, 0, y);
                if (gameType == GameType.Pyramid)
                {
                    // Shift tile positions for a pyramid look.
                    tilePos = TilePositionShifter.ShiftPosition(tilePos, midX, midY, 0.5f);
                }
                // Create the tile game object
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = tilePos;
            }
        }
    }
}
