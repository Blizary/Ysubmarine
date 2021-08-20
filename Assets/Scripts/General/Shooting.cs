using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shooting : MonoBehaviour
{

    public Transform fireBullet;
    public GameObject bulletsPrefab;
    public LayerMask coneLayer;

    public float bulletSpeed = 20f;

    public int maxAmmo = 6;
    public int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    public TextMeshProUGUI bulletText;

    public int ConeofVision = 6;

    private void Start()
    {
        currentAmmo = maxAmmo;
        
    }
    // Update is called once per frame
    void Update()
    {


        if (isReloading)
            return;

        if(currentAmmo <= 0)
        {
            bulletText.text = "" + currentAmmo.ToString() + " / " + maxAmmo.ToString();
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.up, Mathf.Infinity, coneLayer);
            if(hit)
            {
                if (hit.collider.gameObject.CompareTag("coneVision"))
                {
                    Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
            
            bulletText.text = "" + currentAmmo.ToString() + " / " + maxAmmo.ToString();
            //Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        bulletText.text = "" + currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

    void Shoot(Vector3 hitPoint)
    {
        currentAmmo--;

        GameObject bullet = Instantiate(bulletsPrefab, fireBullet.position, fireBullet.rotation);//Spawn bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();//get bullet rigidbody
        Vector3 hitpointFix = new Vector3(hitPoint.x, hitPoint.y, fireBullet.position.z);// make sure the z is the same as plane where the bullet is spawned
        //Debug.DrawLine(fireBullet.transform.position, hitpointFix, Color.red,100000F);
        Vector3 dir = (hitpointFix - fireBullet.transform.position).normalized;// get the direction of movement by using the starting point and the mouse point
        bullet.transform.up = hitpointFix - bullet.transform.position;//rotate the bullet so it faces where it is moving

        rb.AddForce(dir*bulletSpeed, ForceMode2D.Impulse);//apply force to the bullet in the direction wanted
        
        //bullet.transform.rotation = Quaternion.LookRotation(rb.velocity);


    }

    // I need to make the shooting go to the mouse position 
}
