using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHashes 
{
    public readonly int X = Animator.StringToHash("X");
    public readonly int Y = Animator.StringToHash("Y");
    public readonly int Die = Animator.StringToHash("Death");
    public readonly int ResetAnimator = Animator.StringToHash("ResetAnimator");
    public readonly int IdleUpset = Animator.StringToHash("IdleUpset");
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

    public static List<Vector2> DirectionVectors => new List<Vector2>
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
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

