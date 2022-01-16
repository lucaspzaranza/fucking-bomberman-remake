using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpeed : Item
{
    public float incrementRate = 0.25f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8) // player layer
        {
            var player = other.gameObject.GetComponentInParent<Player>(); // the player collider is in a child object
            player.PlayerData.SetSpeed(player.PlayerData.Speed + incrementRate);
            Destroy(gameObject);
        }
    }
}
