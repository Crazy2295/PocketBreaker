using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.GameObject;

public class BattleHelper : MonoBehaviour
{
    public GameObject[] BattleUnitPrefab;
    public GameObject[] AttackUnitPrefab;

    public GameObject MainCamera;
    public GameObject BattleCamera;

    public GameObject BattleUI;

    public Transform PlayerBattlePosition;
    public Transform EnemyBattlePosition;

    [FormerlySerializedAs("playerModel")] public Animator playerAnimator;
    [FormerlySerializedAs("enemyModel")] public Animator enemyAnimator;

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

    private void ConfigureAR()
    {
        var battleground = gameObject.FindObject("BattleGround");
        var arBackground = gameObject.FindObject("ARBackground");
        var arButton = Find("ArButton").GetComponent<Button>();
        var oldColor = arButton.image.color;
        Color color;
        if (_globalStore.IsAugmented)
        {
            color = new Color(oldColor.r, oldColor.g, oldColor.b, 1f);
            battleground.SetActive(false);
            arBackground.SetActive(true);
            arBackground.AddComponent<DeviceCameraRenderer>();
        }
        else
        {
            color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);
            battleground.SetActive(true);
            if (arBackground.GetComponent<DeviceCameraRenderer>() != null)
                Destroy(arBackground.GetComponent<DeviceCameraRenderer>());
            arBackground.SetActive(false);
        }
        arButton.image.color = color;
    }
    public void StartBattle(UnitModel myUnitModel)
    {
        IsBattle = true;
        BattleVissibility(IsBattle);
        ConfigureAR();
        playerAnimator = Instantiate(BattleUnitPrefab[_playerHelper.MyUnitModel.UnitPrefabId]).GetComponent<Animator>();
        playerAnimator.transform.SetParent(PlayerBattlePosition, false);

        PlayerBattleHelper = playerAnimator.GetComponent<BattleUnitHelper>();
        PlayerBattleHelper.Load(_playerHelper.MyUnitModel);

        enemyAnimator = Instantiate(BattleUnitPrefab[myUnitModel.UnitPrefabId-1]).GetComponent<Animator>();
        enemyAnimator.transform.SetParent(EnemyBattlePosition, false);

        EnemyBattleHelper = enemyAnimator.GetComponent<BattleUnitHelper>();
        EnemyBattleHelper.Load(myUnitModel);

        UpdateUI();
    }

    private void UpdateUI()
    {
        EnemyName.text = EnemyBattleHelper.Name;
        EnemyHealth.maxValue = EnemyBattleHelper.MaxHealth;
        EnemyHealth.value = EnemyBattleHelper.Health;

//        PlayerUnitName.text = PlayerBattleHelper.Name;
        PlayerHealth.maxValue = PlayerBattleHelper.MaxHealth;
        PlayerHealth.value = PlayerBattleHelper.Health;
    }

    void EnemyAttack()
    {
        if (!IsBattle)
            return;

        PlayerBattleHelper.TakeDamage(EnemyBattleHelper.MyUnitModel.AdditionalDamage);
        UpdateUI();

        GameObject effect = Instantiate(AttackUnitPrefab[EnemyBattleHelper.MyUnitModel.UnitPrefabId]);
        effect.transform.position = PlayerBattleHelper.transform.position;
        Destroy(effect, 1);

        if (EnemyBattleHelper.IsDead)
        {
            IsBattle = false;
            Destroy(PlayerBattleHelper.gameObject);
            StartCoroutine(CloseBattle());
        }
    }

    public void DoDamage()
    {
        enemyAnimator.SetTrigger("Hit");
        EnemyBattleHelper.TakeDamage(PlayerBattleHelper.MyUnitModel.AdditionalDamage);
        UpdateUI();
        
        if (EnemyBattleHelper.IsDead)
        {
            enemyAnimator.SetTrigger("Death");
            StartCoroutine(Delay(
                enemyAnimator.GetDurationOfClip("FallenAngle_Death"),
                () =>
                {
//                    _loadUnitData.DestroyUnit(EnemyBattleHelper.MyUnitModel);
                    IsBattle = false;
//                    Destroy(EnemyBattleHelper.gameObject);
                    StartCoroutine(CloseBattle());
                }
            ));
        }
    }

    public IEnumerator Delay(float seconds, Action func)
    {
        yield return new WaitForSeconds(seconds);
        func();
    }

    public void Attack()
    {
        playerAnimator.SetTrigger("Attack1");
        DoDamage();
    }

    public void Maneuver()
    {
        playerAnimator.SetTrigger("Attack2");
        DoDamage();
    }

    public void Parry()
    {
        playerAnimator.SetTrigger("Attack3");
        DoDamage();
    }

    public void FightSpellButton()
    {
        if (!IsBattle)
            return;

        EnemyBattleHelper.TakeDamage(PlayerBattleHelper.MyUnitModel.AdditionalDamage);
        UpdateUI();

        GameObject effect = Instantiate(AttackUnitPrefab[PlayerBattleHelper.MyUnitModel.UnitPrefabId]);
        Vector3 effectPos = EnemyBattleHelper.transform.position;
        effectPos.y += 0.5f;
        effect.transform.position = effectPos;
        Destroy(effect, 1);

        if (EnemyBattleHelper.IsDead)
        {
            enemyAnimator.Play("FallenAngle_Death", 0);
            StartCoroutine(CloseBattle());
        }
    }

    public void UltimateSpellButton()
    {
        Debug.Log("UltimateSpellButton");

    }

    private IEnumerator CloseBattle()
    {
        var arBackground = gameObject.FindObject("ARBackground");
        if (arBackground.GetComponent<DeviceCameraRenderer>() != null)
            Destroy(arBackground.GetComponent<DeviceCameraRenderer>());
        yield return new WaitForSeconds(2);
        EndBattle();
    }

    public void EndBattle()
    {
        IsBattle = false;
        BattleVissibility(IsBattle);

//        _loadUnitData.DestroyUnit(EnemyBattleHelper.MyUnitModel);
        Destroy(EnemyBattleHelper.gameObject);
    }

    public void SetAR()
    {
        _globalStore.IsAugmented = !_globalStore.IsAugmented;
        ConfigureAR();
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
