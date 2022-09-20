using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public Transform target;

    Rigidbody2D rb;
    Animator anim;

    [SerializeField] float speed, detectionRadius;
    [SerializeField] CircleCollider2D colliderDetection;

    enum States { Chase, Attack, Death}
    States states;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        colliderDetection.radius = detectionRadius;

        states = States.Chase;
    }

    // Update is called once per frame
    void Update()
    {
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
        Debug.Log("Chase");

        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        rb.velocity = direction * speed;
    }

    void Attack()
    {
        Debug.Log("Attack");

        rb.velocity = Vector2.zero;
    }

    void Death()
    {
        Debug.Log("Death");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        WagonLogic wagon = collision.GetComponent<WagonLogic>();

        Bullet bullet = collision.GetComponent<Bullet>();

        if (wagon == null && bullet == null) return;

        if (wagon != null) states = States.Attack;
        else states = States.Death;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        WagonLogic wagon = collision.GetComponent<WagonLogic>();

        if (wagon == null) return;

        states = States.Chase;
    }
}
