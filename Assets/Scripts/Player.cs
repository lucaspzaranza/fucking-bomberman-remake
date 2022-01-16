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
    private const float blinkFrameInterval = 0.025f;

    [SerializeField] private float xCornerDeadzone = 0.2f;
    [SerializeField] private float yCornerDeadzone = 0.35f;
    [SerializeField] private float cornerOffset = 0.05f;
    [SerializeField] private float stopSmoothCornerDeadzone = 0.1f;
    [SerializeField] private float blinkDuration;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask blockLayerMask;
    [SerializeField] private LayerMask blockCornerSlopeLayerMask;
    [SerializeField] private LayerMask bombLayerMask;
    [SerializeField] private Collider2D[] wallHitColliders = null;
    [SerializeField] private Collider2D[] bombHitColliders = null;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private SpriteRenderer playerSR;

    private PlayerAnimatorHashes animHashes = new PlayerAnimatorHashes();
    private Direction currentDirection = Direction.Down;
    private Direction previousDirection = 0;
    private bool isBlinking = false;
    private bool isMovingCorner = false;
    private Vector2 cornerTarget;
    private float blinkIntervalTimeCounter;
    private float blinkTimeCounter;

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
        PlayerData.OnDamageTaken += HandleOnDamageTaken;
        PlayerData.OnPlayerDied += HandleOnPlayerDied;
        PlayerData.OnPlayerRespawn += HandleOnPlayerRespawn;
        PlayerData.OnGameOver += HandleOnGameOver;
    }

    void Update()
    {
        if (isMovingCorner)
            SmoothCornerMovement();
        else
            Move();
    }

    private void FixedUpdate()
    {
        if (isBlinking)
        {
            blinkIntervalTimeCounter += Time.fixedDeltaTime;
            blinkTimeCounter += Time.fixedDeltaTime;
            if (blinkIntervalTimeCounter >= blinkFrameInterval)
            {
                Blink();
                blinkIntervalTimeCounter = 0f;
            }
        }
    }

    private void HandleOnDamageTaken()
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

    private void HandleOnPlayerDied()
    {
        TriggerDieAnimation();
        PlayerData.DecrementLifeCount();
    }

    private void TriggerDieAnimation() => anim.SetTrigger(animHashes.Die);
    private void ResetPlayerData()
    {
        PlayerData.IncrementHealth(); // set health = 1
        transform.position = new Vector2(0, 0);
        anim.SetTrigger(animHashes.ResetAnimator);
    }

    public void Blink()
    {
        if (blinkTimeCounter < blinkDuration)
        {
            float alpha = playerSR.color.a;
            float transparency = alpha == 1 ? 0f : 1f;
            playerSR.color = new Color(255f, 255f, 255f, transparency);
        }
        else
        {
            isBlinking = false;
            playerSR.color = new Color(255f, 255f, 255f, 1f);
            blinkTimeCounter = 0f;
        }
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

    private void SmoothCornerMovement()
    {
        if ((int)currentDirection < 2) // Up and Down
            VerticalCornerMovement();
        else // Left and Right
            HorizontalCornerMovement();
            
        SetMovementAnimators(Vector2Int.RoundToInt(rb.velocity));
    }

    private void VerticalCornerMovement()
    {
        int signal = 1;
        if (Mathf.Abs(transform.position.x - cornerTarget.x) > stopSmoothCornerDeadzone)
        {
            signal = (cornerTarget.x > transform.position.x) ? 1 : -1;
            rb.velocity = new Vector2(PlayerData.Speed * signal, 0f);
        }
        else if (Mathf.Abs(transform.position.y - cornerTarget.y) > stopSmoothCornerDeadzone)
        {
            signal = (cornerTarget.y > transform.position.y) ? 1 : -1;
            rb.velocity = new Vector2(0f, PlayerData.Speed * signal);
        }
        else
            isMovingCorner = false;
    }

    private void HorizontalCornerMovement()
    {
        int signal = 1;
        if (Mathf.Abs(transform.position.y - cornerTarget.y) > stopSmoothCornerDeadzone)
        {
            signal = (cornerTarget.y > transform.position.y) ? 1 : -1;
            rb.velocity = new Vector2(0f, PlayerData.Speed * signal);
        }
        else if (Mathf.Abs(transform.position.x - cornerTarget.x) > stopSmoothCornerDeadzone)
        {
            signal = (cornerTarget.x > transform.position.x) ? 1 : -1;
            rb.velocity = new Vector2(PlayerData.Speed * signal, 0f);
        }
        else
            isMovingCorner = false;
    }

    private void GetCornerDestination(GameObject hitObj)
    {
        float offset = 0f;
        Vector2 nextTile = SharedData.GetNextCornerPosition(transform.position, hitObj.transform.position, currentDirection);

        isMovingCorner = CanDoCornerSlope(nextTile);
        if (!isMovingCorner)
            return;

        // Negative to down/left, positive to up/right directions
        offset = ((int)currentDirection % 3 != 0)? -cornerOffset : cornerOffset;

        if ((int)currentDirection < 2) // Up/Down
            cornerTarget = new Vector2(nextTile.x, transform.position.y + offset);
        else // Left/Right
            cornerTarget = new Vector2(transform.position.x + offset, nextTile.y);
    }

    private bool CanDoCornerSlope(Vector2 nextTile)
    {
        bool canSlope = !HitSomeToPreventCornerSlope(nextTile);
        return canSlope;
    }

    private void CornerSlopeDetection(Collider2D hit)
    {
        if((int)currentDirection < 2) // Up/Down, check X Axis
            MovingCornerSetup(hit);
        else // Left/Right, check Y Axis
            MovingCornerSetup(hit);
    }

    private void MovingCornerSetup(Collider2D hit)
    {
        if (IsInsideSlopArea(transform.position, hit, currentDirection))
            GetCornerDestination(hit.gameObject);
    }

    private bool IsInsideSlopArea(Vector2 pos, Collider2D hit, Direction dir)
    {
        if (hit == null) return false;
        // The hit block corners positions
        Vector2 upperLeft = new Vector2(hit.bounds.min.x, hit.bounds.max.y);
        Vector2 upperRight = hit.bounds.max;
        Vector2 lowerLeft = hit.bounds.min;
        Vector2 lowerRight = new Vector2(hit.bounds.max.x, hit.bounds.min.y);
        Vector2 chosenCorner = Vector2.zero;
        Vector2 hitPos = hit.transform.position;
        float distance = 0f;
        bool result = false;

        if((int)dir < 2) // Up and Down, Y Axis
        {
            if (dir == Direction.Up) // Lower corners
                chosenCorner = (pos.x < hitPos.x) ? lowerLeft : lowerRight;
            else if (dir == Direction.Down) // Upper corners
                chosenCorner = (pos.x < hitPos.x) ? upperLeft : upperRight;
            distance = Mathf.Abs(chosenCorner.x - pos.x);
            result = distance <= yCornerDeadzone;
        }
        else // Left And Right, X Axis
        {
            if (dir == Direction.Right) // Left corners
                chosenCorner = (pos.y < hitPos.y) ? lowerLeft : upperLeft;
            else if (dir == Direction.Left) //Right corners
                chosenCorner = (pos.y < hitPos.y) ? lowerRight : upperRight;
            distance = Mathf.Abs(chosenCorner.y - pos.y);
            result = distance <= xCornerDeadzone;
        }

        return result;
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

    private bool HitSomeToPreventCornerSlope(Vector2 position)
    {
        var hitCollider = Physics2D.OverlapPoint(position, blockCornerSlopeLayerMask);
        return hitCollider != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Explosion" && !isBlinking)
            PlayerData.DecrementHealth();
    }
}
