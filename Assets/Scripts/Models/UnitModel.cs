using UnityEngine;
using System.Collections;

public class UnitModel
{
    public int Id { get; set; }
    public ETypes UnitType { get; set; }

    public float Lat { get; set; }
    public float Lon { get; set; }
    public float Orint { get; set; }

    public int Exp { get; set; }
    public int Damage { get; set; }
    public int Health { get; set; }
}
