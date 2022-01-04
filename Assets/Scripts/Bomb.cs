using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float deadzone = 1f;
    [SerializeField]
    private Collider2D bombCollider;

    private PlayerData playerData;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetPlayerData(PlayerData player)
    {
        playerData = player;
    }
   
    public virtual void Explode()
    {

    }

    //public bool IsInSameTile(Vector2 playerPos)
    //{
    //    return Vector2.Distance(transform.position, playerPos) == 0;
    //}

    public bool PlayerIsAroundTheBomb(Vector2 playerPos)
    {
        return Mathf.Abs(Vector2.Distance(playerPos, transform.position)) > deadzone;
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.tag != "DamageHit") return;

    //    if (other.transform.parent.gameObject.Equals(playerData.Player))
    //    {
    //        print("left");
    //        bombCollider.isTrigger = false;
    //    }
    //}
}
