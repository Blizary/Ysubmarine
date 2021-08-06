using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public int maxHealth = 10;
    public int currentHealth;
    public TextMeshProUGUI playerHealth;

    public GameObject gameOver;

    //public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(2);
            playerHealth.text = "" + currentHealth.ToString() + " / " + maxHealth.ToString();

            if(currentHealth <= 0)
            {
                GameOver();
            }
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    void GameOver()
    {
        this.gameObject.SetActive(false);
        gameOver.SetActive(true);
    }
}
