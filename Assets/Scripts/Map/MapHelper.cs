using UnityEngine;
using System.Collections;

public class MapHelper : MonoBehaviour
{
    private const string Key = "AIzaSyBvH8rkd29W1MyusMpJJ-feHnjDvUf89Ys";

    private string _url = "";
    private const int Multiplier = 1; //1 для size=640x640 tile, 2 для size=1280*1280

    public Transform myMap;
    public Renderer mapRender;
    public Transform player;

    private Vector2 _playerPosition =
        new Vector2(47.240557f, 38.883231f); //Latitude, Longitude

    private Vector3 _iniRef;

    public Vector3 IniRef
    {
        get { return _iniRef; }
        set { _iniRef = value; }
    }

    private const int Zoom = 17;
    private const string MapType = "terrain";
    private const int MapScale = 1;
    private const int MapSize = 640;
    
    private GPSCheck _gpsCheck;
    void Awake()
    {
        _gpsCheck = GameObject.FindObjectOfType<GPSCheck>();
    }
    
    void Start()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo loc = Input.location.lastData;

            _playerPosition.x = loc.latitude;
            _playerPosition.y = loc.longitude;
        }

        //Set Position
        _iniRef = PositionHelper(_playerPosition);

        LoadMap(_playerPosition);

        InvokeRepeating("UpdateMyPosition", 1, 0.5f);
        InvokeRepeating("UpdateMap", 1, 3f);
    }

    private bool _mapLoaded;
    private bool UpdatedPosition { get; set; }

    void UpdateMap()
    {
        if (_gpsCheck.GpsOn && _mapLoaded && UpdatedPosition)
            LoadMap(_playerPosition);
    }

    private const float DistanceMapUpdate = 2;
    private Vector2 _lastMapCenter;
    private Vector3 _positionForLerp;

    void UpdateMyPosition()
    {
        if (_gpsCheck.GpsOn && Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo loc = Input.location.lastData;

            _playerPosition.x = loc.latitude;
            _playerPosition.y = loc.longitude;

            if (Vector3.Distance(_lastMapCenter, player.position) > DistanceMapUpdate)
                UpdatedPosition = true;

            _positionForLerp = PositionHelper(_playerPosition, _iniRef);
        }
    }


    /// <summary>
    /// Download map from Google
    /// </summary>
    /// <param name="playerPosition">GPS coordinates (x,y)</param>
    private void LoadMap(Vector2 playerPosition)
    {
        _mapLoaded = false;
        _url = "https://maps.googleapis.com/maps/api/staticmap?center=" +
              _playerPosition.x.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture) +
              "," + _playerPosition.y.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture) +
              "&zoom=" + Zoom + "&size=" + MapSize + "x" + MapSize + "&scale=" + MapScale + "&language=ru" +
              "&type=" + MapType +
              "&style=feature:all|element:labels|visibility:off&style=feature:landscape.man_made%7Celement:geometry%7Cvisibility:off" +
              "&style=feature:road%7Ccolor:0xacaca4&style=feature:road.local%7Ccolor:0x9b7653&style=feature:poi%7Cvisibility:off" +
              "&style=feature:landscape.natural%7Celement:geometry%7Ccolor:0x008000&style=feature:water%7Ccolor:0x003F87" +
              "&style=feature:transit%7Cvisibility:off" + "&key=" + Key;

        _lastMapCenter = player.position;
        UpdatedPosition = false;

        StartCoroutine(LoadMap());
    }

    float _download = 0;
    
    private IEnumerator LoadMap()
    {
        WWW www = new WWW(_url);

        while (!www.isDone)
        {
            _download = (www.progress);
            yield return null;
        }

        if (www.error == null)
        {
            Debug.Log("Map Ready!");
            yield return new WaitForSeconds(0.5f);
            mapRender.material.mainTexture = null;
            Texture2D tmp;
            tmp = new Texture2D(MapSize, MapSize, TextureFormat.RGB24, false);
            mapRender.material.mainTexture = tmp;
            www.LoadImageIntoTexture(tmp);
        }
        else
        {
            Debug.Log("Map Error: " + www.error);
            yield return new WaitForSeconds(1);
            mapRender.material.mainTexture = null;
        }

        mapRender.enabled = true;
        ReSet();
        ReScale();

        _mapLoaded = true;
    }

    /// <summary>
    /// Reset player position for new map
    /// </summary>
    void ReSet()
    {
        transform.position = PositionHelper(_playerPosition, _iniRef);
    }

    /// <summary>
    /// Rescale map for better display
    /// </summary>
    void ReScale()
    {
        Vector3 newScale = myMap.localScale;
        newScale.x = (float) (Multiplier * 100532.244f / (Mathf.Pow(2, Zoom)));
        newScale.z = newScale.x;
        myMap.localScale = newScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position != _positionForLerp)
            player.position = Vector3.Lerp(player.position, _positionForLerp, 0.15f);
    }

    /// <summary>
    /// Transform position GPS coordinates to Unity position
    /// </summary>
    /// <param name="position">New coordinates</param>
    /// <param name="previous">Previous coordinates</param>
    /// <returns>Correct new coordinates values</returns>
    Vector3 PositionHelper(Vector2 position, Vector3 previous = new Vector3())
    {
        Vector3 newPosition = new Vector3();

        newPosition.x = (float) ((position.y * 20037508.34 / 180) / 100);
        if (previous.x != 0)
            newPosition.x = (float) (newPosition.x - previous.x);

        newPosition.z = (float) (System.Math.Log(System.Math.Tan((90 + position.x)
                                                                 * System.Math.PI / 360)) / (System.Math.PI / 180));
        newPosition.z = (float) ((newPosition.z * 20037508.34 / 180) / 100);
        if (previous.z != 0)
            newPosition.z = (float) (newPosition.z - previous.z);

        newPosition.y = 0;

        return newPosition;
    }
}