using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBombForce : Item
{
    public bool isSuperForce = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8) // player layer
        {
            var player = other.gameObject.GetComponentInParent<Player>(); // the player collider is in a child object
            if(!isSuperForce)
                player.PlayerData.SetBombForce(player.PlayerData.BombForce + 1);
            else
                player.PlayerData.SetBombForce(PlayerData.maxBombForce);

            Destroy(gameObject);
        }
    }
}
