using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{

    public Transform fireBullet;
    public GameObject bulletsPrefab;

    public float bulletSpeed = 20f;

    public int maxAmmo = 6;
    public int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    Text bulletText;

    public int ConeofVision = 6;

    private void Start()
    {
        currentAmmo = maxAmmo;

        bulletText = GetComponent<Text>();
        bulletText.text = currentAmmo.ToString(currentAmmo + " / 6");
    }
    // Update is called once per frame
    void Update()
    {
        if (isReloading)
            return;

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            /*RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.up, Mathf.Infinity, ConeofVision);
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("coneVision"))
            {
                Debug.Log("pew pew!");
                Shoot();
            }*/
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        currentAmmo--;

        GameObject bullet = Instantiate(bulletsPrefab, fireBullet.position, fireBullet.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(fireBullet.up * bulletSpeed, ForceMode2D.Impulse);
    }

    // I need to make the shooting go to the mouse position 
}
