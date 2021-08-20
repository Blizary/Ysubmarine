using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public float maxHealth = 10;
    public float currentHealth;
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
            AddLife(5);
            //TakeDamage(2);

            if(currentHealth == 0)
            {
                GameOver();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
        }
        else if(currentHealth <= 0)
        {
            //stop doing damage
            currentHealth = 0;
        }
        playerHealth.text = "" + currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    public void AddLife(float _life)
    {

        currentHealth += _life;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        playerHealth.text = "" + currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    void GameOver()
    {
        //this.gameObject.SetActive(false);
        //gameOver.SetActive(true);
    }
}
