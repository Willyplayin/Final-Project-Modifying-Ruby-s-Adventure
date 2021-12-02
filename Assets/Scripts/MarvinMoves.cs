using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarvinMoves : MonoBehaviour
{
    public float speed;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;

    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        position.x = position.x + Time.deltaTime * speed * direction;
        animator.SetFloat("Move X", direction);
        animator.SetFloat("Move Y", 0);

        rigidbody2D.MovePosition(position);
    }

}
