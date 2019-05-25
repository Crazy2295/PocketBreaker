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
        socket.Emit("start battle", $"ai#{unitModel.Id}");
        FindObjectOfType<BattleHelper>().StartBattle(unitModel);
    }
}
