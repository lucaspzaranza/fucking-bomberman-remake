using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFire : MonoBehaviour
{
    private const float offsetValue = 0.1f;
    private const string brickTag = "Brick";

    public ExplosionFireType explosionType;
    public int force;
    public LayerMask contactLayer;
    public Direction direction;
    public BoxCollider2D boxCollider;

    public void SelfDestroy() 
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        CollisionDetection();
    }

    private void CollisionDetection()
    {
        if (explosionType == ExplosionFireType.Base)
            BaseFireCollision();
        else if (explosionType == ExplosionFireType.Body)
            BodyFireCollision();
        else if (explosionType == ExplosionFireType.Beak)
            BeakFireCollision();
    }

    private void BaseFireCollision()
    {
        Vector2 tilePos = transform.position;
        Collider2D hit = null;

        hit = Physics2D.OverlapCircle(tilePos, offsetValue, contactLayer); // Overlap only on its own tile
        if (hit != null)
            CallExplosionHit(hit, tilePos);

        foreach (var dir in SharedData.AllDirections) // Overlap on the tiles around him
        {
            tilePos = GetBaseHitTilePos(dir);
            hit = Physics2D.OverlapCircle(tilePos, offsetValue, contactLayer);

            if (hit == null) continue;

            CallExplosionHit(hit, tilePos);
        }
    }

    private void BodyFireCollision()
    {
        Vector2 tilePos = transform.position;
        Collider2D hit = null;

        hit = Physics2D.OverlapCircle(tilePos, offsetValue, contactLayer); // Overlap on its own tile
        if (hit != null)
            CallExplosionHit(hit, tilePos);

        tilePos = GetBodyHitTilePos(direction); // Overlap on the tile in front of him
        hit = Physics2D.OverlapCircle(tilePos, offsetValue, contactLayer);
        if (hit != null)
            CallExplosionHit(hit, tilePos);
    }

    private void BeakFireCollision()
    {
        Vector2 tilePos = transform.position;
        Collider2D hit = null;

        hit = Physics2D.OverlapCircle(tilePos, offsetValue, contactLayer); // Overlap on its own tile
        if (hit != null)
            CallExplosionHit(hit, tilePos);
    }

    /// <summary>
    /// Used to detect collision between the explosion body and another tile in front of it.
    /// </summary>
    /// <param name="currentDirection">The direction of the body.</param>
    /// <returns>The tile position.</returns>
    private Vector2 GetBodyHitTilePos(Direction currentDirection)
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

    /// <summary>
    /// Used to detect collision between the explosion base and another tile in front of it or around it.
    /// </summary>
    /// <param name="currentDirection">The direction of the body.</param>
    /// <returns>The tile position.</returns>
    private Vector2 GetBaseHitTilePos(Direction currentDirection)
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

    private void CallExplosionHit(Collider2D hit, Vector2 tilePos)
    {
        if (hit.tag == brickTag)
            hit.GetComponent<Destructible>().ExplosionHit(tilePos);
        else
            hit.GetComponent<Destructible>()?.ExplosionHit();
    }
}
