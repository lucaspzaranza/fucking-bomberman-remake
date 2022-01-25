using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Enemy : Character
{
    #region Serializable Fields

    [SerializeField] protected LayerMask playerLayerMask;
    [SerializeField] protected Collider2D enemyCollider;
    [SerializeField] protected GameObject scoreText;
    [SerializeField] protected float timeToChangeDirection = 2f;
    [SerializeField] protected float playerDistance = 5f;

    #endregion

    #region Protected Fields

    protected GameObject whoHitMe;
    protected float timeCounter;

    #endregion

    #region Props

    [SerializeField] protected bool _isBoss = false;
    public bool IsBoss => _isBoss;

    [SerializeField] protected bool _followPlayer = false;
    public bool FollowPlayer => _followPlayer;

    [SerializeField] protected int _score;
    public int Score => _score;

    #endregion

    #region Methods

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(FollowPlayer)
        {
            if (!FoundPlayer())
                CalculateNewDirection();
        }
        else
            CalculateNewDirection();

        if (isMovingCorner)
            SmoothCornerMovement();
        else
            Move();
    }

    public virtual void Move() 
    {
        Vector2 movement = Vector2.zero;
        if ((int)currentDirection < 2)
            movement = new Vector2(0f, ((int)currentDirection == 0) ? _speed : -_speed);
        else if((int)currentDirection >= 2 && (int)currentDirection < 4)
            movement = new Vector2(((int)currentDirection == 2) ? -_speed : _speed, 0f);

        if(movement != Vector2.zero && currentDirection != Direction.None)
        {
            if (previousDirection != currentDirection)
                SetCollidersActivation((int)previousDirection, false);

            SetCollidersActivation((int)currentDirection, true);
        }

        Collider2D hit = HitSomething();

        if (!hit)
            rb.velocity = movement;
        else if (!isMovingCorner) // Do the corner slope here
        {
            if (hit?.gameObject.layer != 12) // EdgeWall Layer
                CornerSlopeDetection(hit);
            if (!isMovingCorner)
                rb.velocity = Vector2.zero;
        }

        SetMovementAnimators(Vector2Int.RoundToInt(movement));
    }

    public virtual void CalculateNewDirection()
    {
        timeCounter += Time.fixedDeltaTime;
        if(timeCounter  >= timeToChangeDirection)
        {
            previousDirection = currentDirection;
            currentDirection = (Direction)Random.Range(0, 5);
            timeCounter = 0f;
        }
    }

    private void SetCollidersActivation(int index, bool value)
    {
        if (index != 4)
            wallHitColliders[index].gameObject.SetActive(value);
        else
        {
            foreach (var collider in wallHitColliders)
            {
                if (collider.isActiveAndEnabled)
                    collider.gameObject.SetActive(false);
            }
        }
    }

    public override void DecrementHealth()
    {
        _health--;
        if (Health <= 0)
            GetScore(whoHitMe);
        else
            isBlinking = true;
    }

    public virtual void GetScore(GameObject explosion)
    {
        var explosionFire = explosion.GetComponent<ExplosionFire>();
        if (explosionFire.explosionBaseFire != null &&
            !explosionFire.explosionBaseFire.enemiesDestroyed.Contains(gameObject)) 
        {
            explosionFire.explosionBaseFire.enemiesDestroyed.Add(gameObject);
            int multipliedScore = Score * explosionFire.explosionBaseFire.enemiesDestroyed.Count;
            int newScore = explosionFire.playerWhoOwns.Score + multipliedScore;
            explosionFire.playerWhoOwns.SetScore(newScore);
            var scoreTextInstance = Instantiate(scoreText, transform.position, Quaternion.identity, UIController.instance.canvas.transform);
            var textMesh = scoreTextInstance.GetComponent<TextMeshProUGUI>();
        
            textMesh.text = multipliedScore.ToString();
            Destroy(gameObject);
        }
    }

    public virtual bool FoundPlayer()
    {
        bool foundPlayer = false;
        RaycastHit2D hit;
        foreach (var direction in SharedData.DirectionVectors)
        {
            hit = Physics2D.Raycast(transform.position, direction, playerDistance, playerLayerMask);
            foundPlayer = hit.collider != null;
            if(foundPlayer && !HasObstacleBeforePlayer(direction, hit.distance))
            {
                int index = SharedData.DirectionVectors.IndexOf(direction);
                currentDirection = (Direction)index;
                break;
            }
        }

        return foundPlayer;
    }

    private Collider2D HitSomething()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(blockLayerMask);
        if (currentDirection != Direction.None)
        {
            wallHitColliders[(int)currentDirection].OverlapCollider(contactFilter, results);
            return results[0];
        }
        else return null;
    }

    private bool HasObstacleBeforePlayer(Vector2 direction, float distance)
    {
        var hit = Physics2D.Raycast(transform.position, direction, distance, blockLayerMask);
        return hit;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Explosion" && !isBlinking)
        {
            whoHitMe = other.gameObject;
            DecrementHealth();
        }
    }

    #endregion
}
