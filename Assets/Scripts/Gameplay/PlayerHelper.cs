using UnityEngine;
using System.Collections;

public class PlayerHelper : MonoBehaviour {

    public UnitModel MyUnitModel { get; set; }

    private LoadUnitData _loadUnitData;

    private void Awake()
    {
	    _loadUnitData = GameObject.FindObjectOfType<LoadUnitData>();
    }

    void Start () {
        MyUnitModel = new UnitModel()
        {
	        UnitPrefabId = 0,
            Hp = 100,
            AdditionalDamage = 10
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
