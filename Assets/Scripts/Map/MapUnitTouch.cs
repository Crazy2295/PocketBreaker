using UnityEngine;

public class MapUnitTouch : MonoBehaviour
{
    public UnitHelper unitHelper;

    private void Start()
    {
        //throw new System.NotImplementedException();
    }

    void OnMouseDown() {
        FindObjectOfType<BattleHelper>().StartBattle(unitHelper.MyUnitModel);
    }
}
