using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GameObject;

public class BattleHelper : MonoBehaviour
{
    const float Scale = 2;

    public GameObject[] BattleUnitPrefab;
    public GameObject[] AttackUnitPrefab;

    public GameObject MainCamera;
    public GameObject BattleCamera;

    public GameObject BattleUI;

    public Transform PlayerBattlePosition;
    public Transform EnemyBattlePosition;

    #region UI
    public Text PlayerName;
    public Slider PlayerHealth;

    public Text EnemyName;
    public Slider EnemyHealth;
    #endregion

    public BattleUnitHelper EnemyBattleHelper { get; set; }
    public BattleUnitHelper PlayerBattleHelper { get; set; }
    public bool IsBattle { get; set; }

    PlayerHelper _playerHelper;
    LoadUnitData _loadUnitData;
    GlobalStore _globalStore;
    
    void Start()
    {
        _playerHelper = FindObjectOfType<PlayerHelper>();
        _loadUnitData = FindObjectOfType<LoadUnitData>();
        _globalStore = FindObjectOfType<GlobalStore>();
        //InvokeRepeating("EnemyAttack", AttackSpeed, AttackSpeed);
    }

    public void StartBattle(UnitModel myUnitModel)
    {
        IsBattle = true;
       
        BattleVissibility(IsBattle);
        if (_globalStore.IsAugmented)
        {
            Find("BattleGround").SetActive(false);
            Find("ARBackground").AddComponent<DeviceCameraRenderer>();
        }
        GameObject player = Instantiate(BattleUnitPrefab[(int)_playerHelper.MyUnitModel.UnitType]);
        player.transform.SetParent(PlayerBattlePosition, false);
        player.transform.localScale = new Vector3(Scale / 3, Scale / 3, Scale / 3);

        PlayerBattleHelper = player.GetComponent<BattleUnitHelper>();
        PlayerBattleHelper.Load(_playerHelper.MyUnitModel);

        GameObject enemy = Instantiate(BattleUnitPrefab[(int)myUnitModel.UnitType]);
        enemy.transform.SetParent(EnemyBattlePosition, false);
        //enemy.transform.localScale = new Vector3(Scale, Scale, Scale);

        EnemyBattleHelper = enemy.GetComponent<BattleUnitHelper>();
        EnemyBattleHelper.Load(myUnitModel);

        UpdateUI();
    }

    private void UpdateUI()
    {
        EnemyName.text = EnemyBattleHelper.Name;
        EnemyHealth.maxValue = EnemyBattleHelper.MaxHealth;
        EnemyHealth.value = EnemyBattleHelper.Health;

        //PlayerUnitName.text = PlayerBattleHelper.Name;
        PlayerHealth.maxValue = PlayerBattleHelper.MaxHealth;
        PlayerHealth.value = PlayerBattleHelper.Health;
    }

    void EnemyAttack()
    {
        if (!IsBattle)
            return;

        PlayerBattleHelper.TakeDamage(EnemyBattleHelper.MyUnitModel.Damage);
        UpdateUI();

        GameObject effect = Instantiate(AttackUnitPrefab[(int)EnemyBattleHelper.MyUnitModel.UnitType]);
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

        EnemyBattleHelper.TakeDamage(PlayerBattleHelper.MyUnitModel.Damage);
        UpdateUI();

        //GameObject effect = Instantiate(AttackUnitPrefab[(int)PlayerBattleHelper.MyUnitModel.UnitType]);
        GameObject effect = Instantiate(AttackUnitPrefab[(int)0]);
        Vector3 effectPos = EnemyBattleHelper.transform.position;
        effectPos.y += 0.5f;
        effect.transform.position = effectPos;
        Destroy(effect, 1);

        if (EnemyBattleHelper.IsDead)
        {
            _loadUnitData.DestroyUnit(EnemyBattleHelper.MyUnitModel);
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
        Destroy(Find("Background").GetComponent<DeviceCameraRenderer>());
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

        BattleUI.SetActive(isBattle);
    }
}
