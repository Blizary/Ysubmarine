using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
	public float moveSpeed = 1f;
    public float rotSpeed = 50f;

    public bool physicsController;

	public Rigidbody2D rb;

	public Vector2 movement;

    private float lastClickTime;
    private const float doubleClick = 0.5f;

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x > 0)
        {
            transform.Rotate(-Vector3.forward * rotSpeed * Time.deltaTime);
        }


        if (movement.x < 0)
        {
            transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
        }

        /*if(Input.GetKey(KeyCode.W))
        {
            float timeSinceLastClick = Time.deltaTime - lastClickTime;

            if(timeSinceLastClick <= doubleClick)
            {
                //double click
                Debug.Log("double clickee!!");
            }
            else
            {
                //normal click
                Debug.Log("click!");
            }
        }*/

        //pew pew time

    }

    private void FixedUpdate()
    {
        Vector3 moveProj = transform.up * movement.y;

        //movement (w/ and whithout phycysssss)
        if (physicsController)
        {
            if (Input.GetKey(KeyCode.W) && Vector3.Magnitude(rb.velocity) <= 2)
            {
                rb.AddForce(new Vector2(moveProj.x, moveProj.y) * moveSpeed);
            }
            else if (Input.GetKey(KeyCode.S) && Vector3.Magnitude(rb.velocity) >= -2)
            {
                rb.AddForce(new Vector2(moveProj.x, moveProj.y) * moveSpeed);
            }
        }
        else
        {
            rb.MovePosition(rb.position + new Vector2(moveProj.x, moveProj.y) * moveSpeed * Time.deltaTime);
        }

    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        cityPopUp.SetActive(true);
        if (Input.GetKey("up"))
        {
            //Its not working, I will try and do it with GetButtonDown
            //After getting button down, make a pop up appear with some words
            Debug.Log("Entering City!");
        }
        //Debug.Log("Found a city!");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        cityPopUp.SetActive(false);
        //Debug.Log("No city found");
    }*/
}
