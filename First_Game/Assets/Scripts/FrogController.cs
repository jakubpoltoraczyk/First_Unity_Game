using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    private float speed = 2f, jump = 5f;
    private enum State {idle, jump, fall, death};
    private State state = State.idle;
    private enum Direction {left, right};
    private Direction direction = Direction.left;
    [SerializeField] private LayerMask land;
    [SerializeField] private float LeftCorner;
    [SerializeField] private float RightCorner;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        SetState();
    }

    private void Movement()
    {
        if(direction == Direction.left)
        {
            if(transform.position.x - speed > LeftCorner)
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                rb.velocity = new Vector2(rb.velocity.x, jump);
                state = State.jump;
            }
            else
            {
                direction = Direction.right;
                transform.localScale = new Vector2(-1,1);
            }
        }
        else if(direction == Direction.right)
        {
            if(transform.position.x + speed < RightCorner)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
                rb.velocity = new Vector2(rb.velocity.x, jump);
                state = State.jump;
            }
            else
            {
                direction = Direction.left;
                transform.localScale = new Vector2(1,1);
            }
        }
    }

    private void SetState()
    {
        if(state == State.jump)
        {
            if(rb.velocity.y < 0.1f)
            {
                state = State.fall;
            }
        }
        else if(state == State.fall && coll.IsTouchingLayers(land))
        {
            state = State.idle;
        }
        anim.SetInteger("state", (int)state);
    }

    private void ChangeToDeath()
    {
        state = State.death;
        rb.velocity = new Vector2(.0f, .0f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
