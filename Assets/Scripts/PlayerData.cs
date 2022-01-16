using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    #region Consts

    public const int maxBombCount = 8;
    public const int minBombForce = 1;
    public const int maxBombForce = 8;
    public const float minSpeed = 3f;
    public const float maxSpeed = 4.5f;
    public const int maxLives = 9;
    public const int minHealth = 1;
    public const int maxHealth = 5;

    #endregion

    #region Event

    public delegate void DamageTaken();
    public delegate void PlayerDied();
    public delegate void GameOver();
    public delegate void PlayerRespawn();

    public event DamageTaken OnDamageTaken;
    public event PlayerDied OnPlayerDied;
    public event GameOver OnGameOver;
    public event PlayerRespawn OnPlayerRespawn;

    #endregion

    #region Props

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
    [Range(minHealth, maxHealth)]
    private int _health;
    public int Health => _health;

    [SerializeField]
    [Range(minSpeed, maxSpeed)]
    private float _speed;
    public float Speed => _speed;

    #endregion

    #region Methods

    public void IncrementBombCount() => _bombCount = Mathf.Clamp(_bombCount + 1, 0, maxBombCount);
    public void DecrementBombCount() => _bombCount = Mathf.Clamp(_bombCount - 1, 0, maxBombCount);
    public void IncrementLifeCount()
    {
        _life = Mathf.Clamp(_life + 1, 0, maxLives);
        UIController.instance.LifeText.text = _life.ToString();
    }
        
    public void DecrementLifeCount()
    {
        _life = Mathf.Clamp(_life - 1, 0, maxLives);
        UIController.instance.LifeText.text = _life.ToString();
        if (_life <= 0)
            OnGameOver?.Invoke();
        else
            OnPlayerRespawn?.Invoke();
    }
    public void IncrementHealth() => _health = Mathf.Clamp(_health + 1, minHealth, maxHealth);
    public void DecrementHealth()
    {
        _health = Mathf.Clamp(_health - 1, 0, maxHealth);

        if (_health <= 0)
            OnPlayerDied?.Invoke();
        else
            OnDamageTaken?.Invoke();
    }

    // These one are different because sometimes it'll need to set a value bigger than one unit.
    public void SetSpeed(float newSpeed) => _speed = Mathf.Clamp(newSpeed, minSpeed, maxSpeed);
    public void SetBombForce(int newForce) => _bombForce = Mathf.Clamp(newForce, minBombForce, maxBombForce);

    #endregion
}
