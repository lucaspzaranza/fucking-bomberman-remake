using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionBasePrefab;
    [SerializeField] private GameObject explosionBeakPrefab;
    [SerializeField] private GameObject explosionXBodyPrefab;
    [SerializeField] private GameObject explosionYBodyPrefab;
    [SerializeField] private float positionOffset = 0.5f;

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

        GameObject body = null, beak = null;

        for (int i = 0; i < explosionDirectionForces.Length; i++)
        {
            int force = explosionDirectionForces[i];
            Direction direction = (Direction)i;
            if(force == PlayerData.BombForce) // has beak
            {
                if(force != 1) // not a mini explosion
                {
                    body = InstantiateExplosionBody(direction, force - 1); // minus one since the beak counts as the last index
                    if (force == PlayerData.BombForce && body != null)
                        beak = InstantiateExplosionBeak(direction, body.transform.position, force);
                }
                else // it's a mini explosion
                    beak = InstantiateExplosionBeak(direction, explosionBaseInstance.transform.position, force);
            }
            else // hasn't beak
                body = InstantiateExplosionBody(direction, force);

            body?.transform.SetParent(explosionBaseInstance.transform);
            beak?.transform.SetParent(explosionBaseInstance.transform);
        }
    }

    private GameObject InstantiateExplosionBody(Direction direction, int force)
    {
        if (force <= 0) return null;
        var bodyPos = GetExplosionBodyPosition(direction);
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

        SharedData.RenameObjectWithDirection(explosionBody, direction);
        return explosionBody;
    }

    private GameObject InstantiateExplosionBeak(Direction direction, Vector2 bodyPos, int force)
    {
        Vector2 beakPos = Vector2.zero;
        if (force != 1)
        {
            if ((int)direction < 2)
            {
                float distance = (direction == Direction.Up) ? force - positionOffset : -force +positionOffset;
                beakPos = new Vector2(bodyPos.x, bodyPos.y + distance);
            }
            else
            {
                float distance = (direction == Direction.Right) ? force - positionOffset : -force + positionOffset;
                beakPos = new Vector2(bodyPos.x + distance, bodyPos.y);
            }
        }
        else
            beakPos = GetMiniExplosionBeakPosition(direction);

        var beak = Instantiate(explosionBeakPrefab, beakPos, GetBeakRotation(direction));
        beak.GetComponent<ExplosionFire>().direction = direction;
        //beak.name = beak.name.Replace("(Clone)", $" {direction}");
        SharedData.RenameObjectWithDirection(beak, direction);

        return beak;
    }

    private Quaternion GetBeakRotation(Direction direction)
    {
        Quaternion result;
        if ((int)direction < 2)
            result = Quaternion.AngleAxis((direction == Direction.Up) ? -270f : 270f, Vector3.forward);
        else
            result = new Quaternion(0, (direction == Direction.Right )? 0f : 180f, 0, 0);
        return result;
    }

    private Vector2 GetExplosionBodyPosition(Direction direction)
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

    private Vector2 GetMiniExplosionBeakPosition(Direction direction)
    {
        return SharedData.GetNextTile(transform.position, direction);
    }

    public void SetPlayerData(PlayerData player)
    {
        _playerData = player;
    }
}
