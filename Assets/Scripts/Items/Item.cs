using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Destructible
{
    [SerializeField] protected GameObject itemExplosion;
    public override void ExplosionHit()
    {
        Destroy(gameObject);
        Instantiate(itemExplosion, transform.position, Quaternion.identity);
    }
}
