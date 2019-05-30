using System;
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
    public GameObject allPlayers;
    public GameObject[] unitPrefabs;
    public List<UnitModel> Units { get; set; }
    public List<OtherPlayerModel> Players { get; set; }

    private GlobalStore _globalStore;

    private void Awake()
    {
        _globalStore = GameObject.FindObjectOfType<GlobalStore>();
        Units = new List<UnitModel>();
        Players = new List<OtherPlayerModel>();
    }

    private IEnumerator Start()
    {
        while (!_globalStore.GpsOn)
        {
            yield return null;
        }

        SocketHandlers();

        GetUnits();
        GetPlayers();
    }

    private void InstantiateUnit(UnitModel unit)
    {
        unit.UnitPrefab = Instantiate(unitPrefabs[unit.UnitPrefabId - 1], allUnits.transform, false);
        unit.UnitPrefab.GetComponent<SetGeolocation>().SetLocation(unit.Lat, unit.Lon);
        unit.UnitPrefab.AddComponent<MapUnitTouch>().unitModel = unit;
    }

    private void InstantiatePlayer(OtherPlayerModel player)
    {
        player.PlayerPrefab = Instantiate(unitPrefabs[0], allPlayers.transform, false);
        player.PlayerPrefab.GetComponent<SetGeolocation>().SetLocation(player.Lat, player.Lon);
        player.PlayerPrefab.AddComponent<MapUnitTouch>().unitModel = player.AsUnitModel();
    }

    public void GetUnits()
    {
        var pp = new PlayerPosition {Lat = _globalStore.PlayerPosition.x, Lon = _globalStore.PlayerPosition.y};
        _globalStore.socket.EmitJson("units_get_for_map", JsonConvert.SerializeObject(pp));
    }

    public void GetPlayers()
    {
        var pp = new PlayerPosition {Lat = _globalStore.PlayerPosition.x, Lon = _globalStore.PlayerPosition.y};
        _globalStore.socket.EmitJson("players_get_for_map", JsonConvert.SerializeObject(pp));
    }
    IEnumerator Delay(float seconds, Action func)
    {
        yield return new WaitForSeconds(seconds);
        func();
    }

    private void SocketHandlers()
    {
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

                var filteredUnits = new List<UnitModel>();
                foreach (var newUnit in newUnits)
                {
                    if (!Units.Exists(unit => newUnit.Id == unit.Id))
                        filteredUnits.Add(newUnit);
                }

                foreach (var unit in filteredUnits)
                {
                    InstantiateUnit(unit);
                    Units.Add(unit);
                }
            }
        });

        _globalStore.socket.On("units_death", (string data) =>
        {
            var unit = JsonConvert.DeserializeObject<UnitModel>((string) data);
            var unitInList = Units.FindIndex(x => x.Id == unit.Id);

            if (unitInList == -1) return;
            
            var unitPref = Units[unitInList].UnitPrefab;
            unitPref.GetComponent<Animator>().SetTrigger("Death");
            StartCoroutine(Delay(unitPref.GetComponent<Animator>().GetDurationOfClip("FallenAngle_Death"),
                () => { Destroy(unit.UnitPrefab); }
            ));
        });

        

        _globalStore.socket.On("players_get_for_map", (string data) =>
        {
            if (Players.Count == 0)
            {
                Players = JsonConvert.DeserializeObject<List<OtherPlayerModel>>((string) data);
                foreach (var player in Players)
                    InstantiatePlayer(player);
            }
            else
            {
                var newPlayers = JsonConvert.DeserializeObject<List<OtherPlayerModel>>((string) data);
                Players.RemoveAll(player =>
                {
                    if (newPlayers.Exists(newPlayer => player.Email == newPlayer.Email)) return false;
                    Destroy(player.PlayerPrefab);
                    return true;
                });

                var filteredPlayers = new List<OtherPlayerModel>();
                foreach (var newPlayer in newPlayers)
                {
                    if (!Players.Exists(player => newPlayer.Email == player.Email))
                        filteredPlayers.Add(newPlayer);
                }

                foreach (var player in filteredPlayers)
                {
                    InstantiatePlayer(player);
                    Players.Add(player);
                }
            }
        });

        _globalStore.socket.On("players_moving", (string data) =>
        {
            var player = JsonConvert.DeserializeObject<OtherPlayerModel>((string) data);
            var playerInList = Players.FindIndex(x => x.Email == player.Email);

            if (playerInList == -1)
            {
                InstantiatePlayer(player);
                Players.Add(player);
            }
            else
            {
                Players[playerInList].PlayerPrefab.GetComponent<SetGeolocation>()
                    .SetLocation(player.Lat, player.Lon);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}