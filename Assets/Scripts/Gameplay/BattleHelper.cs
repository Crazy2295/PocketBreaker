using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.Internal.Experimental.UIElements;
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
    private Text _message;
    private GameObject _messagePanel;
    private static readonly int _attack1 = Animator.StringToHash("Attack1");
    private static readonly int _attack3 = Animator.StringToHash("Attack3");
    private static readonly int _attack2 = Animator.StringToHash("Attack2");
    private static readonly int _hit = Animator.StringToHash("Hit");
    private static readonly int _death = Animator.StringToHash("Death");

    private string valueMessage(int value)
    {
        return value < 0 ? $"healed self for {-value}" : $"damaged for {value}";
    }

    void Start()
    {
        _playerHelper = FindObjectOfType<PlayerHelper>();
        _loadUnitData = FindObjectOfType<LoadUnitData>();
        _globalStore = FindObjectOfType<GlobalStore>();

        _messagePanel = Find("Canvas").FindObject("MessagePanel");
        _message = _messagePanel.FindObject("Text").GetComponent<Text>();
        
        var battleHandlers = gameObject.GetComponent<BattleHandlers>();
        battleHandlers.MoveGotResult = result =>
        {
            LockInterface(3f);
            EnemyAttack(result.Enemy.Move.ActionType);
            
            PlayerBattleHelper.NewHp(result.Self.Model.Hp);
            EnemyBattleHelper.NewHp(result.Enemy.Model.Hp);
            var value = result.Self.ActualValue;
            var enemyValue = result.Enemy.ActualValue;
            ShowMessage($"Your move is {result.Self.Strength}! " +
                        $"Your opponent {valueMessage(enemyValue)} " +
                        $"You {valueMessage(value)}", 3f);
            UpdateUI();
        };

        battleHandlers.EndBattle = result =>
        {

            if (result.Self.Model.Hp <= 0 && result.Enemy.Model.Hp <= 0)
            {
                ShowMessage("Nobody won, but you fought good!", 3f);
                PlayerBattleHelper.IsDead = true;
                DoDamage(PlayerBattleHelper, playerAnimator);
                EnemyBattleHelper.IsDead = true;
                DoDamage(EnemyBattleHelper, enemyAnimator);
            } else if (result.Self.Model.Hp <= 0)
            {
                ShowMessage("You lost.", 3f);
                PlayerBattleHelper.IsDead = true;
                DoDamage(PlayerBattleHelper, playerAnimator);
            } else if (result.Enemy.Model.Hp <= 0)
            {
                ShowMessage("Victory!", 3f);
                EnemyBattleHelper.IsDead = true;
                DoDamage(EnemyBattleHelper, enemyAnimator);
            }
            else
            {
                throw new ArgumentException(
                    $"Ответ от сервера неверен! Мое HP = {result.Self.Model.Hp}, а у врага HP = {result.Enemy.Model.Hp}"
                );
            }
        };
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
    private void ShowMessage(string message, float seconds)
    {
        _message.text = message;
        _messagePanel.SetActive(true);
        StartCoroutine(Delay(seconds,
            () => _messagePanel.SetActive(false)
        ));
    }
    public void LockInterface(float duration)
    {
        var canvas = Find("Canvas");
        if (canvas == null) return;
        var playerPanel = canvas.FindObject("PlayerPanel");
        var enemyPanel = canvas.FindObject("EnemyPanel");
        playerPanel.SetActive(false);
        enemyPanel.SetActive(false);
        StartCoroutine(Delay(duration, () =>
        {
            playerPanel.SetActive(true);
            enemyPanel.SetActive(true);
        }));
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

    void EnemyAttack(int attackType)
    {
        int trigger;
        switch (attackType)
        {
            case 0: trigger = _attack1;
                break;
            case 1: trigger = _attack2;
                break;
            case 2: trigger = _attack3;
                break;
            default:
                throw new ArgumentException("Unknown attack type");
        }
        enemyAnimator.SetTrigger(trigger);
        DoDamage(PlayerBattleHelper, playerAnimator);
//        if (!IsBattle)
//            return;
//
//        PlayerBattleHelper.TakeDamage(EnemyBattleHelper.MyUnitModel.AdditionalDamage);
//        UpdateUI();
//
//        GameObject effect = Instantiate(AttackUnitPrefab[EnemyBattleHelper.MyUnitModel.UnitPrefabId]);
//        effect.transform.position = PlayerBattleHelper.transform.position;
//        Destroy(effect, 1);
//
//        if (EnemyBattleHelper.IsDead)
//        {
//            IsBattle = false;
//            Destroy(PlayerBattleHelper.gameObject);
//            StartCoroutine(CloseBattle());
//        }
    }

    public void DoDamage(BattleUnitHelper characterBattleHelper, Animator characterAnimator)
    {
//        if (isEnemy) enemyAnimator.SetTrigger(_hit);
//        else playerAnimator.SetTrigger(_hit);
//        EnemyBattleHelper.TakeDamage(PlayerBattleHelper.MyUnitModel.AdditionalDamage);
//        UpdateUI();
        
        if (characterBattleHelper.IsDead)
        {
            characterAnimator.SetTrigger(_death);
            if (!IsBattle) return;
            var duration = enemyAnimator.GetDurationOfClip("FallenAngle_Death");
            LockInterface(duration + 2);
            IsBattle = false;
            StartCoroutine(Delay(duration,
                () =>
                {
//                    _loadUnitData.DestroyUnit(EnemyBattleHelper.MyUnitModel);
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
        var socket = _globalStore.socket;
        socket.Emit("pass move", "1");
        playerAnimator.SetTrigger(_attack1);
        DoDamage(EnemyBattleHelper, enemyAnimator);
    }

    public void Maneuver()
    {
        var socket = _globalStore.socket;
        socket.Emit("pass move", "2");
        playerAnimator.SetTrigger(_attack2);
        DoDamage(EnemyBattleHelper, enemyAnimator);
    }

    public void Parry()
    {
        var socket = _globalStore.socket;
        socket.Emit("pass move", "3");
        playerAnimator.SetTrigger(_attack3);
        DoDamage(EnemyBattleHelper, enemyAnimator);
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
        if (PlayerBattleHelper != null) Destroy(PlayerBattleHelper.gameObject);
        if (EnemyBattleHelper != null) Destroy(EnemyBattleHelper.gameObject);
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
