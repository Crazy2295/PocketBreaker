using UnityEngine;

public class OtherPlayerModel
{
    public string Email { get; set; }
    public string Name { get; set; }
    public float Lat { get; set; }
    public float Lon { get; set; }
    
    public GameObject PlayerPrefab { get; set; }

    public UnitModel AsUnitModel()
    {
        return new UnitModel
        {
            Id = -1,
            UnitPrefabId = 1,
            UnitPrefab = PlayerPrefab,
            Hp = 100,
            Lat = Lat,
            Lon = Lon,
            UnitModification = Email
        };
    }
}
