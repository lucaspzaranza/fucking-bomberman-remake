using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Brick : Destructible
{
    public static Brick instance;

    public AnimatedTile explosionTile;
    public Tilemap brickTilemap;
    public TilemapCollider2D tilemapCollider;

    public Vector2 coordinates;
    public GameObject brickExplosion;
    public float timeToDestroyBrick;

    private ContactFilter2D contactFilter;
    private List<Collider2D> colliders = new List<Collider2D>();

    public delegate void BrickDestroyed(Vector2Int brickPosition);
    public static event BrickDestroyed OnBrickDestroyed;       

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    public void DestroyTile(Vector2 brickPos)
    {
        Vector3Int currentCell = brickTilemap.WorldToCell(brickPos);
        brickTilemap.SetTile(currentCell, null);
        Instantiate(brickExplosion, brickPos, Quaternion.identity); 
        OnBrickDestroyed?.Invoke(Vector2Int.RoundToInt(brickPos));
    }

    public bool HasBrickTile(Vector2 coords)
    {
        Vector3Int currentCell = brickTilemap.WorldToCell(coords);
        return brickTilemap.HasTile(currentCell);
    }

    public override void ExplosionHit(Vector2 tilePos)
    {
        if (instance.HasBrickTile(tilePos))
            instance.DestroyTile(tilePos);
    }
}
