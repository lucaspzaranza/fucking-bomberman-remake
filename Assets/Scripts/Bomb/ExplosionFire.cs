using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFire : MonoBehaviour
{
    // offset 0.52, -0.37
    public int force;
    public bool isBase;
    public LayerMask contactLayer;
    public Direction direction;
    public BoxCollider2D boxCollider;
    private float offsetValue = 0.1f;

    public void SelfDestroy() 
    {
        Destroy(gameObject);
    }

    private Vector2 GetHitTilePos(Direction currentDirection)
    {
        Vector2 offsetPos = transform.position;
        int dirInt = (int)currentDirection;

        // Positive to Up and Right directions (values 0 and 3), negative to Left and Down directions (values 1 and 2)
        float offset = (dirInt % 3 == 0) ? offsetValue : -offsetValue;

        if((int)currentDirection < 2) // Up and down positions, Y Axis verification
            offsetPos = new Vector2(offsetPos.x, offsetPos.y + offset);
        else // Left and right positions, X Axis verification
            offsetPos = new Vector2(offsetPos.x + offset, offsetPos.y);

        return SharedData.GetNextTile(offsetPos, currentDirection, force);
    }
    
    private Vector2 GetBaseExplosionHitTilePos(Direction currentDirection)
    {
        int x = Mathf.RoundToInt(transform.position.x);

        if (currentDirection == Direction.Left)
            x--;
        else if (currentDirection == Direction.Right)
            x++;

        int y = Mathf.RoundToInt(transform.position.y);

        if (currentDirection == Direction.Up)
            y++;
        else if (currentDirection == Direction.Down)
            y--;

        Vector2 coordinates = new Vector2(x, y);

        return coordinates;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Brick")
        {
            if(!isBase)
            {
                Vector2 coords = GetHitTilePos(direction);
                Brick.instance.DestroyTile(coords);
            }
            else
            {
                foreach (var dir in SharedData.AllDirections)
                {
                    Vector2 coords = GetBaseExplosionHitTilePos(dir);
                    if (Brick.instance.HasBrickTile(coords))
                        Brick.instance.DestroyTile(coords);
                }
            }
        }
    }
}
