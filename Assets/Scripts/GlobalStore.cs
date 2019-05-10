using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStore : MonoBehaviour
{
    public string ServerProtocol { get; private set; }
    public string ServerUri { get; private set; }
    public bool GpsOn { get; set; }
    
    public bool IsMainScreen { get; set; }
    public bool IsMenuMode { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        ServerProtocol = "http://";
        ServerUri = "127.0.0.1:5000";

        IsMainScreen = false;
        IsMenuMode = true;
        GpsOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
