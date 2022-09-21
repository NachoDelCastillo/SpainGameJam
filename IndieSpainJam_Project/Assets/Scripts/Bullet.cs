using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb = null;
    [SerializeField] float velocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition((rb.position + (Vector2)transform.right * velocity * Time.fixedDeltaTime));
    }



}
