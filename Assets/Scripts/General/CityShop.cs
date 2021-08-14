using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityShop : MonoBehaviour
{

    //Pop up
    public GameObject cityPopUp;

    public GameObject player;

    public bool isInCity;
    public GameObject shopInventory;

    public GameObject upgradeSub;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if(isInCity)
        {
            Debug.Log("No shooting!");
            player.GetComponent<Shooting>().enabled = false;

            if (shopInventory.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Thank you! Please come again");
                    shopInventory.SetActive(false);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Welcome to our shop!");
                    shopInventory.SetActive(true);
                }
            }
        }

        //Debug.Log("You can shoot!");
        player.GetComponent<Shooting>().enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.CompareTag("Player"))
        {
            cityPopUp.SetActive(true);
            isInCity = true;
            UpgradingSub();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            cityPopUp.SetActive(false);
            isInCity = false;
            shopInventory.SetActive(false);
        }
    }

    public void UpgradingSub()
    {
        if(upgradeSub.gameObject.CompareTag("Defense"))
        {
            Debug.Log("Your life increased!");

        }
    }
}
