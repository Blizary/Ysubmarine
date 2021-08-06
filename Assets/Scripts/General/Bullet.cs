using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public GameObject hitEffect;


    private void Update()
    {
        Movement();
    }

    public void CreateBullet()
    {

    }

    void Movement()
    {

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Debug.Log(collision.gameObject.name);
        Destroy(effect, 0.5f);
        Destroy(gameObject);
    }



}
