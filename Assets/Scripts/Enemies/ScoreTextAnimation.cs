using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextAnimation : MonoBehaviour
{
    public float timeToDestroy = 3f;
    public float lerpRate = 0.005f;
    public float distance = 2f;
    private float timeCounter;
    private Vector2 destiny;

    private void Start()
    {
        destiny = new Vector2(transform.position.x, transform.position.y + distance);
    }

    private void FixedUpdate()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter >= timeToDestroy)
            Destroy(gameObject);
        else
            transform.position = Vector2.Lerp(transform.position, destiny, lerpRate);
    }
}
