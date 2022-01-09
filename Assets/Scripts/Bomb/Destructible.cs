using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Destructible: MonoBehaviour
{
    public virtual void ExplosionHit() { }
    public virtual void ExplosionHit(Vector2 tilePosition) { }
}
