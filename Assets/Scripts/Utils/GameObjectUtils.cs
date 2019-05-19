using System.Linq;
using UnityEngine;

public static class GameObjectUtils
{
    public static GameObject FindObject(this GameObject parent, string name)
    {
        var trs= parent.GetComponentsInChildren<Transform>(true);
        return (from t in trs where t.name == name select t.gameObject).FirstOrDefault();
    }
}