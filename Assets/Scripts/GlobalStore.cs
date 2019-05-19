using socket.io;
using UnityEngine;

public class GlobalStore : MonoBehaviour
{
    public string ServerProtocol { get; private set; }
    public string ServerUri { get; private set; }
    public Socket socket;
    public bool GpsOn { get; set; }
    public bool SocketSet { get; set; }
    
    public bool IsMainScreen { get; set; }
    public bool IsMenuMode { get; set; }
    public bool IsAugmented { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        ServerProtocol = "http://";
        ServerUri = "127.0.0.1:5000";

        IsMainScreen = false;
        IsMenuMode = true;
        GpsOn = true;
        IsAugmented = true;
        SocketSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
