using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;

public class LoadUnitData : MonoBehaviour
{
    public GameObject[] UnitPrefabs;

    string _xml = "";

    public List<UnitModel> UnitModels { get; set; }
    public List<UnitHelper> UnitHelpers { get; set; }

    GPSCheck _gpsCheck;
    IEnumerator Start()
    {
        UnitModels = new List<UnitModel>();
        UnitHelpers = new List<UnitHelper>();
        _gpsCheck = GameObject.FindObjectOfType<GPSCheck>();

        while (!_gpsCheck.GpsOn)
        {
            Debug.Log("Wait!");
            yield return null;
        }

        WWW www = new WWW("https://drive.google.com/uc?authuser=0&id=1xeJ8hw-xwhK_6s0E0sbCfkWB0cMgrUDO&export=download");
        while (!www.isDone)
        {
            yield return null;
        }

        Debug.Log(www.text);
        _xml = www.text;


        XDocument doc = XDocument.Parse(_xml);
        XElement element = doc.Element("units");
        IEnumerable<XElement> elements = element.Elements();

        foreach (XElement item in elements)
        {
            UnitModel unitModel = new UnitModel();
            int unitTypeInt = System.Convert.ToInt32(item.Attribute("type").Value);
            unitModel.UnitType = (ETypes)unitTypeInt;

            unitModel.Id = System.Convert.ToInt32(item.Attribute("id").Value);

            unitModel.Lat = float.Parse(item.Attribute("lat").Value, System.Globalization.CultureInfo.InvariantCulture);
            unitModel.Lon = float.Parse(item.Attribute("lon").Value, System.Globalization.CultureInfo.InvariantCulture);
            unitModel.Orint = System.Convert.ToSingle(item.Attribute("orint").Value);

            unitModel.Exp = System.Convert.ToInt32(item.Attribute("exp").Value);
            unitModel.Damage = System.Convert.ToInt32(item.Attribute("damage").Value);
            unitModel.Health = System.Convert.ToInt32(item.Attribute("health").Value);

            UnitModels.Add(unitModel);
        }

        Debug.Log("UnitModels.Count = " + UnitModels.Count);


        for (int i = 0; i < UnitModels.Count; i++)
        {
            var item = UnitModels[i];

            GameObject unit = Instantiate(UnitPrefabs[(int)item.UnitType]);
            SetGeolocation setGeolocation = unit.GetComponent<SetGeolocation>();
            setGeolocation.SetLocation(item.Lat, item.Lon, item.Orint);

            UnitHelper unitHelper = unit.GetComponent<UnitHelper>();
            unitHelper.LoadUnit(item);

            UnitHelpers.Add(unitHelper);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void DestroyUnit(UnitModel myUnitModel)
    {
        UnitHelper remove = null;
        foreach (var item in UnitHelpers)
        {
            if (item.MyUnitModel.Id == myUnitModel.Id)
            {
                remove = item;
            }
        }

        Destroy(remove.gameObject);
        UnitHelpers.Remove(remove);
    }
}
