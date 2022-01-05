using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionBasePrefab;
    [SerializeField] private GameObject explosionBodyPrefab;
    [SerializeField] private float positionOffset = 0.5f;
    [SerializeField] private float colliderOffset = 0.025f;
    [SerializeField] private float colliderYSize = 0.9f;
    [SerializeField] private float offsetVectorXOffset = 0.0125f;
    [SerializeField] private float offsetVectorYOffset = -0.37f;

    private int[] distances;

    [SerializeField] private Collider2D _bombCollider;
    public Collider2D BombCollider => _bombCollider;

    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

    [SerializeField] private ExplosionRaycast _explosionRaycast;
    public ExplosionRaycast ExplosionRaycast => _explosionRaycast;
   
    public void InitiateExplosion()
    {
        PlayerData?.IncrementBombCount();
        _explosionRaycast.CalculateRaycastAll(out distances);
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        var explosionBaseInstance = Instantiate(explosionBasePrefab, transform.position, Quaternion.identity);
        var baseFire = explosionBaseInstance.GetComponent<ExplosionFire>();
        baseFire.bomb = this;

        InstantiateExplosionBody(Direction.Right, distances[3]);
    }

    private void InstantiateExplosionBody(Direction direction, int force)
    {
        if (force == 0) return;
        var bodyPos = GetExplosionBodyPosition(direction);
        var explosionBody = Instantiate(explosionBodyPrefab, bodyPos, Quaternion.identity);
        var sr = explosionBody.GetComponent<SpriteRenderer>();
        sr.size = new Vector2(force, 1);
        var collider = explosionBody.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(force + colliderOffset, colliderYSize);
        float offsetX = force;
        offsetX /= 2;
        collider.offset = new Vector2(offsetX + offsetVectorXOffset, offsetVectorYOffset);
    }

    private Vector2 GetExplosionBodyPosition(Direction direction)
    {
        float x = Mathf.RoundToInt(transform.position.x);
        float y = Mathf.RoundToInt(transform.position.y);

        if((int)direction < 2) // Up and down
            y += (direction == Direction.Up) ?  1 : -1;
        else // Left and right
            x += (direction == Direction.Right) ?  1 : -1;
        x -= positionOffset;

        return new Vector2(x, y);
    }

    public void SetPlayerData(PlayerData player)
    {
        _playerData = player;
    }
}
