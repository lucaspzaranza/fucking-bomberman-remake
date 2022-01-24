using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickExplosion : MonoBehaviour
{
    public float timeToDestroy = 1f;

    private void Start()
    {
        Invoke(nameof(SelfDestruct), timeToDestroy);
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
