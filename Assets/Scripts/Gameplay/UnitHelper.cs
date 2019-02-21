using UnityEngine;
using System.Collections;
using System;

public class UnitHelper : MonoBehaviour
{

    public UnitModel MyUnitModel { get; set; }
    BattleHelper _battleHelper;

    void Start()
    {
        _battleHelper = GameObject.FindObjectOfType<BattleHelper>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!_battleHelper.IsBattle)
        {
            _battleHelper.StartBattle(MyUnitModel);
        }
        //Destroy(gameObject);
    }

    public void LoadUnit(UnitModel item)
    {
        MyUnitModel = item;

    }
}
