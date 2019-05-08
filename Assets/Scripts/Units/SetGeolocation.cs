using UnityEngine;
using System.Collections;
using System;

public class SetGeolocation : MonoBehaviour
{

    public float lat;
    public float lon;
    public float orientation;

    private float _initX;
    private float _initZ;

    private bool _gpsOn;
    
    private MapHelper _mapHelper;
    private GPSCheck _gpsCheck;
    
    void Awake()
    {
        _mapHelper = GameObject.FindObjectOfType<MapHelper>();
        _gpsCheck = GameObject.FindObjectOfType<GPSCheck>();
        _gpsOn = _gpsCheck.GpsOn;
    }

    IEnumerator Start()
    {
        while (!_gpsOn)
        {
            _gpsOn = _gpsCheck.GpsOn;
            yield return null;
        }
        _initX = _mapHelper.IniRef.x;
        _initZ = _mapHelper.IniRef.z;

        yield return new WaitForSeconds(1);
        GeoLocation();
    }

    void GeoLocation()
    {
        Vector3 pos = transform.position;
        pos.x = (float)(((lon * 20037508.34) / 18000) - _initX);
        pos.z = (float)(((Mathf.Log(Mathf.Tan((90 + lat) * Mathf.PI / 360))
            / (Mathf.PI / 180)) * 1113.19490777778) - _initZ);
        pos.y = 0;
        transform.position = pos;
        Vector3 eAngles = transform.eulerAngles;
        eAngles.y = orientation;
        transform.eulerAngles = eAngles;
    }

    public void SetLocation(float latitude, float longitude, float newOrientation)
    {
        lat = latitude;
        lon = longitude;
        orientation = newOrientation;
        GeoLocation();
    }
}
