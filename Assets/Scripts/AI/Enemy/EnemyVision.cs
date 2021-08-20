using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public List<GameObject> visibleObjs;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ValidateObjs()
    {
        foreach(GameObject obj in visibleObjs)
        {
            if(obj==null)
            {
                visibleObjs.Remove(obj);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
   
        if(collision.CompareTag("Rocks")|| collision.CompareTag("Player"))
        {
            if(!visibleObjs.Contains(collision.gameObject))
            {
                visibleObjs.Add(collision.gameObject);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocks") || collision.CompareTag("Player"))
        {
            if (visibleObjs.Contains(collision.gameObject))
            {
                visibleObjs.Remove(collision.gameObject);
            }

        }
    }
}
