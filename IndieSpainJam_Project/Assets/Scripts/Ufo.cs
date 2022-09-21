using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed;

    [HideInInspector] public Vector3 target;
    [HideInInspector] public bool inPlace, canMove;
    [HideInInspector] public static bool attacckedFinished;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) Move(target);
    }

    public void Move(Vector3 target)
    {
        if (Vector3.Distance(target, transform.position) > 0.1f)
        {
            Vector3 direction = target - transform.position;
            direction.Normalize();

            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
            inPlace = true;
        }
    }

    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        canMove = false;

        Debug.Log("Attack");

        yield return new WaitForSeconds(5);

        attacckedFinished = true;
    }
}
