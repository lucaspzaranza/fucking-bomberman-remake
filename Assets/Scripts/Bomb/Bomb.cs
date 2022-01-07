using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionBasePrefab;
    [SerializeField] private GameObject explosionXBodyPrefab;
    [SerializeField] private GameObject explosionYBodyPrefab;
    // Valores antigos: 0.465f e 0.535f;
    [SerializeField] private float positionOffset = 0.5f;
    [SerializeField] private float positionMinOffset = 0.035f;
    [SerializeField] private float colliderYSize = 0.9f;
    //[SerializeField] private Vector2 offsetVector;

    private int[] explosionDirectionForces;

    [SerializeField] private Collider2D _bombCollider;
    public Collider2D BombCollider => _bombCollider;

    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

    [SerializeField] private ExplosionRaycast _explosionRaycast;
    public ExplosionRaycast ExplosionRaycast => _explosionRaycast;
   
    public void InitiateExplosion()
    {
        PlayerData?.IncrementBombCount();
        _explosionRaycast.CalculateRaycastAll(out explosionDirectionForces);
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        var explosionBaseInstance = Instantiate(explosionBasePrefab, transform.position, Quaternion.identity);
        var baseFire = explosionBaseInstance.GetComponent<ExplosionFire>();
        baseFire.force = PlayerData.BombForce;

        InstantiateExplosionBody(Direction.Up, explosionDirectionForces[0]);
        InstantiateExplosionBody(Direction.Down, explosionDirectionForces[1]);
        InstantiateExplosionBody(Direction.Left, explosionDirectionForces[2]);
        InstantiateExplosionBody(Direction.Right, explosionDirectionForces[3]);
    }

    private void InstantiateExplosionBody(Direction direction, int force)
    {
        if (force == 0) return;
        var bodyPos = GetExplosionBodyPosition(direction, force);
        GameObject explosionBody;
        if((int)direction < 2)
            explosionBody = Instantiate(explosionYBodyPrefab, bodyPos, Quaternion.identity);
        else
            explosionBody = Instantiate(explosionXBodyPrefab, bodyPos, Quaternion.identity);

        if (direction == Direction.Left)
            explosionBody.transform.localScale = new Vector2(-explosionBody.transform.localScale.x, explosionBody.transform.localScale.y);
        else if(direction == Direction.Up)
            explosionBody.transform.localScale = new Vector2(explosionBody.transform.localScale.x, -explosionBody.transform.localScale.y);

        var bodyFire = explosionBody.GetComponent<ExplosionFire>();
        bodyFire.direction = direction;
        bodyFire.force = force;

        var sr = explosionBody.GetComponent<SpriteRenderer>();
        if((int)direction < 2)
            sr.size = new Vector2(1, force);
        else
            sr.size = new Vector2(force, 1);

        //var collider = explosionBody.GetComponent<BoxCollider2D>();
        //collider.size = new Vector2(force, colliderYSize);
    }

    private Vector2 GetExplosionBodyPosition(Direction direction, int force)
    {
        Vector2 nextTile = SharedData.GetNextTile(transform.position, direction);
        if((int)direction >= 2) // Left and right
        {
            float newX = nextTile.x - ((direction == Direction.Right)? positionOffset : -positionOffset);
            nextTile.Set(newX, nextTile.y);
        }
        else
        {
            float newY = nextTile.y + ((direction == Direction.Up)? -positionOffset : positionOffset);
            nextTile.Set(nextTile.x, newY);
        }

        return nextTile;
    }

    public void SetPlayerData(PlayerData player)
    {
        _playerData = player;
    }
}
