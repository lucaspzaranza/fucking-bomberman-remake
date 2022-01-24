using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class Player : Character
{
    #region Vars

    [SerializeField] private LayerMask bombLayerMask;
    [SerializeField] private Collider2D[] bombHitColliders = null;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float idleTime = 10f;

    private float idleTimeCounter = 0f;
    private bool isIdle = false;

    // Input Action
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

        bomb.performed -= PutBomb;
        bomb.Disable();
    }

    private void Start()
    {
        PlayerData.OnPlayerRespawn += HandleOnPlayerRespawn;
        PlayerData.OnGameOver += HandleOnGameOver;
    }

    public override void FixedUpdate()
    {
        if (Health <= 0)
        {
            StopPlayer();
            return;
        }

        base.FixedUpdate();

        if (isMovingCorner)
            SmoothCornerMovement();
        else
            Move();
    }

    public override void IncrementHealth()
    {
        _health = Mathf.Clamp(_health + 1, minHealth, maxHealth);
    }

    public override void DecrementHealth()
    {
        _health = Mathf.Clamp(_health - 1, 0, maxHealth);

        if (_health <= 0)
            PlayerDeath();
        else
            TakeDamage();
    }

    private void StopPlayer() => rb.velocity = Vector2.zero;

    private void TakeDamage()
    {
        isBlinking = true;
    }

    private void HandleOnPlayerRespawn()
    {
        isBlinking = true;
        Invoke(nameof(ResetPlayerData), 0.5f);
    }

    private void HandleOnGameOver()
    {
        Invoke(nameof(DeactivatePlayer), 0.5f);
        UIController.instance.GameOver.SetActive(true);
    }

    private void DeactivatePlayer()
    {
        gameObject.SetActive(false);
    }

    private void PlayerDeath()
    {
        if (isIdle)
        {
            idleTimeCounter = 0f;
            isIdle = false;
            anim.SetBool(animHashes.IdleUpset, false);
        }
        SetDieAnimation(true);
        PlayerData.DecrementLifeCount();
    }

    private void SetDieAnimation(bool val) => anim.SetBool(animHashes.Die, val);
    private void ResetPlayerData()
    {
        SetDieAnimation(false);
        IncrementHealth(); // set health = 1
        transform.position = new Vector2(0, 0);
        anim.SetTrigger(animHashes.ResetAnimator);
    }

    private void PutBomb(CallbackContext context)
    {
        Vector2 roundedPos = new 
            Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (PlayerData.BombCount > 0 && !HasBomb(roundedPos))
        {
            var newBomb = Instantiate(bombPrefab, roundedPos, Quaternion.identity);
            Bomb bombScript = newBomb.GetComponent<Bomb>();
            bombScript.SetPlayerData(PlayerData);
            bombScript.PlayerData.DecrementBombCount();
        }
    }

    private bool HasBomb(Vector2 tilePos)
    {
        return Physics2D.OverlapPoint(tilePos, bombLayerMask);
    }

    private void Move()
    {
        Vector2 moveVector = movement.ReadValue<Vector2>();

        float horizontal = GetNormalizedMovementValue(moveVector.x);
        float vertical = horizontal == 0 ? GetNormalizedMovementValue(moveVector.y) : 0f;
        Vector2Int axisValues = new Vector2Int((int)horizontal, (int)vertical);
        if (axisValues != Vector2Int.zero)
        {
            idleTimeCounter = 0f;
            isIdle = false;
            anim.SetBool(animHashes.IdleUpset, isIdle);
            previousDirection = currentDirection;
            SetCollidersActivation((int)previousDirection, false);

            currentDirection = GetCurrentDirection(axisValues);
            SetCollidersActivation((int)currentDirection, true);
        }
        else
        {
            idleTimeCounter += Time.deltaTime;
            if (idleTimeCounter >= idleTime)
            {
                isIdle = true;
                anim.SetBool(animHashes.IdleUpset, isIdle);
            }
        }

        Collider2D hit = HitSomething(currentDirection);

        if (!isIdle && !hit && !HitBomb(currentDirection))
        {
            rb.velocity = new Vector2(Speed * horizontal, Speed * vertical);
            NormalizeOrtogonalAxis(currentDirection);
        }
        else if(!isIdle && !isMovingCorner) // Do the corner slope here
        {           
            if(hit?.gameObject.layer != 12) // EdgeWall Layer
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
        float deadzoneX = 0.2f; // 0.3f
        float deadzoneY = 0.3f; // 0.4f
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

    private float GetNormalizedMovementValue(float raw)
    {
        if (raw > 0) return 1f;
        else if (raw < 0) return -1f;

        return 0f;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if((other.tag == "Explosion" || other.tag == "Enemy") && !isBlinking)
            DecrementHealth();
    }
}
