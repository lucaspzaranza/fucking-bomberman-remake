using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Consts

    protected const float blinkFrameInterval = 0.025f;
    protected const int minHealth = 1;
    protected const int maxHealth = 5;
    protected const float cornerMoveSpeed = 5f;
    protected const float cornerMoveOffset = 0.5f;
    protected const float minSpeed = 1f;
    protected const float maxSpeed = 4f;

    #endregion

    #region Corner Slope

    [SerializeField] protected float xLowerCornerMinDeadzone = 0.14f;
    [SerializeField] protected float xLowerCornerMaxDeadzone = 0.3f;
    [SerializeField] protected float xUpperCornerMinDeadzone = 0.14f;
    [SerializeField] protected float xUpperCornerMaxDeadzone = 0.3f;
    [SerializeField] protected float yCornerDeadzone = 0.35f;
    [SerializeField] protected float cornerOffset = 0.05f;
    [SerializeField] protected float stopSmoothCornerDeadzone = 0.1f;
    [SerializeField] protected LayerMask blockCornerSlopeLayerMask;
    [SerializeField] protected LayerMask blockLayerMask;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Collider2D[] wallHitColliders = null;
    [SerializeField] protected Rigidbody2D rb;

    #endregion

    #region Serializable

    [SerializeField] protected float blinkDuration;
    [SerializeField] protected SpriteRenderer characterSR;
    [SerializeField] protected Direction previousDirection = Direction.None;
    [SerializeField] protected Direction currentDirection = Direction.Down;

    #endregion

    #region Protected Members

    protected AnimatorHashes animHashes = new AnimatorHashes();

    protected bool isBlinking = false;
    protected float blinkIntervalTimeCounter;
    protected float blinkTimeCounter;
    protected Vector2 cornerTarget;
    protected Vector2 adjacentCornerTileTarget;
    
    protected bool isMovingCorner = false;

    #endregion

    #region Props
    [Header("Character General Data")]
    [Range(minHealth, maxHealth)]
    [SerializeField] protected int _health;
    public int Health => _health;

    [SerializeField]
    [Range(minSpeed, maxSpeed)]
    protected float _speed;
    public float Speed => _speed;

    #endregion

    #region Methods

    public virtual void IncrementHealth() { }
    public virtual void DecrementHealth() { }

    public void SetSpeed(float newSpeed) => _speed = Mathf.Clamp(newSpeed, minSpeed, maxSpeed);

    public virtual void FixedUpdate()
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

    public virtual void Blink()
    {
        if (blinkTimeCounter < blinkDuration)
        {
            float alpha = characterSR.color.a;
            float transparency = alpha == 1 ? 0f : 1f;
            characterSR.color = new Color(255f, 255f, 255f, transparency);
        }
        else
        {
            isBlinking = false;
            characterSR.color = new Color(255f, 255f, 255f, 1f);
            blinkTimeCounter = 0f;
        }
    }

    protected void SetMovementAnimators(Vector2Int axisValues)
    {
        anim.SetInteger(animHashes.X, axisValues.x);
        anim.SetInteger(animHashes.Y, axisValues.y);
    }

    #region Corner Slope

    protected void CornerSlopeDetection(Collider2D hit)
    {
        if (IsInsideSlopArea(transform.position, hit, currentDirection))
            GetCornerDestination(hit.gameObject);
    }

    protected bool IsInsideSlopArea(Vector2 pos, Collider2D hit, Direction dir)
    {
        if (hit == null) return false;
        // The hit block corners positions
        Vector2 upperLeft = new Vector2(hit.bounds.min.x, hit.bounds.max.y);
        Vector2 upperRight = hit.bounds.max;
        Vector2 lowerLeft = hit.bounds.min;
        Vector2 lowerRight = new Vector2(hit.bounds.max.x, hit.bounds.min.y);
        Vector2 chosenCorner = Vector2.zero;
        Vector2 hitPos = hit.transform.position;
        float distance = 0f, minDeadzone = 0f, maxDeadzone = 0f;
        bool result = false;

        if ((int)dir < 2) // Up and Down, Y Axis
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
            bool isUpperCorner = chosenCorner == upperLeft || chosenCorner == upperRight;
            minDeadzone = isUpperCorner ? xUpperCornerMinDeadzone : xLowerCornerMinDeadzone;
            maxDeadzone = isUpperCorner ? xUpperCornerMaxDeadzone : xLowerCornerMaxDeadzone;
            result = distance > minDeadzone && distance <= maxDeadzone;
        }

        return result;
    }

    protected void GetCornerDestination(GameObject hitObj)
    {
        float offset = 0f;
        Vector2 nextTile = SharedData.GetNextCornerPosition(transform.position, hitObj.transform.position, currentDirection);
        adjacentCornerTileTarget = SharedData.GetAdjacentCornerTilePosition(transform.position, hitObj.transform.position, currentDirection);

        isMovingCorner = CanDoCornerSlope(nextTile);
        if (!isMovingCorner) return;
        else
            isMovingCorner &= CanDoCornerSlope(adjacentCornerTileTarget);

        if (!isMovingCorner) return;

        // Negative to down/left, positive to up/right directions
        offset = ((int)currentDirection % 3 != 0) ? -cornerOffset : cornerOffset;

        if ((int)currentDirection < 2) // Up/Down
            cornerTarget = new Vector2(nextTile.x, transform.position.y + offset);
        else // Left/Right
            cornerTarget = new Vector2(transform.position.x + offset, nextTile.y);
    }

    protected bool CanDoCornerSlope(Vector2 nextTile)
    {
        bool canSlope = !HitSomeToPreventCornerSlope(nextTile);
        return canSlope;
    }

    protected bool HitSomeToPreventCornerSlope(Vector2 position)
    {
        var hitCollider = Physics2D.OverlapPoint(position, blockCornerSlopeLayerMask);
        return hitCollider; 
    }

    protected void SmoothCornerMovement()
    {
        if ((int)currentDirection < 2) // Up and Down
            VerticalCornerMovement();
        else // Left and Right
            HorizontalCornerMovement();

        SetMovementAnimators(Vector2Int.RoundToInt(rb.velocity));
    }

    protected void VerticalCornerMovement()
    {
        int signal = 1;
        if (Mathf.Abs(transform.position.x - cornerTarget.x) > stopSmoothCornerDeadzone)
        {
            signal = (cornerTarget.x > transform.position.x) ? 1 : -1;
            rb.velocity = new Vector2(Speed * signal, 0f);
        }
        else if (Mathf.Abs(transform.position.y - cornerTarget.y) > stopSmoothCornerDeadzone)
        {
            signal = (cornerTarget.y > transform.position.y) ? 1 : -1;
            rb.velocity = new Vector2(0f, Speed * signal);
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
            rb.velocity = new Vector2(0f, Speed * signal);
        }
        else if (Mathf.Abs(transform.position.x - cornerTarget.x) > stopSmoothCornerDeadzone)
        {
            signal = (cornerTarget.x > transform.position.x) ? 1 : -1;
            rb.velocity = new Vector2(Speed * signal, 0f);
        }
        else
            isMovingCorner = false;
    }

    protected Collider2D HitSomething(Direction currentDirection)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(blockLayerMask);
        wallHitColliders[(int)currentDirection].OverlapCollider(contactFilter, results);
        return results[0];
    }

    #endregion

    #endregion
}
