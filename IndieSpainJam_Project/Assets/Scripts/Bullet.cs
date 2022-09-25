using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb = null;
    [SerializeField] float velocity, timeForDestroy;
    [SerializeField] ParticleSystem pSys;
    bool dead;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, timeForDestroy);
        dead = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dead) return;

        rb.MovePosition((rb.position + (Vector2)transform.right * velocity * Time.fixedDeltaTime));
    }


    public void ShieldImpacted()
    {
        dead = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        rb.isKinematic = true;
        pSys.Play();
    }


    public void EnemyImpacted()
    {
        dead = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        rb.isKinematic = true;
    }

    
}
