using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePokemonHelper : MonoBehaviour
{
    public PokemonModel MyPokemonModel { get; set; }

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

    internal void Load(PokemonModel myPokemonModel)
    {
        MyPokemonModel = myPokemonModel;

        MaxHealth = MyPokemonModel.Health;
        Health = MyPokemonModel.Health;
        Name = myPokemonModel.PokemonType.ToString();
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
