using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapUnitTouch : MonoBehaviour
{
    public UnitModel unitModel;
    public GameObject UI;
    private GlobalStore _globalStore;

    private Text[] _textFields;
    private Button[] _buttons;

    private void Awake()
    {
        _globalStore = FindObjectOfType<GlobalStore>();
    }

    private void Start()
    {
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        ShowInfo();
    }

    void ShowInfo()
    {
        _textFields = UI.GetComponentsInChildren<Text>();
        _textFields[0].text = unitModel.UnitModification;
        _textFields[1].text = unitModel.Hp.ToString();
        if (unitModel.AdditionalDamage > 0)
            _textFields[2].text = "+" + unitModel.AdditionalDamage.ToString();
        else
            _textFields[2].text = unitModel.AdditionalDamage.ToString();

        _buttons = UI.GetComponentsInChildren<Button>();
        _buttons[0].onClick.RemoveAllListeners();
        _buttons[1].onClick.RemoveAllListeners();
        
        _buttons[0].onClick.AddListener(GoFight);
        _buttons[1].onClick.AddListener(Decline);

        UI.SetActive(true);
    }

    void GoFight()
    {
        UI.SetActive(false);
        var socket = _globalStore.socket;
        var id = unitModel.UnitModification.Contains("@") ? unitModel.UnitModification : $"ai#{unitModel.Id}";
        socket.Emit("start battle", id);
        FindObjectOfType<BattleHelper>().StartBattle(unitModel);
    }
    
    void Decline()
    {
        UI.SetActive(false);
        _buttons[0].onClick.RemoveAllListeners();
        _buttons[1].onClick.RemoveAllListeners();
    }
}