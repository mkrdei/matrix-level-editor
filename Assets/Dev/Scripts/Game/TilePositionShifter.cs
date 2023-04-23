using UnityEngine;

public static class TilePositionShifter
{
    public static Vector3 ShiftPosition(Vector3 position, int midX, int midY, float shiftAmount)
    {
        // Calculate the distance vector between the middle tile and this tile
        Vector2 distanceVec = new Vector2(midX - position.x, midY - position.z);

        // Divide the distance vector by 2 and add it to the position vector
        Vector3 shiftedPos = new Vector3(position.x + distanceVec.x * shiftAmount, position.y, position.z + distanceVec.y * shiftAmount);

        // Calculate the distance between the middle tile and this tile
        int distance = (int)Mathf.Max(Mathf.Abs(midX - position.x), Mathf.Abs(midY - position.z));

        // Set the height of the tile based on the distance
        if (distance > 0)
        {
            shiftedPos.y -= distance;
        }

        return shiftedPos;
    }
}