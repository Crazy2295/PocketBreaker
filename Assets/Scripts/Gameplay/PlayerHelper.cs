using UnityEngine;
using System.Collections;

public class PlayerHelper : MonoBehaviour {

    public PokemonModel MyPokemonModel { get; set; }
    // Use this for initialization
    void Start () {
        MyPokemonModel = new PokemonModel()
        {
            PokemonType = ETypes.knight
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
