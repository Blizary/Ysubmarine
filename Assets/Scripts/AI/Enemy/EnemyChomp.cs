using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChomp : MonoBehaviour
{
    [SerializeField] private EnemyManager manager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerManager>().TakeDamage((int)manager.attackPower);
        }
    }

}
