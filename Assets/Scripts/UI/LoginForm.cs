using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginForm : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject LoginFormUI;
    public GameObject RegistrationFormUI;

    public InputField emailText;
    public InputField passwordText;
    public Text errorText;
    public Button loginButton;

    private GlobalStore _globalStore;
    private PlayerModel _playerModel;

    private void Awake()
    {
        _globalStore = GameObject.FindObjectOfType<GlobalStore>();
        _playerModel = GameObject.FindObjectOfType<PlayerModel>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!PlayerPrefs.HasKey("token")) return;
        
        _playerModel.Email = PlayerPrefs.GetString("email");
        _playerModel.Token = PlayerPrefs.GetString("token");
        StartCoroutine(UserDataRequest(true));
    }

    public void ToRegistrationClick()
    {
        LoginFormUI.SetActive(false);
        RegistrationFormUI.SetActive(true);
    }

    public void LoginButtonClick()
    {
        loginButton.interactable = false;
        StartCoroutine(LoginRequest());
    }

    private IEnumerator LoginRequest()
    {
        if (emailText.text.Length == 0 || passwordText.text.Length == 0)
            _showError("Missing data");
        else
        {
            _hideError();

            string email = emailText.text;
            string password = passwordText.text;

            string authorization = Authenticate(email, password);
            string url = _globalStore.ServerProtocol + _globalStore.ServerUri + "/api/token";

            UnityWebRequest uwr = UnityWebRequest.Get(url);
            uwr.SetRequestHeader("AUTHORIZATION", authorization);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
                _showError(uwr.responseCode == 401 ? "Wrong data" : "Something wrong");
            }
            else
            {
                Debug.Log("Token: " + uwr.downloadHandler.text);

                PlayerForJson forJson = JsonUtility.FromJson<PlayerForJson>(uwr.downloadHandler.text);
                
                _playerModel.Token = forJson.token;
                _playerModel.Email = email;
                
                PlayerPrefs.SetString("email", email);
                PlayerPrefs.SetString("token", _playerModel.Token);
                _hideError();

                StartCoroutine(UserDataRequest());
            }
        }

        loginButton.interactable = true;
    }

    public static string Authenticate(string email, string password)
    {
        string auth = email + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    private IEnumerator UserDataRequest(bool autoConnect=false)
    {
        string url = _globalStore.ServerProtocol + _playerModel.Token + "@" + _globalStore.ServerUri + "/api/users/" + _playerModel.Email;

        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            if(!autoConnect)
                _showError("Something wrong");
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            PlayerForJson forJson = JsonUtility.FromJson<PlayerForJson>(uwr.downloadHandler.text);

            _playerModel.name = forJson.name;
            _playerModel.Gender = forJson.gender;
            
            _hideError();

            _globalStore.IsMainScreen = true;
            _globalStore.IsMenuMode = false;
            MainMenuUI.SetActive(false);
        }
    }

    private void _showError(string error)
    {
        errorText.text = error;
        errorText.gameObject.SetActive(true);
    }

    private void _hideError()
    {
        errorText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}