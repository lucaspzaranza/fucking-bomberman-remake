using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    #region Vars
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

        bool hitSome = HitSomething(currentDirection);

        if (!hitSome && !HitBomb(currentDirection))
        {
            rb.velocity = new Vector2(speed * horizontal, speed * vertical);
            NormalizeOrtogonalAxis(currentDirection);
        }
        else // Do the wall slope here
            rb.velocity = Vector2.zero;

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
        float deadzoneX = 0.2f; // 0.2f
        float deadzoneY = 0.3f; // 0.3f
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

    private bool HitSomething(Direction currentDirection)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(blockLayerMask);
        wallHitColliders[(int)currentDirection].OverlapCollider(contactFilter, results);
        return results[0] != null;
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
