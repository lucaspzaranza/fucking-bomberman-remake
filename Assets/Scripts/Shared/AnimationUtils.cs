using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUtils : MonoBehaviour
{
    private float yPos = 0f;

    public void SelfDestuct()
    {
        Destroy(gameObject);
    }

    public void MemorizeY()
    {
        yPos = transform.position.y;
    }

    public void RoundY()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = new Vector2(transform.position.x, yPos);
    }
}
