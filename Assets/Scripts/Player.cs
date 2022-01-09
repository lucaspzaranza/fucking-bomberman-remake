using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class Player : Destructible
{
    #region Vars

    private const float cornerMoveSpeed = 5f;
    private const float cornerMoveOffset = 0.5f;

    [SerializeField] private float cornerDeadzone = 0.3f;
    [SerializeField] private float cornerOffset = 0.05f;
    [SerializeField] private float stopSmoothCornerDeadzone = 0.1f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask blockLayerMask;
    [SerializeField] private LayerMask bombLayerMask;
    [SerializeField] private Collider2D[] wallHitColliders = null;
    [SerializeField] private Collider2D[] bombHitColliders = null;
    [SerializeField] private GameObject bombPrefab;

    private AnimatorHashes animHashes = new AnimatorHashes();
    private Direction currentDirection = Direction.Down;
    private Direction previousDirection = 0;
    private bool isMovingCorner = false;
    private GameObject ironBlock = null;
    private Vector2 cornerTarget;

    private BombermanInput input;
    private InputAction movement;
    private InputAction bomb;

    #endregion

    #region Props

    [SerializeField] private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

    public Vector2 RoundedPlayerPosition =>
        new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

    #endregion

    private void Awake()
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
        if (isMovingCorner)
            SmoothCornerMovement(ironBlock);
        else
            Move();
    }

    private void PutBomb(CallbackContext context)
    {
        if (PlayerData.BombCount > 0)
        {
            Vector2 pos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            var newBomb = Instantiate(bombPrefab, pos, Quaternion.identity);
            Bomb bombScript = newBomb.GetComponent<Bomb>();
            bombScript.SetPlayerData(PlayerData);
            bombScript.PlayerData.DecrementBombCount();
        }
    }

    private void Move()
    {
        Vector2 moveVector = movement.ReadValue<Vector2>();

        float horizontal = GetNormalizedMovementValue(moveVector.x);
        float vertical = horizontal == 0 ? GetNormalizedMovementValue(moveVector.y) : 0f;
        float speed = PlayerData.Speed;
        Vector2Int axisValues = new Vector2Int((int)horizontal, (int)vertical);
        if (axisValues != Vector2Int.zero)
        {
            previousDirection = currentDirection;
            SetCollidersActivation((int)previousDirection, false);

            currentDirection = GetCurrentDirection(axisValues);
            SetCollidersActivation((int)currentDirection, true);
        }

        Collider2D hit = HitSomething(currentDirection);

        if (!hit && !HitBomb(currentDirection))
        {
            rb.velocity = new Vector2(speed * horizontal, speed * vertical);
            NormalizeOrtogonalAxis(currentDirection);
        }
        else if(!isMovingCorner) // Do the corner slope here
        {           
            CornerSlopeDetection(hit);
            if(!isMovingCorner)
                rb.velocity = Vector2.zero;
        }

        SetMovementAnimators(axisValues);
    }

    private void SetCollidersActivation(int index, bool value)
    {
        wallHitColliders[index].gameObject.SetActive(value);
        bombHitColliders[index].gameObject.SetActive(value);
    }

    private void NormalizeOrtogonalAxis(Direction currentDir)
    {
        int dir = (int)currentDir;
        int rounded = 0;
        float deadzoneX = 0.3f; // 0.3f
        float deadzoneY = 0.4f; // 0.4f
        if (dir - 2 < 0) // Y Axis, so normalize X Axis
        {
            rounded = Mathf.RoundToInt(transform.position.x);
            if (Mathf.Abs(transform.position.x - rounded) <= deadzoneX)
                transform.position = new Vector2(rounded, transform.position.y);
        }
        else // X Axis, so normalize Y Axis
        {
            rounded = Mathf.RoundToInt(transform.position.y);
            if (Mathf.Abs(transform.position.y - rounded) <= deadzoneY)
                transform.position = new Vector2(transform.position.x, rounded);
        }
    }

    private void SmoothCornerMovement(GameObject hitObj)
    {
        if (Mathf.Abs(Vector2.Distance(transform.position, cornerTarget)) > stopSmoothCornerDeadzone)
            transform.position = Vector2.MoveTowards(transform.position, cornerTarget, (PlayerData.Speed + cornerMoveOffset) * Time.smoothDeltaTime);
        else 
            isMovingCorner = false;
    }

    private void GetCornerDestination(GameObject hitObj)
    {
        float offset = 0f;
        Vector2 nextTile = SharedData.GetNextCornerPosition(transform.position, hitObj.transform.position, currentDirection);

        // Negative to down/left, positive to up/right directions
        offset = ((int)currentDirection % 3 != 0)? -cornerOffset : cornerOffset;

        if ((int)currentDirection < 2) // Up/Down
            cornerTarget = new Vector2(nextTile.x, transform.position.y + offset);
        else // Left/Right
            cornerTarget = new Vector2(transform.position.x + offset, nextTile.y);
    }

    private void CornerSlopeDetection(Collider2D hit)
    {
        float distance = 0f;
        if((int)currentDirection < 2) // Up/Down, check X Axis
        {
            distance = transform.position.x - Mathf.RoundToInt(transform.position.x);
            MovingCornerSetup(hit, distance);
        }
        else // Left/Right, check Y Axis
        {
            distance = transform.position.y - Mathf.RoundToInt(transform.position.y);            
            MovingCornerSetup(hit, distance);
        }
    }

    private void MovingCornerSetup(Collider2D hit, float distance)
    {
        isMovingCorner = (Mathf.Abs(distance) >= cornerDeadzone);
        if (isMovingCorner)
        {
            ironBlock = hit.gameObject;
            GetCornerDestination(hit.gameObject);
        }
    }

    private Direction GetCurrentDirection(Vector2Int axisValues)
    {
        if (axisValues.x != 0)
            return (axisValues.x > 0) ? Direction.Right : Direction.Left;
        else if (axisValues.y != 0)
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

    private Collider2D HitSomething(Direction currentDirection)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(blockLayerMask);
        wallHitColliders[(int)currentDirection].OverlapCollider(contactFilter, results);
        return results[0];
    }

    private bool HitBomb(Direction currentDirection)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(bombLayerMask);
        bombHitColliders[(int)currentDirection].OverlapCollider(contactFilter, results);
        return results[0] != null;
    }
}
