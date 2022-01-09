using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHashes
{
    public readonly int XSpeed = Animator.StringToHash("XSpeed");
    public readonly int YSpeed = Animator.StringToHash("YSpeed");
}

public static class SharedData
{
    private const string cloneString = "(Clone)";
    private const double ceilDeadzone = 0.45f;
    public static List<Direction> AllDirections => new List<Direction>
    {
        Direction.Up,
        Direction.Down,
        Direction.Left,
        Direction.Right
    };

    public static Vector2 GetNextTile(Vector2 pos, Direction direction, int tileDistance = 1)
    {
        float x = Mathf.RoundToInt(pos.x);
        float y = Mathf.RoundToInt(pos.y);

        if ((int)direction < 2) // Up and down
            y += (direction == Direction.Up) ? tileDistance : -tileDistance;
        else // Left and right
            x += (direction == Direction.Right) ? tileDistance : -tileDistance;

        return new Vector2(x, y);
    }

    public static Vector2 GetNextCornerPosition(Vector2 pos, Vector2 blockPos, Direction direction)
    {
        int roundedX = ((pos.x - blockPos.x) < 0) ? Mathf.FloorToInt(pos.x) : Mathf.CeilToInt(pos.x);
        int roundedY = ((pos.y - blockPos.y) < 0) ? Mathf.FloorToInt(pos.y) : Mathf.CeilToInt(pos.y);
        float finalX, finalY;

        finalX = ((int)direction < 2) ? roundedX : pos.x;
        finalY = ((int)direction >= 2) ? roundedY : pos.y;

        return new Vector2(finalX, finalY);
    }

    public static void RenameObjectWithDirection(GameObject obj, Direction direction)
    {
        string directionString = $" {direction}";

        if (obj.name.Contains(cloneString))
            obj.name = obj.name.Replace(cloneString, directionString);
        else
            obj.name += directionString;
    }
}