using UnityEngine;
using UnityEngine.EventSystems;

public class MapUnitTouch : MonoBehaviour
{
    public UnitModel unitModel;
    private GlobalStore _globalStore;

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
        var socket = _globalStore.socket;
        var id = unitModel.UnitModification.Contains("@") ? 
            unitModel.UnitModification : $"ai#{unitModel.Id}";
        socket.Emit("start battle", id);
        FindObjectOfType<BattleHelper>().StartBattle(unitModel);
    }
}
