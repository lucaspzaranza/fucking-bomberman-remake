using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Destructible
{
    [SerializeField] protected GameObject itemExplosion;
    [SerializeField] protected Collider2D itemCollider;
    [SerializeField] protected SpriteRenderer itemSR;
    [SerializeField] protected float timeToEnableCollider = 0.75f;

    private void Start()
    {
        Brick.OnBrickDestroyed += EnableColliderIfItsInTheSamePosition;
    }

    public override void ExplosionHit()
    {
        Destroy(gameObject);
        Instantiate(itemExplosion, transform.position, Quaternion.identity);
    }

    private void EnableColliderIfItsInTheSamePosition(Vector2Int brickPosition)
    {
        if (brickPosition == Vector2Int.RoundToInt(transform.position))
            Invoke(nameof(ShowItem), timeToEnableCollider);
    }

    private void ShowItem()
    {
        itemSR.enabled = true;
        itemCollider.enabled = true;
    }

    private void OnDestroy()
    {
        Brick.OnBrickDestroyed -= EnableColliderIfItsInTheSamePosition;
    }
}
