using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLife : Item
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8) // player layer
        {
            var player = other.gameObject.GetComponentInParent<Player>(); // the player collider is in a child object
            player.PlayerData.IncrementLifeCount();
            Destroy(gameObject);
        }
    }
}
