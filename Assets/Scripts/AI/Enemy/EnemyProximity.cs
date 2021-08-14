using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximity : MonoBehaviour
{
    public Vector3 closeWall;
    private GameObject closeWallOBJ;
    // Start is called before the first frame update
    void Start()
    {
        closeWall = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.CompareTag("Rocks"))
        {
            if(closeWall==Vector3.zero)
            {
                var collisionPoint = collision.ClosestPoint(transform.position);
                closeWall = new Vector3(collisionPoint.x, collisionPoint.y, transform.position.z);
                closeWallOBJ = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocks"))
        {
            if(closeWallOBJ== collision.gameObject)
            {
                closeWall = Vector3.zero;
                closeWallOBJ = null;
            }
        }
    }
}
