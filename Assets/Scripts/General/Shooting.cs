using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public Transform fireBullet;
    public GameObject bulletsPrefab;

    public float bulletSpeed = 20f;

    public int ConeofVision = 6;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.up, Mathf.Infinity, ConeofVision);
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("coneVision"))
            {
                Debug.Log("pew pew!");
                Shoot();
            }
                
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletsPrefab, fireBullet.position, fireBullet.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(fireBullet.up * bulletSpeed, ForceMode2D.Impulse);
    }

    // I need to make the shooting go to the mouse position 
}
