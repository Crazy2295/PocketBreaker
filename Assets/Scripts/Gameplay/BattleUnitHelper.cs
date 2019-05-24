using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitHelper : MonoBehaviour
{
    public UnitModel MyUnitModel { get; set; }

    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public string Name { get; set; }

    public bool IsDead { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Load(UnitModel myUnitModel)
    {
        MyUnitModel = myUnitModel;

        MaxHealth = 100;// TODO load MyUnitModel.Hp;
        Health = 100; //MyUnitModel.Hp;
//        Name = myUnitModel.UnitType.ToString();
        Name = "AI";
    }

    internal void NewHp(int hp)
    {
        if (hp <= 0)
        {
            Health = 0;
            IsDead = true;
        }

        Health = hp;
    }

    internal void TakeDamage(int damage)
    {
        int health = Health - damage;

        if (health <= 0)
        {
            Health = 0;
            IsDead = true;
        }

        Health = health;
    }
}
