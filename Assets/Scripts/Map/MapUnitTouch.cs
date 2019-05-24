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
        var socket = _globalStore.socket;
        socket.Emit("start battle", "ai");
        if (!EventSystem.current.IsPointerOverGameObject())
            FindObjectOfType<BattleHelper>().StartBattle(unitModel);
    }
}
