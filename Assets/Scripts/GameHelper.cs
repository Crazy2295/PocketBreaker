using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GameHelper : MonoBehaviour
{
    const string Key = "AIzaSyBvH8rkd29W1MyusMpJJ-feHnjDvUf89Ys";
    const int WaitTime = 10;

    public bool GpsFix { get; set; }

    /// <summary>
    /// Url propertyes
    /// </summary>
    string Url = "";
    public Transform myMap;
    int _multiplier = 1; //1 для size=640x640 tile, 2 для size=1280*1280

    public Renderer maprender;
    public Text StatusText;

    Vector2 PlayerPosition =
       new Vector2(47.240342f, 38.879884f);  //Latitude, Longitude

    private double tempLat;
    private double tempLon;
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
        yield return new WaitForSeconds(5);

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
            LocationInfo loc = Input.location.lastData;
            Debug.Log("Input.location.lastData");
            yield return new WaitForSeconds(2);
            /// Только для телефонов --------------------------------------------------------
            //PlayerPosition.x = loc.latitude;
            //PlayerPosition.y = loc.longitude;

            //Set Position
            _iniRef.x = (float)((PlayerPosition.y * 20037508.34 / 180) / 100);
            _iniRef.z = (float)(System.Math.Log(System.Math.Tan((90 + PlayerPosition.x) 
                * System.Math.PI / 360)) / (System.Math.PI / 180));
            _iniRef.z = (float)((_iniRef.z * 20037508.34 / 180) / 100);
            _iniRef.y = 0;

            ///Все гуд
            GpsFix = true;
            LoadMap(PlayerPosition);
        }

        InvokeRepeating("UpdateMyPosition", 1, 0.5f);
        InvokeRepeating("UpdateMap", 1, 5f);
        //  InvokeRepeating("Orientate", 1, 0.05f);
    }

    public Transform Player;
    private bool _mapLoaded;
    public bool UpdatedPosition { get; set; }

    private Vector3 _newUserPos;
    void UpdateMap()
    {
        if (GpsFix && _mapLoaded && UpdatedPosition)
            LoadMap(PlayerPosition);
    }

    const float DistanceMapUpdate = 2;
    Vector2 _lastPlayerPosition;
    void UpdateMyPosition()
    {
        if (GpsFix)
        {
            LocationInfo loc = Input.location.lastData;
            /// Только для телефонов --------------------------------------------------------
            //PlayerPosition.x = loc.latitude;
            //PlayerPosition.y = loc.longitude;

            if (Vector3.Distance(_lastPlayerPosition, Player.position) > DistanceMapUpdate)
                UpdatedPosition = true;

            if (UpdatedPosition)
            {
                _newUserPos.x = (float)(((PlayerPosition.y * 20037508.34 / 180) / 100) - _iniRef.x);
                _newUserPos.z = (float)(System.Math.Log(System.Math.Tan((90 + PlayerPosition.x)
                    * System.Math.PI / 360)) / (System.Math.PI / 180));
                _newUserPos.z = (float)(((_newUserPos.z * 20037508.34 / 180) / 100) - _iniRef.z);

                Player.position = _newUserPos;

                UpdatedPosition = false;
            }
        }
    }

    void Orientate()
    {
        //if (!simGPS && gpsFix)
        //{
        //    heading = Input.compass.trueHeading;
        //}
        //else {
        //    heading = user.eulerAngles.y;
        //}
    }




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
            "&style=feature:transit%7Cvisibility:off" + "&key=" + Key;

        _lastPlayerPosition = Player.position;
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

        tempLat = PlayerPosition.x;
        tempLon = PlayerPosition.y;

        maprender.enabled = true;
        ReSet();
        ReScale();

        _mapLoaded = true;
    }

    void ReSet()
    {
        Vector3 newPosition = new Vector3();
        newPosition.x = (float)(((tempLon * 20037508.34 / 180) / 100) - _iniRef.x);
        newPosition.z = (float)(System.Math.Log(System.Math.Tan((90 + tempLat) 
            * System.Math.PI / 360)) / (System.Math.PI / 180));
        newPosition.z = (float)(((newPosition.z * 20037508.34 / 180) / 100) - _iniRef.z);
        transform.position = newPosition;
    }

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

    }
}
