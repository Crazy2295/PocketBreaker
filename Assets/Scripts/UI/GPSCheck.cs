using UnityEngine;

public class GPSCheck : MonoBehaviour
{
    public bool GpsOn { get; set; }
    
    public GameObject GPSErrorUI;
    
    // Start is called before the first frame update
    void Start()
    {
        Input.location.Start(5, 5);
        Input.compass.enabled = true;
        
        GpsOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.location.isEnabledByUser || 
            Input.location.status == LocationServiceStatus.Stopped ||
            Input.location.status == LocationServiceStatus.Failed)
        {
            GpsOn = false;
            GPSErrorUI.SetActive(true);
        }
        else
        {
            GpsOn = true;
            GPSErrorUI.SetActive(false);
                
            Input.location.Start(5, 5);
        }
    }
}
