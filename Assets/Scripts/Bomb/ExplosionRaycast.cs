using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRaycast : MonoBehaviour
{
    public Bomb bomb;
    public LayerMask collisionLayers;
    public Direction[] directions = null;
    [SerializeField] private Transform[] raycastTransforms;
    //[SerializeField] private Collider2D raycastCollider;

    private Dictionary<Direction, Vector2> DirectionsDictionary = new Dictionary<Direction, Vector2>()
    {
        { Direction.Up, Vector2.up },
        { Direction.Down, Vector2.down },
        { Direction.Left, Vector2.left },
        { Direction.Right, Vector2.right },
    };

    /// <summary>
    /// Calculates the explosion raycast of the 4 directions.
    /// </summary>
    public void CalculateRaycastAll(out int[] distances)
    {
        distances = new int[raycastTransforms.Length];

        foreach (Direction direction in directions)
        {
            int newDistance = bomb.PlayerData.BombForce; // default value
            Vector2 rayPos = raycastTransforms[(int)direction].transform.position;
            var raycast = Physics2D.Raycast(rayPos, DirectionsDictionary[direction], bomb.PlayerData.BombForce, collisionLayers);

            if (raycast.collider != null)
                newDistance = Mathf.RoundToInt(raycast.distance);
            distances[(int)direction] = newDistance;
        }
    }

    /// <summary>
    /// Calculates a single explosion raycast given a certain direction.
    /// </summary>
    /// <param name="direction"></param>
    public int CalculateRaycast(Direction direction)
    {
        int distance = bomb.PlayerData.BombForce; // default value
        Vector2 rayPos = raycastTransforms[(int)direction].transform.position;
        var raycast = Physics2D.Raycast(rayPos, DirectionsDictionary[direction], bomb.PlayerData.BombForce, collisionLayers);

        if (raycast.collider != null)
            distance = Mathf.RoundToInt(raycast.distance);

        return distance;
    }
}
