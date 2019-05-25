using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Units;

public class LoadUnitData : MonoBehaviour
{
    public GameObject allUnits;
    public GameObject[] unitPrefabs;
    public List<UnitModel> Units { get; set; }

    private GlobalStore _globalStore;

    private void Awake()
    {
        _globalStore = GameObject.FindObjectOfType<GlobalStore>();
        Units = new List<UnitModel>();
    }

    public void RequestNewSetUnits()
    {
        var pp = new PlayerPosition {Lat = _globalStore.PlayerPosition.x, Lon = _globalStore.PlayerPosition.y};
        _globalStore.socket.EmitJson("units_get_for_map", JsonConvert.SerializeObject(pp));

        
    }
    private IEnumerator Start()
    {
        while (!_globalStore.GpsOn)
        {
            yield return null;
        }

        _globalStore.socket.On("units_get_for_map", (string data) =>
        {
            Units = JsonConvert.DeserializeObject<List<UnitModel>>((string) data);
            foreach (var unit in Units)
            {
                unit.UnitPrefab = Instantiate(unitPrefabs[unit.UnitPrefabId - 1], allUnits.transform, false);
                unit.UnitPrefab.GetComponent<SetGeolocation>().SetLocation(unit.Lat, unit.Lon);
                unit.UnitPrefab.AddComponent<MapUnitTouch>().unitModel = unit;
            }
        });
        RequestNewSetUnits();

    }

    // Update is called once per frame
    void Update()
    {
    }
}
