using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class UnitModel
{
    public int Id { get; set; }
    [JsonProperty("spawn_id")]
    public int SpawnId { get; set; }
    public GameObject UnitPrefab { get; set; }
    [JsonProperty("unit_model")]
    public int UnitPrefabId { get; set; }
    public float Lat { get; set; }
    public float Lon { get; set; }
    [JsonProperty("unit_modification")]
    public string UnitModification { get; set; }
    public int Hp { get; set; }
    public int Exp { get; set; }
    [JsonProperty("additional_damage")]
    public int AdditionalDamage { get; set; }
}
