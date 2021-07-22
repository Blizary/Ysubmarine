using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityShop : MonoBehaviour
{

    //Pop up
    public GameObject cityPopUp;

    //public GameObject player;

    public bool isInCity;
    public GameObject shopInventory;

    void Update()
    {
        if(isInCity)
        {
            if(shopInventory.activeInHierarchy)
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
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.CompareTag("Player"))
        {
            cityPopUp.SetActive(true);
            isInCity = true;
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
}
