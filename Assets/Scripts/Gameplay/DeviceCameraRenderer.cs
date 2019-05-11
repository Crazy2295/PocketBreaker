using UnityEngine;
using UnityEngine.Serialization;

public class DeviceCameraRenderer: MonoBehaviour
{
    public WebCamTexture webCamera;
    public GameObject plane;
    
    // initialization
    void Start()
    {
        Debug.Log("Script has been started");
        if (plane == null) plane = GameObject.FindWithTag("Background"); 
        webCamera = new WebCamTexture();
        plane.GetComponent<Renderer>().material.mainTexture = webCamera;
        webCamera.Play();
    }
}