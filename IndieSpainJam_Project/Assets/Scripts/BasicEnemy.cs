using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public Transform target;

    Rigidbody2D rb;
    [SerializeField] Animator anim;

    [SerializeField] float speed, detectionRadius, destroyTime;
    [SerializeField] CircleCollider2D colliderDetection;

    enum States { Chase, Attack, Death }
    States states;

    float initialScale;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        colliderDetection.radius = detectionRadius;

        states = States.Chase;

        initialScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) states = States.Death;

        switch (states)
        {
            case States.Chase:
                Chase();
                break;

            case States.Attack:
                Attack();
                break;

            case States.Death:
                Death();
                break;
        }
    }

    void Chase()
    {
        anim.SetBool("Attacking", false);

        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        if (direction.x < 0) transform.localScale = new Vector3(-initialScale, initialScale, initialScale);
        else if(direction.x > 0) transform.localScale = new Vector3(initialScale, initialScale, initialScale);

        rb.velocity = direction * speed;
    }


    void Attack()
    {
        anim.SetBool("Attacking", true);

        rb.velocity = Vector2.zero;
    }

    void Death()
    {
        Debug.Log("Death");

        rb.velocity = Vector2.zero;

        anim.SetTrigger("Death");
        Destroy(gameObject, destroyTime);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Entrabalas");
        PlayerController_2D wagon = collision.GetComponent<PlayerController_2D>();

        Bullet bullet = collision.GetComponent<Bullet>();

        if (wagon == null && bullet == null) return;

        if (wagon != null) states = States.Attack;
        else states = States.Death;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController_2D wagon = collision.GetComponent<PlayerController_2D>();

        if (wagon == null) return;

        states = States.Chase;
    }
}
