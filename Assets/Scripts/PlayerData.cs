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
    public const int maxLives = 9;
    public const int minScore = 0;
    public const int maxScore = 999999;

    #endregion

    #region Event

    public delegate void GameOver();
    public delegate void PlayerRespawn();
    public delegate void UpdateScore(int newScore);
    public delegate void UpdateLives(int newLives);

    public event GameOver OnGameOver;
    public event PlayerRespawn OnPlayerRespawn;
    public event UpdateScore OnPlayerUpdateScore;
    public event UpdateLives OnPlayerUpdateLives;

    #endregion

    #region Props

    [SerializeField]
    private GameObject _player;
    public GameObject Player => _player;

    [Range(minScore, maxScore)]
    [SerializeField] private int _score;
    public int Score => _score;

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

    #endregion

    #region Methods

    public void IncrementBombCount() => _bombCount = Mathf.Clamp(_bombCount + 1, 0, maxBombCount);
    public void DecrementBombCount() => _bombCount = Mathf.Clamp(_bombCount - 1, 0, maxBombCount);
    public void IncrementLifeCount()
    {
        _life = Mathf.Clamp(_life + 1, 0, maxLives);
        OnPlayerUpdateLives?.Invoke(_life);
    }
    
    public void SetScore(int newScore)
    {
        _score = newScore;
        OnPlayerUpdateScore?.Invoke(_score);
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

    // This one is different because sometimes it'll need to set a value bigger than one unit.
    public void SetBombForce(int newForce) => _bombForce = Mathf.Clamp(newForce, minBombForce, maxBombForce);

    #endregion
}
