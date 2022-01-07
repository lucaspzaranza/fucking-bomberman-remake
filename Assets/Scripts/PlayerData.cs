using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public const int maxBombCount = 8;
    public const int minBombForce = 1;
    public const int maxBombForce = 8;
    public const float minSpeed = 3f;
    public const float maxSpeed = 5f;
    public const int maxLives = 9;

    [SerializeField]
    private GameObject _player;
    public GameObject Player => _player;

    [SerializeField]
    [Range(0, maxLives)]
    private int _life;
    public int Life => _life;

    [SerializeField]
    [Range(0, maxBombCount)]
    private int _bombCount;
    public int BombCount => _bombCount;

    [SerializeField]
    [Range(minBombForce, maxBombForce)]
    private int _bombForce;
    public int BombForce => _bombForce;

    [SerializeField]
    [Range(minSpeed, maxSpeed)]
    private float _speed;
    public float Speed => _speed;

    public void IncrementBombCount() => _bombCount = Mathf.Clamp(_bombCount + 1, 0, maxBombCount);
    public void DecrementBombCount() => _bombCount = Mathf.Clamp(_bombCount - 1, 0, maxBombCount);
    public void IncrementLifeCount() => _life = Mathf.Clamp(_life + 1, 0, maxLives);
    public void DecrementLifeCount() => _life = Mathf.Clamp(_life - 1, 0, maxLives);
    public void SetSpeed(float newSpeed) => _speed = Mathf.Clamp(newSpeed, minSpeed, maxSpeed);
    public void SetBombForce(int newForce) => _bombForce = Mathf.Clamp(newForce, minBombForce, maxBombForce);
}
