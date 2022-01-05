using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFire : MonoBehaviour
{
    public LayerMask contactLayer;
    public bool isBase;
    public Direction direction;
    public Bomb bomb;

    public void SelfDestroy() 
    {
        Destroy(gameObject);
    }

    private Vector2 GetHitTilePos(Direction currentDirection)
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
                foreach (var dir in bomb.ExplosionRaycast.directions)
                {
                    Vector2 coords = GetHitTilePos(dir);
                    if (Brick.instance.HasBrickTile(coords))
                        Brick.instance.DestroyTile(coords);
                }
            }
        }
    }
}
