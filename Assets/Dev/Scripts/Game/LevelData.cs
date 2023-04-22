using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int width;
    public int height;
    public int tileSize;
    public List<MatrixData> matrixDataList;
    public List<Dictionary<Vector2Int, string>> tileDataList;

    public LevelData(int width, int height, int tileSize, List<MatrixData> matrixDataList, List<Dictionary<Vector2Int, string>> tileDataList)
    {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        this.matrixDataList = matrixDataList;
        this.tileDataList = tileDataList;
    }
}

[System.Serializable]
public class MatrixData
{
    public int matrixIndex;
    public List<TileData> tiles;

    public MatrixData(int matrixIndex, List<TileData> tiles)
    {
        this.matrixIndex = matrixIndex;
        this.tiles = tiles;
    }
}

[System.Serializable]
public class TileData
{
    public int x;
    public int y;
    public string type;
    public Vector2Int pos;
    public TileData(Vector2Int pos, string type)
    {
        this.pos = pos;
        this.x = pos.x;
        this.y = pos.y;
        this.type = type;
    }
}
