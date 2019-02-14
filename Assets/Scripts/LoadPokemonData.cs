using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;

public class LoadPokemonData : MonoBehaviour
{
    public GameObject[] PokemonPrefabs;

    string _xml = "";

    public List<PokemonModel> PokemonModels { get; set; }
    public List<PokemonHelper> PokemonHelpers { get; set; }

    GameHelper _gameHelper;
    IEnumerator Start()
    {
        PokemonModels = new List<PokemonModel>();
        PokemonHelpers = new List<PokemonHelper>();
        _gameHelper = GetComponent<GameHelper>();

        while (!_gameHelper.GpsFix)
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
            PokemonModel pokemonModel = new PokemonModel();
            ///<pokemon type="0" lat="42.3637" lon="-71.05686"></pokemon>
            int pokemonTypeInt = System.Convert.ToInt32(item.Attribute("type").Value);
            pokemonModel.PokemonType = (ETypes)pokemonTypeInt;

            pokemonModel.Id = System.Convert.ToInt32(item.Attribute("id").Value);

            pokemonModel.Lat = float.Parse(item.Attribute("lat").Value, System.Globalization.CultureInfo.InvariantCulture);
            pokemonModel.Lon = float.Parse(item.Attribute("lon").Value, System.Globalization.CultureInfo.InvariantCulture);
            pokemonModel.Orint = System.Convert.ToSingle(item.Attribute("orint").Value);

            pokemonModel.Exp = System.Convert.ToInt32(item.Attribute("exp").Value);
            pokemonModel.Damage = System.Convert.ToInt32(item.Attribute("damage").Value);
            pokemonModel.Health = System.Convert.ToInt32(item.Attribute("health").Value);

            PokemonModels.Add(pokemonModel);
        }

        Debug.Log("PokemonModels.Count = " + PokemonModels.Count);


        for (int i = 0; i < PokemonModels.Count; i++)
        {
            var item = PokemonModels[i];

            GameObject pokemon = Instantiate(PokemonPrefabs[(int)item.PokemonType]);
            SetGeolocation setGeolocation = pokemon.GetComponent<SetGeolocation>();
            setGeolocation.SetLoacation(item.Lat, item.Lon, item.Orint);

            PokemonHelper pokemonHelper = pokemon.GetComponent<PokemonHelper>();
            pokemonHelper.LoadPokemon(item);

            PokemonHelpers.Add(pokemonHelper);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void DestroyPokemon(PokemonModel myPokemonModel)
    {
        PokemonHelper remove = null;
        foreach (var item in PokemonHelpers)
        {
            if (item.MyPokemonModel.Id == myPokemonModel.Id)
            {
                remove = item;
            }
        }

        Destroy(remove.gameObject);
        PokemonHelpers.Remove(remove);
    }
}
