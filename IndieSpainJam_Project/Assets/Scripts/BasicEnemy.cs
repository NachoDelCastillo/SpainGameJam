using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [HideInInspector] public Transform target;

    Rigidbody2D rb;
    [SerializeField] Animator anim;

    [SerializeField] float speed, detectionRadius, destroyTime, damage, attackFrecuency;
    [SerializeField] CircleCollider2D colliderDetection;

    [HideInInspector] public EnemySpawner enemySpawner;

    enum States { Chase, Attack, Death }
    States states;

    float initialScale, attackCD;

    bool attacked = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        colliderDetection.radius = detectionRadius;

        states = States.Chase;

        initialScale = transform.localScale.x;

        attackCD = attackFrecuency;
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
        anim.SetBool("Attacking", false);

        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        if (direction.x < 0) transform.localScale = new Vector3(-initialScale, initialScale, initialScale);
        else if(direction.x > 0) transform.localScale = new Vector3(initialScale, initialScale, initialScale);

        rb.velocity = direction * speed;
    }


    void Attack()
    {
        if(!attacked)
        {
            attacked = true;
            TrainManager.Instance.TakeDamage(damage);
        }

        attackCD -= Time.deltaTime;
        if(attackCD <= 0)
        {
            attacked = false;
            attackCD = attackFrecuency;
        }


        anim.SetBool("Attacking", true);

        rb.velocity = Vector2.zero; 
    }

    public void Dmg()
    {
        TrainManager.Instance.TakeDamage(damage);
    }

    void Death()
    {
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Death");
        enemySpawner.enemysAlive.Remove(gameObject);
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Entrabalas");
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
