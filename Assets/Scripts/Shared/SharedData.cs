using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorHashes
{
    public readonly int XSpeed = Animator.StringToHash("XSpeed");
    public readonly int YSpeed = Animator.StringToHash("YSpeed");
    public readonly int Die = Animator.StringToHash("Death");
    public readonly int ResetAnimator = Animator.StringToHash("ResetAnimator");
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
        float x = 0f, y = 0f;

        if((int)direction < 2) // Up and Down, Y Axis
        {
            y = blockPos.y;
            x = (pos.x < blockPos.x) ? blockPos.x - 1 : blockPos.x + 1;
        }
        else // Left and right, X Axis
        {
            x = blockPos.x;
            y = (pos.y < blockPos.y) ? blockPos.y - 1 : blockPos.y + 1;
        }
        
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

