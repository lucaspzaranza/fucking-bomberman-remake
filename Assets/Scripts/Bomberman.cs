using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class AnimatorHashes
{
    public readonly int XSpeed = Animator.StringToHash("XSpeed");
    public readonly int YSpeed = Animator.StringToHash("YSpeed");
}

public enum Direction
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
}

public class Bomberman : MonoBehaviour
{
    public float timeToDestroyBrick;

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private Collider2D[] wallhitColliders = null;

    private AnimatorHashes animHashes = new AnimatorHashes();
    private Direction currentDirection = (Direction)1;
    private Direction previousDirection;

    private BombermanInput input;
    private InputAction movement;
    private InputAction bomb;

    public AnimatedTile explosionTile;
    public Tilemap terrainMap;

    private bool isLeft;

    void Awake()
    {
        input = new BombermanInput();
    }

    private void OnEnable()
    {
        movement = input.PlayerControls.WASDMoves;
        movement.Enable();

        bomb = input.PlayerControls.Bomb;
        bomb.performed += PutBomb;
        bomb.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
    }
   

    void Update()
    {
        Move();
    }

    private void PutBomb(CallbackContext context)
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

        Vector3Int currentCell = terrainMap.WorldToCell(coordinates);

        terrainMap.SetTile(currentCell, explosionTile);
        StartCoroutine(DestroyTile(currentCell));
    }

    private IEnumerator DestroyTile(Vector3Int cell)
    {
        yield return new WaitForSeconds(timeToDestroyBrick);
        terrainMap.SetTile(cell, null);
    }

    private void Move()
    {
        Vector2 moveVector = movement.ReadValue<Vector2>();

        float horizontal = GetNormalizedMovementValue(moveVector.x);
        float vertical = horizontal == 0? GetNormalizedMovementValue(moveVector.y) : 0f;
        Vector2Int axisValues = new Vector2Int((int)horizontal, (int)vertical);
        if(axisValues != Vector2Int.zero)
        {
            previousDirection = currentDirection;
            wallhitColliders[(int)previousDirection].gameObject.SetActive(false);

            currentDirection = GetCurrentDirection(axisValues);
            wallhitColliders[(int)currentDirection].gameObject.SetActive(true);
        }

        if (!HitWall(currentDirection))
        {
            rb.velocity = new Vector2(speed * horizontal, speed * vertical);
            NormalizeOrtogonalAxis(currentDirection);
        }

        SetMovementAnimators(axisValues);
    }
    
    private void NormalizeOrtogonalAxis(Direction currentDir)
    {
        int dir = (int)currentDir;
        int rounded = 0;
        if(dir - 2 < 0) // Y Axis, so normalize X Axis
        {
            rounded = Mathf.RoundToInt(transform.position.x);
            transform.position = new Vector2(rounded, transform.position.y);
        }
        else // X Axis, so normalize Y Axis
        {
            rounded = Mathf.RoundToInt(transform.position.y);
            transform.position = new Vector2(transform.position.x, rounded);
        }
    }

    private Direction GetCurrentDirection(Vector2Int axisValues)
    {
        if(axisValues.x != 0)
            return (axisValues.x > 0) ? Direction.Right : Direction.Left;
        else if(axisValues.y != 0)
            return (axisValues.y > 0) ? Direction.Up : Direction.Down;

        return Direction.Down;
    }

    private void SetMovementAnimators(Vector2Int axisValues)
    {
        anim.SetInteger(animHashes.XSpeed, axisValues.x);
        anim.SetInteger(animHashes.YSpeed, axisValues.y);
    }

    private float GetNormalizedMovementValue(float raw)
    {
        if (raw > 0) return 1f;
        else if (raw < 0) return -1f;

        return 0f;
    }

    private bool HitWall(Direction currentDirection)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.SetLayerMask(terrainLayerMask);
        wallhitColliders[(int)currentDirection].OverlapCollider(contactFilter, results);
        return results[0] != null;
    }
}
