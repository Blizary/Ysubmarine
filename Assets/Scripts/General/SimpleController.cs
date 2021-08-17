using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
	public float baseSpeed = 1f;
    public float speedUpModifier;
    public float speedDownModifier;
    private float currentSpeed;
    public float rotSpeed = 50f;

    public bool physicsController;

    public Animator animator;

	public Rigidbody2D rb;

	public Vector2 movement;

    private float lastClickTime;
    private const float doubleClick = 0.5f;

    private void Start()
    {
        currentSpeed = baseSpeed;
    }


    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); //Rotation
        movement.y = Input.GetAxisRaw("Vertical"); //Up and Down

        animator.SetFloat("Speed", Mathf.Abs(movement.y));

        if (movement.x > 0)
        {
            transform.Rotate(-Vector3.forward * rotSpeed * Time.deltaTime);
        }


        if (movement.x < 0)
        {
            transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
        }


        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            //fast speed
            currentSpeed = baseSpeed * 2;
        }
        else if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.W))
        {
            //slow speed
            currentSpeed = baseSpeed / 2;
        }
        else
        {
            currentSpeed = baseSpeed;
        }



    }

    private void FixedUpdate()
    {
        Vector3 moveProj = transform.up * movement.y;

        if (physicsController)
        {
            if (Input.GetKey(KeyCode.W) && Vector3.Magnitude(rb.velocity) <= 2)
            {
                rb.AddForce(new Vector2(moveProj.x, moveProj.y) * currentSpeed);
            }
            else if (Input.GetKey(KeyCode.S) && Vector3.Magnitude(rb.velocity) >= -2)
            {
                rb.AddForce(new Vector2(moveProj.x, moveProj.y) * currentSpeed);
            }
        }
        else
        {
            rb.MovePosition(rb.position + new Vector2(moveProj.x, moveProj.y) * currentSpeed * Time.deltaTime);
        }

    }


}
