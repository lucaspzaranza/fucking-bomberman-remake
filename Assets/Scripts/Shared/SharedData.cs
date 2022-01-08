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

    public static void RenameObjectWithDirection(GameObject obj, Direction direction)
    {
        string directionString = $" {direction}";

        if (obj.name.Contains(cloneString))
            obj.name = obj.name.Replace(cloneString, directionString);
        else
            obj.name += directionString;
    }
}