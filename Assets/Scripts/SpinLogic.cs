using UnityEngine;
using System.Collections;
 
public class SpinLogic : MonoBehaviour {
 
    float f_lastX = 0.0f;
    float f_lastY = 0.0f;
    
    float f_difX = 0.5f;
    float f_difY = 0.5f;
    
    float f_curX = 0.0f;
    float f_curY = 0.0f;
    
    int i_direction = 1;
 
    // Use this for initialization
    void Start () 
    {
         
    }
     
    // Update is called once per frame
    void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            f_difX = 0.0f;
            f_difY = 0.0f;
        }
        else if (Input.GetMouseButton(0))
        {
            f_curX = Input.GetAxis("Mouse X");
            f_curY = Input.GetAxis("Mouse Y");
            
            f_difX = Mathf.Abs(f_lastX - f_curX);
            f_difY = Mathf.Abs(f_lastY - f_curY);
 
            if (f_lastX < f_curX)
            {
                i_direction = 1;
                transform.Rotate(Vector3.up, Screen.height/2 < Input.mousePosition.y ? -f_difX : f_difX);
            }
 
            if (f_lastX > f_curX)
            {
                i_direction = -1;
                transform.Rotate(Vector3.up, Screen.height/2 < Input.mousePosition.y ? f_difX : -f_difX);
            }
            
            
            if (f_lastY > f_curY)
            {
                i_direction = 1;
                transform.Rotate(Vector3.up, Screen.width/2 < Input.mousePosition.x ? -f_difY : f_difY);
            }
 
            if (f_lastY < f_curY)
            {
                i_direction = -1;
                transform.Rotate(Vector3.up, Screen.width/2 < Input.mousePosition.x ? f_difY : -f_difY);
            }
 
            f_lastX = -f_curX;
            f_lastY = -f_curY;

            f_difX = f_difX > f_difY ? f_difX : f_difY;
        }
        else 
        {
            if (f_difX > 0.1f) 
                f_difX -= 0.09f;
            else
                f_difX = 0.0f;
            
            transform.Rotate(Vector3.up, f_difX * i_direction);
        }
    }
}