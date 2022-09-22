using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController_2D playerC = collision.GetComponent<PlayerController_2D>();
        if (playerC)
        {
            LocalMultiplayerManager.GetInstance().Respawn(playerC);
        }
    }
}