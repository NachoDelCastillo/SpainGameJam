using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerBelow : MonoBehaviour
{
    [SerializeField] LocalMultiplayerManager multiplayerManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController_2D player = collision.GetComponent<PlayerController_2D>();

        if (player == null)
        {
            Destroy(collision.gameObject);
            return;
        }

        //Respawn Player
        multiplayerManager.Respawn(player);
    }
}
