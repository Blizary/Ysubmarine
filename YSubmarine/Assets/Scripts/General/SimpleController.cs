using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
	public float moveSpeed = 5f;
    public float rotSpeed = 50f;

	public Rigidbody2D rb;

	public Vector2 movement;


    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x>0)
            transform.Rotate(-Vector3.forward * rotSpeed * Time.deltaTime);

        if (movement.x < 0)
            transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);

    }

    private void FixedUpdate()
    {
        Vector3 moveProj = transform.up * movement.y;

        rb.MovePosition(rb.position + new Vector2(moveProj.x, moveProj.y) * moveSpeed * Time.deltaTime);
    }
}
