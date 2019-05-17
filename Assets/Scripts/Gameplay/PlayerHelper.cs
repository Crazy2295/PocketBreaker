using UnityEngine;
using System.Collections;

public class PlayerHelper : MonoBehaviour {

    public UnitModel MyUnitModel { get; set; }
    // Use this for initialization
    void Start () {
        MyUnitModel = new UnitModel()
        {
            UnitType = UnitsEnum.DemonBladeLord,
            Health = 200,
            Damage = 10
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
