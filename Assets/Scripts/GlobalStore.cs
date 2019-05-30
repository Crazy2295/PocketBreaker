using socket.io;
using UnityEngine;

public class GlobalStore : MonoBehaviour
{
    public Vector2 PlayerPosition { get; set; }
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
    private void Start()
    {
        PlayerPosition = new Vector2(47.24055f, 38.88323f);
        
        ServerProtocol = "http://";
        ServerUri = "pocketbreak.sytes.net";

        IsMainScreen = false;
        IsMenuMode = true;
        GpsOn = false;
        SocketSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
