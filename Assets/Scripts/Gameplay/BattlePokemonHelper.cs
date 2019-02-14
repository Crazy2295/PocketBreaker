using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePokemonHelper : MonoBehaviour
{
    public PokemonModel MyPokemonModel { get; set; }
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
    }
}
