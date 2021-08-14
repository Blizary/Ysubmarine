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

    //public GameObject gameOver;

    //public Animator deathAnim;

    //public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        //deathAnim.SetBool("isDead", false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(2);
            playerHealth.text = "" + currentHealth.ToString() + " / " + maxHealth.ToString();

            if(currentHealth == 0)
            {
                //deathAnim.SetBool("isDead", true);
                GameOver();
            }
        }
    }

    void TakeDamage(int damage)
    {
        if(currentHealth > 0)
        {
            currentHealth -= damage;
        }
        else if(currentHealth <= 0)
        {
            //stop doing damage
            currentHealth = 0;
        }
    }

    void GameOver()
    {
        //this.gameObject.SetActive(false);
        //gameOver.SetActive(true);
    }
}
