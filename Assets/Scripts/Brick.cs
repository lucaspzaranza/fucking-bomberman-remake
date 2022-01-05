using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class Brick : MonoBehaviour
{
    public static Brick instance;

    public AnimatedTile explosionTile;
    public Tilemap brickTilemap;
    public TilemapCollider2D tilemapCollider;

    public Vector2 coordinates;
    public float timeToDestroyBrick;

    private ContactFilter2D contactFilter;
    private List<Collider2D> colliders = new List<Collider2D>();

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
        brickTilemap.SetTile(currentCell, explosionTile);
        StartCoroutine(SetTileToNull(currentCell));
    }

    public bool HasBrickTile(Vector2 coords)
    {
        Vector3Int currentCell = brickTilemap.WorldToCell(coords);
        return brickTilemap.HasTile(currentCell);
    }

    private IEnumerator SetTileToNull(Vector3Int cell)
    {
        yield return new WaitForSeconds(timeToDestroyBrick);
        brickTilemap.SetTile(cell, null);
    }
}
