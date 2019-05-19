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

    public bool IsAugmented
    {
        get { return PlayerPrefs.GetInt("IsAugmented") == 1; }
        set
        {
            PlayerPrefs.SetInt("IsAugmented", value ? 1 : 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ServerProtocol = "http://";
        ServerUri = "127.0.0.1:5000";

        IsMainScreen = false;
        IsMenuMode = true;
        GpsOn = true;
        SocketSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
