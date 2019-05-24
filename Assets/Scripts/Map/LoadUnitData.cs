using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
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

    private IEnumerator Start()
    {
        while (!_globalStore.GpsOn)
        {
            yield return null;
        }

        GetUnits();

        _globalStore.socket.On("units_get_for_map", (string data) =>
        {
            if (Units.Count == 0)
            {
                Units = JsonConvert.DeserializeObject<List<UnitModel>>((string) data);
                foreach (var unit in Units)
                    InstantiateUnit(unit);
            }
            else
            {
                var newUnits = JsonConvert.DeserializeObject<List<UnitModel>>((string) data);
                Units.RemoveAll(unit =>
                {
                    if (newUnits.Exists(newUnit => unit.Id == newUnit.Id)) return false;
                    Destroy(unit.UnitPrefab);
                    return true;

                });

                var exceptUnits = Units.Except(newUnits).ToList();
                foreach (var unit in exceptUnits)
                {
                    InstantiateUnit(unit);
                    Units.Add(unit);
                }
            }
        });
    }

    private void InstantiateUnit(UnitModel unit)
    {
        unit.UnitPrefab = Instantiate(unitPrefabs[unit.UnitPrefabId - 1], allUnits.transform, false);
        unit.UnitPrefab.GetComponent<SetGeolocation>().SetLocation(unit.Lat, unit.Lon);
        unit.UnitPrefab.AddComponent<MapUnitTouch>().unitModel = unit;
    }

    public void GetUnits()
    {
        var pp = new PlayerPosition {Lat = _globalStore.PlayerPosition.x, Lon = _globalStore.PlayerPosition.y};
        _globalStore.socket.EmitJson("units_get_for_map", JsonConvert.SerializeObject(pp));
    }

    // Update is called once per frame
    void Update()
    {
    }
}