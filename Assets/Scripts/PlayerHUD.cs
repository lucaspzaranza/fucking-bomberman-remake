using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public Player player;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI playerLives;

    private void OnEnable()
    {
        player.PlayerData.OnPlayerUpdateScore += UpdatePlayerScore;
        player.PlayerData.OnPlayerUpdateLives += UpdatePlayerLife;
    }

    private void OnDisable()
    {
        player.PlayerData.OnPlayerUpdateScore -= UpdatePlayerScore;
        player.PlayerData.OnPlayerUpdateLives -= UpdatePlayerLife;
    }

    public void UpdatePlayerScore(int newScore)
    {
        playerScore.text = newScore.ToString();
    }

    public void UpdatePlayerLife(int newLife)
    {
        playerLives.text = newLife.ToString();
    }
}
