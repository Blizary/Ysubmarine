using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraDistanceMax;
    public float cameraDistanceMin;
    public float cameraDistance;
    public float scrollSpeed;
    public float cameraSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraZoom();
    }

    /// <summary>
    /// basic camera movement using w,a,s,d
    /// </summary>
    void CameraMove()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");
        if (Camera.current != null)
        {
            Camera.current.transform.Translate(new Vector3(xAxisValue* cameraSpeed*Time.deltaTime, yAxisValue * cameraSpeed * Time.deltaTime, 0.0f ));
        }
    }


    /// <summary>
    /// Camera zoom in and out with scroll wheel
    /// </summary>
    void CameraZoom()
    {
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
        if (Camera.current != null)
        {
            Camera.current.orthographicSize = cameraDistance;
        }
    }
}
