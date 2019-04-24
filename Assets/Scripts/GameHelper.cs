using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GameHelper : MonoBehaviour
{
    const string KEY = "AIzaSyBvH8rkd29W1MyusMpJJ-feHnjDvUf89Ys";
    const int WaitTime = 10;

    public bool GpsFix { get; set; }
    
    string Url = "";
    public Transform myMap;
    int _multiplier = 1; //1 для size=640x640 tile, 2 для size=1280*1280

    public Renderer maprender;
    public Text StatusText;

    private Vector2 PlayerPosition =
        new Vector2(47.240557f, 38.883231f);  //Latitude, Longitude

    private Vector3 _iniRef;

    public Vector3 IniRef
    {
        get { return _iniRef; }
        set { _iniRef = value; }
    }

    int _zoom = 17;
    string _mapType = "terrain";
    int _mapScale = 1;
    int _mapSize = 640;

    private LocationInfo _loc;
    float _download = 0;
    public string Status { set { StatusText.text = value;  } }


    int _counter;
    IEnumerator Start()
    {
        Input.location.Start(5, 5);
        Input.compass.enabled = true;
        Status = "Initializing Location Services..";
       
        // Wait until service initializes
        while (Input.location.status == 
            LocationServiceStatus.Initializing && 
            _counter < WaitTime)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Wait " + _counter);
            Status = "Wait " + _counter;
            _counter++;
        }

        if (_counter >= WaitTime)
        {
            Status = "_counter >= WaitTime";
            yield return new WaitForSeconds(4);
            Application.Quit();
            yield return null;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Status = "Input.location.status == LocationServiceStatus.Failed";
            yield return new WaitForSeconds(4);
            Application.Quit();
            yield return null;
        }
        else
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                LocationInfo loc = Input.location.lastData;
                Debug.Log("First Input.location.lastData");
                Status = "First Input.location.lastData";
                yield return new WaitForSeconds(2);

                // Only for mobile --------------------
                PlayerPosition.x = loc.latitude;
                PlayerPosition.y = loc.longitude;
            }

            //Set Position
            _iniRef = PositionHelper(PlayerPosition);

            GpsFix = true;
            yield return new WaitForSeconds(2);
            LoadMap(PlayerPosition);
        }

        InvokeRepeating("UpdateMyPosition", 1, 0.5f);
        InvokeRepeating("UpdateMap", 1, 3f);
    }

    public Transform Player;
    private bool _mapLoaded;
    private bool UpdatedPosition { get; set; }

    void UpdateMap()
    {
        if (GpsFix && _mapLoaded && UpdatedPosition)
            LoadMap(PlayerPosition);
    }

    const float DistanceMapUpdate = 2;
    Vector2 _lastMapCenter;
    private Vector3 _positionForLerp;
    
    void UpdateMyPosition()
    {
        if (GpsFix && Input.location.status == LocationServiceStatus.Running)
        //if (GpsFix && Input.location.status != LocationServiceStatus.Stopped)
        {

            LocationInfo loc = Input.location.lastData;

            // Only for mobile --------------------
            PlayerPosition.x = loc.latitude;
            PlayerPosition.y = loc.longitude;

            if (Vector3.Distance(_lastMapCenter, Player.position) > DistanceMapUpdate)
                UpdatedPosition = true;

            _positionForLerp = PositionHelper(PlayerPosition, _iniRef);
        }
    }


    /// <summary>
    /// Download map from Google
    /// </summary>
    /// <param name="playerPosition">GPS coordinates (x,y)</param>
    private void LoadMap(Vector2 playerPosition)
    {
        _mapLoaded = false;
        Url = "https://maps.googleapis.com/maps/api/staticmap?center=" +
            PlayerPosition.x.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture) +
            "," + PlayerPosition.y.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture) +
            "&zoom=" + _zoom + "&size=" + _mapSize + "x" + _mapSize + "&scale=" + _mapScale + "&language=ru" + "&type=" + _mapType + 
            "&style=feature:all|element:labels|visibility:off&style=feature:landscape.man_made%7Celement:geometry%7Cvisibility:off" +
            "&style=feature:road%7Ccolor:0xacaca4&style=feature:road.local%7Ccolor:0x9b7653&style=feature:poi%7Cvisibility:off" +
            "&style=feature:landscape.natural%7Celement:geometry%7Ccolor:0x008000&style=feature:water%7Ccolor:0x003F87" +
            "&style=feature:transit%7Cvisibility:off" + "&key=" + KEY;

        _lastMapCenter = Player.position;
        UpdatedPosition = false;

        StartCoroutine(LoadMap());
    }

    private IEnumerator LoadMap()
    {
        WWW www = new WWW(Url);

        while (!www.isDone)
        {
            _download = (www.progress);
            Debug.Log("Updating map " + System.Math.Round(_download * 100) + " %");
            Status = "Updating map " + System.Math.Round(_download * 100) + " %";
            yield return null;
        }

        if (www.error == null)
        {
            Debug.Log("Map Ready!");
            Status = "Map Ready!";
            yield return new WaitForSeconds(0.5f);
            maprender.material.mainTexture = null;
            Texture2D tmp;
            tmp = new Texture2D(_mapSize, _mapSize, TextureFormat.RGB24, false);
            maprender.material.mainTexture = tmp;
            www.LoadImageIntoTexture(tmp);
        }
        else {
            print("Map Error:" + www.error);
            Status = "Map Error:" + www.error;
            yield return new WaitForSeconds(1);
            maprender.material.mainTexture = null;
        }
        
        maprender.enabled = true;
        ReSet();
        ReScale();

        _mapLoaded = true;
    }

    /// <summary>
    /// Reset player position for new map
    /// </summary>
    void ReSet() 
    {
        transform.position = PositionHelper(PlayerPosition, _iniRef);
    }

    /// <summary>
    /// Rescale map for better display
    /// </summary>
    void ReScale()
    {
        Vector3 newScale = myMap.localScale;
        newScale.x = (float)(_multiplier * 100532.244f / (Mathf.Pow(2, _zoom)));
        newScale.z = newScale.x;
        myMap.localScale = newScale;
    }


    // Update is called once per frame
    void Update()
    {
        if (Player.position != _positionForLerp)
            Player.position = Vector3.Lerp(Player.position, _positionForLerp, 0.15f);
    }

    /// <summary>
    /// Transform position GPS coordinates to Unity position
    /// </summary>
    /// <param name="position">New coordinates</param>
    /// <param name="previous">Previous coordinates</param>
    /// <returns>Correct new coordinates values</returns>
    Vector3 PositionHelper (Vector2 position, Vector3 previous = new Vector3())
    {
        Vector3 newPosition = new Vector3();

        newPosition.x = (float)((position.y * 20037508.34 / 180) / 100);
        if (previous.x != 0)
            newPosition.x = (float)(newPosition.x - previous.x);

        newPosition.z = (float)(System.Math.Log(System.Math.Tan((90 + position.x)
            * System.Math.PI / 360)) / (System.Math.PI / 180));
        newPosition.z = (float)((newPosition.z * 20037508.34 / 180) / 100);
        if (previous.z != 0)
            newPosition.z = (float)(newPosition.z - previous.z);

        newPosition.y = 0;

        return newPosition;
    }
}
