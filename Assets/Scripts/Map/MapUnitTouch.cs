using UnityEngine;
using UnityEngine.EventSystems;

public class MapUnitTouch : MonoBehaviour
{
    public UnitHelper unitHelper;

    private void Start()
    {
        //throw new System.NotImplementedException();
    }

    void OnMouseDown() {
        if (!EventSystem.current.IsPointerOverGameObject())
            FindObjectOfType<BattleHelper>().StartBattle(unitHelper.MyUnitModel);
    }
}
