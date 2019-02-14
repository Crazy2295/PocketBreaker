using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHelper : MonoBehaviour
{
    const float Scale = 2;

    public GameObject[] BattlePokemonPrefab;
    public GameObject[] AttackPokemonPrefab;

    public GameObject MainCamera;
    public GameObject BattleCamera;

    public GameObject MainUI;
    public GameObject BattleUI;

    public Transform PlayerBattlePosition;
    public Transform EnemyBattlePosition;

    #region UI
    public Text PlayerName;
    public Slider PlayerHealth;

    public Text EnemyName;
    public Slider EnemyHealth;
    #endregion

    public BattlePokemonHelper EnemyBattleHelper { get; set; }
    public BattlePokemonHelper PlayerBattleHelper { get; set; }
    public bool IsBattle { get; set; }

    PlayerHelper _playerHelper;
    LoadPokemonData _loadPokemonData;

    void Start()
    {
        _playerHelper = GameObject.FindObjectOfType<PlayerHelper>();
        _loadPokemonData = GameObject.FindObjectOfType<LoadPokemonData>();

        //InvokeRepeating("EnemyAttack", AttackSpeed, AttackSpeed);
    }

    public void StartBattle(PokemonModel myPokemonModel)
    {
        IsBattle = true;
        BattleVissibility(IsBattle);

        GameObject player = Instantiate(BattlePokemonPrefab[(int)_playerHelper.MyPokemonModel.PokemonType]);
        player.transform.SetParent(PlayerBattlePosition, false);
        player.transform.localScale = new Vector3(Scale / 3, Scale / 3, Scale / 3);

        PlayerBattleHelper = player.GetComponent<BattlePokemonHelper>();
        PlayerBattleHelper.Load(_playerHelper.MyPokemonModel);

        GameObject enemy = Instantiate(BattlePokemonPrefab[(int)myPokemonModel.PokemonType]);
        enemy.transform.SetParent(EnemyBattlePosition, false);
        //enemy.transform.localScale = new Vector3(Scale, Scale, Scale);

        EnemyBattleHelper = enemy.GetComponent<BattlePokemonHelper>();
        EnemyBattleHelper.Load(myPokemonModel);

        UpdateUI();
        //StartCoroutine(CloseBattle());
    }

    private void UpdateUI()
    {
        EnemyName.text = EnemyBattleHelper.Name;
        EnemyHealth.maxValue = EnemyBattleHelper.MaxHealth;
        EnemyHealth.value = EnemyBattleHelper.Health;

        //PlayerPokemonName.text = PlayerBattleHelper.Name;
        PlayerHealth.maxValue = PlayerBattleHelper.MaxHealth;
        PlayerHealth.value = PlayerBattleHelper.Health;
    }

    void EnemyAttack()
    {
        if (!IsBattle)
            return;

        Debug.Log("EnemyAttack");

        PlayerBattleHelper.TakeDamage(EnemyBattleHelper.MyPokemonModel.Damage);
        UpdateUI();

        GameObject effect = Instantiate(AttackPokemonPrefab[(int)EnemyBattleHelper.MyPokemonModel.PokemonType]);
        effect.transform.position = PlayerBattleHelper.transform.position;
        Destroy(effect, 1);

        if (EnemyBattleHelper.IsDead)
        {
            IsBattle = false;
            Destroy(PlayerBattleHelper.gameObject);
            StartCoroutine(CloseBattle());
        }
    }

    public void FightSpellButton()
    {
        if (!IsBattle)
            return;

        Debug.Log("FightSpellButton");

        EnemyBattleHelper.TakeDamage(PlayerBattleHelper.MyPokemonModel.Damage);
        UpdateUI();

        //GameObject effect = Instantiate(AttackPokemonPrefab[(int)PlayerBattleHelper.MyPokemonModel.PokemonType]);
        GameObject effect = Instantiate(AttackPokemonPrefab[(int)0]);
        Vector3 effectPos = EnemyBattleHelper.transform.position;
        effectPos.y += 0.5f;
        effect.transform.position = effectPos;
        Destroy(effect, 1);

        if (EnemyBattleHelper.IsDead)
        {
            _loadPokemonData.DestroyPokemon(EnemyBattleHelper.MyPokemonModel);
            IsBattle = false;
            Destroy(EnemyBattleHelper.gameObject);
            StartCoroutine(CloseBattle());
        }
    }

    public void UltimateSpellButton()
    {
        Debug.Log("UltimateSpellButton");

    }

    private IEnumerator CloseBattle()
    {
        yield return new WaitForSeconds(2);
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
