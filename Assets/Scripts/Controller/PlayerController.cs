using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    public float speed;         //人物速度
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        anim.SetFloat("run",Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y));
    }
    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        if (GetComponent<PlayerStats>().isDead == false)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            rb.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
            if (horizontalInput > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (horizontalInput < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }
}
