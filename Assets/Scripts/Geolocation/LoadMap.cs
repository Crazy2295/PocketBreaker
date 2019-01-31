using UnityEngine;
using System.Collections;

public class LoadMap : MonoBehaviour
{
    // const string Key = "2Z7Bu4G1SvioKF2TfIl67i0u3MevFdzu";
    const string Key = "AIzaSyBvH8rkd29W1MyusMpJJ-feHnjDvUf89Ys";

    public Renderer maprender;
    Vector2 PlayerPosition =
       new Vector2(47.240342f, 38.879884f);  //Latitude, Longitude
                                             //   new Vector2(42.3627f, -71.05686f);  //Latitude, Longitude

    int _zoom = 17;
    string _mapType = "terrain";
    int _mapScale = 1;
    int _mapSize = 640;
    string _url;

    void Start()
    {
        StartLoadMap(PlayerPosition);
    }


    private void StartLoadMap(Vector2 playerPosition)
    {
        _url = "https://maps.googleapis.com/maps/api/staticmap?center=" +
            PlayerPosition.x.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture) +
            "," + PlayerPosition.y.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture) +
            "&zoom=" + _zoom + "&size=" + _mapSize + "x" + _mapSize + "&scale=" + _mapScale + "&language=ru" +
            "&type=" + _mapType + "&style=feature:all|element:labels|visibility:off" + "&key=" + Key;

        Debug.Log(_url);

        StartCoroutine(LoadImage());
    }

    private IEnumerator LoadImage()
    {
        WWW www = new WWW(_url);
        while (!www.isDone)
        {
            Debug.Log("progress = " + www.progress);
            yield return null;
        }

        if (www.error == null)
        {
            Debug.Log("Updating map 100 %");
            Debug.Log("Map Ready!");
            yield return new WaitForSeconds(0.5f);
            maprender.material.mainTexture = null;
            Texture2D tmp;
            tmp = new Texture2D(_mapSize, _mapSize, TextureFormat.RGB24, false);
            maprender.material.mainTexture = tmp;
            www.LoadImageIntoTexture(tmp);
        }
        else {
            Debug.Log("Map Error:" + www.error);
            yield return new WaitForSeconds(1);
            maprender.material.mainTexture = null;
        }

        maprender.enabled = true;
    }
}
