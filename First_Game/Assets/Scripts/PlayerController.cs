using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;
    private enum State {idle, running, jumping, falling, hurt};
    private State state = State.idle;
    [SerializeField] private LayerMask land, frame;
    [SerializeField] private Text cherriesUI, healthUI;

    private float speed = 5f, jump = 7.5f;
    private int cherries = 0, health = 3;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement();
        SetState();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Collectable")
        {
            Destroy(other.gameObject);
            cherries += 1;
            cherriesUI.text = cherries.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemies")
        {
            if(state == State.falling)
            {
                Destroy(other.gameObject);
                rb.velocity = new Vector2(rb.velocity.x, jump);
                state = State.jumping;
            }
            else
            {
                state = State.hurt;
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                health -=1;
                healthUI.text = health.ToString();
            }
        }
    }

    private void Movement()
    {
        if(state != State.hurt)
        {
            float hdirection = Input.GetAxis("Horizontal");
            if(hdirection > 0)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
                transform.localScale = new Vector2(1, 1);
            }
            if(hdirection < 0)
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                transform.localScale = new Vector2(-1, 1);
            } 
            if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(land))
            {
                rb.velocity = new Vector2(rb.velocity.x, jump);
                state = State.jumping;
            }
        }
    }

    private void SetState()
    {
        if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < 0.1f)
                state = State.idle;
        }
        else if(state == State.jumping)
        {
            if(rb.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(land) || coll.IsTouchingLayers(frame))
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
        anim.SetInteger("state",(int)state);
    }
}