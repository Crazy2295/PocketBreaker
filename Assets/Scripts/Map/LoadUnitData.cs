using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Units;

public class LoadUnitData : MonoBehaviour
{
    public GameObject[] UnitPrefabs;
    public List<UnitModel> Units { get; set; }

    private GlobalStore _globalStore;

    private void Awake()
    {
        _globalStore = GameObject.FindObjectOfType<GlobalStore>();
        Units = new List<UnitModel>();
    }

    private IEnumerator Start()
    {
        while (!_globalStore.GpsOn)
        {
            yield return null;
        }


        var pp = new PlayerPosition {Lat = _globalStore.PlayerPosition.x, Lon = _globalStore.PlayerPosition.y};
        _globalStore.socket.EmitJson("units_get_for_map", JsonConvert.SerializeObject(pp));

        _globalStore.socket.On("units_get_for_map", (string data) =>
        {
            Units = JsonConvert.DeserializeObject<List<UnitModel>>((string) data);
            foreach (var unit in Units)
            {
                unit.UnitPrefab = Instantiate(UnitPrefabs[unit.UnitPrefabId - 1]);
                unit.UnitPrefab.GetComponent<SetGeolocation>().SetLocation(unit.Lat, unit.Lon);
                unit.UnitPrefab.AddComponent<MapUnitTouch>().unitModel = unit;
            }

        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
