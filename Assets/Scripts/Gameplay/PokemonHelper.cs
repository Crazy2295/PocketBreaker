using UnityEngine;
using System.Collections;
using System;

public class PokemonHelper : MonoBehaviour
{

    public PokemonModel MyPokemonModel { get; set; }
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
            _battleHelper.StartBattle(MyPokemonModel);
        }
        //Destroy(gameObject);
    }

    public void LoadPokemon(PokemonModel item)
    {
        MyPokemonModel = item;

    }
}
