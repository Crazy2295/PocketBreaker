using UnityEngine;
using UnityEngine.Serialization;

public class DeviceCameraRenderer: MonoBehaviour
{
    public WebCamTexture webCamera;
    //public GameObject plane;
    
    // initialization
    void Start()
    {
        Debug.Log("Script has been started");
        var plane = gameObject;//GameObject.FindWithTag("Background"); //gameObject;
        webCamera = new WebCamTexture();
        plane.GetComponent<Renderer>().material.mainTexture = webCamera;
        webCamera.Play();
    }
    void OnDestroy()
    {
        if (webCamera != null) webCamera.Stop();
    }
}