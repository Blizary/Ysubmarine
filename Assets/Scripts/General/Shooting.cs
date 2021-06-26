using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public Transform fireBullet;
    public GameObject bulletsPrefab;

    public float bulletSpeed = 20f;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletsPrefab, fireBullet.position, fireBullet.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(fireBullet.up * bulletSpeed, ForceMode2D.Impulse);
    }
}
