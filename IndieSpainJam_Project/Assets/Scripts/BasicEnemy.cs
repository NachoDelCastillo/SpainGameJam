using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BasicEnemy : MonoBehaviour
{
    [HideInInspector] public Transform target;

    Rigidbody2D rb;
    [SerializeField] Animator anim;

    [SerializeField] float speed, detectionRadius, destroyTime, damage, attackFrecuency;
    [SerializeField] GameObject sparksPrefab, bloodPrefab;
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

        SpriteRenderer sp = GetComponentInChildren<SpriteRenderer>();
        Color colorBruhhhh = sp.color;
        sp.color = new Color(colorBruhhhh.r, colorBruhhhh.g, colorBruhhhh.b, 0);
        GetComponentInChildren<SpriteRenderer>().DOFade(1, 1);
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

    void globalLightColorChange() { 

    }

    void Attack()
    {
        if(!attacked)
        {
            attacked = true;
            TrainManager.Instance.TakeDamage(damage);

            GameObject sparks = Instantiate(sparksPrefab, target.transform.position, Quaternion.identity);
            var velosidad = sparks.GetComponent<ParticleSystem>().velocityOverLifetime.x;
            velosidad.curveMultiplier = velosidad.curveMultiplier * TrainManager.Instance.GetmainVelocity() / 100f *20;

            AudioManager_PK.instance.Play("EnemyHit", UnityEngine.Random.Range(0.9f, 1.2f));
        }

        attackCD -= Time.deltaTime;
        if(attackCD <= 0)
        {
            attacked = false;
            attackCD = attackFrecuency + Random.Range(-0.1f, 0.1f);
        }


        anim.SetBool("Attacking", true);

        rb.velocity = Vector2.zero; 
    }

    public void Dmg()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        TrainManager.Instance.TakeDamage(damage);
    }

    void Death()
    {
        AudioManager_PK.instance.Play("EDeath", Random.Range(0.9f, 1.1f));

        TutorialManager.GetInstance().TryToChangePhase(TutorialManager.tutPhases.matarEnemigoTorreta);

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

        if (wagon != null && wagon.transform.GetChild(0).GetChild(0).GetComponent<Turret>() == null) states = States.Attack;
        else if(bullet != null)
        {

           

            Instantiate(bloodPrefab, transform.position, Quaternion.identity);

            bullet.EnemyImpacted();
            states = States.Death;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        WagonLogic wagon = collision.GetComponent<WagonLogic>();

        if (wagon == null) return;

        states = States.Chase;
    }
}
