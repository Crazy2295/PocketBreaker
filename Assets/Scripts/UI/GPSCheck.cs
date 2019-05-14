﻿using UnityEngine;

public class GPSCheck : MonoBehaviour
{
    public GameObject GPSErrorUI;
    
    private GlobalStore _globalStore;

    private void Awake()
    {
        _globalStore = GameObject.FindObjectOfType<GlobalStore>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Input.location.Start(5, 5);
        Input.compass.enabled = true;
        _globalStore.GpsOn = true;
    }

    // Update is called once per frame
    void Update()
    {
//        if (_globalStore.IsMenuMode) 
//            return;
//        
//        if (!Input.location.isEnabledByUser ||
//            Input.location.status == LocationServiceStatus.Stopped ||
//            Input.location.status == LocationServiceStatus.Failed)
//        {
//            _globalStore.GpsOn = false;
//            GPSErrorUI.SetActive(true);
//        }
//        else
//        {
//            _globalStore.GpsOn = true;
//            GPSErrorUI.SetActive(false);
//
//            Input.location.Start(5, 5);
//        }
    }
}
