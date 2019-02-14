using UnityEngine;
using System.Collections;

public class PlayerHelper : MonoBehaviour {

    public PokemonModel MyPokemonModel { get; set; }
    // Use this for initialization
    void Start () {
        MyPokemonModel = new PokemonModel()
        {
            PokemonType = ETypes.knight,
            Health = 200,
            Damage = 10
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
