using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonLogic : MonoBehaviour
{
    public int RailRow = 1;

    private void Update()
    {
        //GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController_2D player = collision.rigidbody.GetComponent<PlayerController_2D>();

        if (player != null)
            player.transform.SetParent(this.transform);

        collision.transform.SetParent(this.transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController_2D player = collision.rigidbody.GetComponent<PlayerController_2D>();

        if (player != null)
            player.transform.SetParent(null);

        collision.transform.SetParent(null);
    }
}
