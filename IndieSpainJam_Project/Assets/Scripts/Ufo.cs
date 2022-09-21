using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToLane(Vector3 target)
    {
        if (Vector3.Distance(target, transform.position) > 0.1f)
        {
            Vector3 direction = target - transform.position;
            direction.Normalize();

            rb.velocity = direction * speed;
        }
        else rb.velocity = Vector2.zero;

    }

    public void Attack()
    {

    }

    public void Leave()
    {

    }
}
