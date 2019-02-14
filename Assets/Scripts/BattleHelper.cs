using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHelper : MonoBehaviour
{
    const float Scale = 2;

    public GameObject[] BattlePokemonPrefab;

    public GameObject MainCamera;
    public GameObject BattleCamera;

    public GameObject MainUI;
    public GameObject BattleUI;

    public Transform PlayerBattlePossition;
    public Transform EnemyBattlePossition;

    public BattlePokemonHelper EnemyBattleHelper { get; set; }
    public bool IsBattle { get; set; }

    PlayerHelper _playerHelper;
    void Start()
    {
        _playerHelper = GameObject.FindObjectOfType<PlayerHelper>();
    }

    public void StartBattle(PokemonModel myPokemonModel)
    {
        IsBattle = true;
        BattleVissibility(IsBattle);

        GameObject player = Instantiate(BattlePokemonPrefab[(int)_playerHelper.MyPokemonModel.PokemonType]);
        player.transform.SetParent(PlayerBattlePossition, false);
        player.transform.localScale = new Vector3(Scale / 3, Scale / 3, Scale / 3);

        GameObject enemy = Instantiate(BattlePokemonPrefab[(int)myPokemonModel.PokemonType]);
        enemy.transform.SetParent(EnemyBattlePossition, false);
        //enemy.transform.localScale = new Vector3(Scale, Scale, Scale);

        EnemyBattleHelper = enemy.GetComponent<BattlePokemonHelper>();
        EnemyBattleHelper.Load(myPokemonModel);

        StartCoroutine(CloseBattle());
    }

    private IEnumerator CloseBattle()
    {
        yield return new WaitForSeconds(4);
        EndBattle();
    }

    public void EndBattle()
    {
        IsBattle = false;
        BattleVissibility(IsBattle);
    }

    void Update()
    {
        
    }

    private void BattleVissibility(bool isBattle)
    {
        MainCamera.SetActive(!isBattle);
        BattleCamera.SetActive(isBattle);

        MainUI.SetActive(!isBattle);
        BattleUI.SetActive(isBattle);
    }
}
