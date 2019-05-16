using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using socket.io;

public class RegistrationForm : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject RegistrationFormUI;

    public InputField nameText;
    public InputField emailText;
    public InputField passwordText;
    public InputField repeatPasswordText;
    public Dropdown gender;

    public Text errorText;
    public Button registrationButton;

    private GlobalStore _globalStore;
    private PlayerModel _playerModel;

    private void Awake()
    {
        _globalStore = GameObject.FindObjectOfType<GlobalStore>();
        _playerModel = GameObject.FindObjectOfType<PlayerModel>();
    }

    // Start is called before the first frame update
    void Start()
    {
        List<string> mDropOptions = Enum.GetNames(typeof(GenderEnum)).ToList();
        gender.ClearOptions();
        gender.AddOptions(mDropOptions);
    }

    public void RegistrationButtonClick()
    {
        registrationButton.interactable = false;
        StartCoroutine(RegistrationRequest());
    }

    private IEnumerator RegistrationRequest()
    {
        Regex regex = new Regex(@"\w+@\w+\.\w+", RegexOptions.IgnoreCase);
        if (nameText.text.Length == 0 || emailText.text.Length == 0 ||
            passwordText.text.Length == 0 || repeatPasswordText.text.Length == 0)
        {
            _showError("Missing data");
        }
        else if (passwordText.text != repeatPasswordText.text)
        {
            _showError("Passwords not equal");
        }
        else if (!regex.IsMatch(emailText.text))
        {
            _showError("Incorrect email");
        }
        else
        {
            _hideError();
            
            string nameT = nameText.text;
            string email = emailText.text;
            string password = passwordText.text;

            WWWForm form = new WWWForm();
            form.AddField("name", nameT);
            form.AddField("password", password);
            form.AddField("email", email);
            form.AddField("gender", (int) (GenderEnum) Enum.Parse(typeof(GenderEnum), gender.captionText.text));

            string url = _globalStore.ServerProtocol + _globalStore.ServerUri + "/api/users";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
                _showError(uwr.responseCode == 409 ? "Email or Name is already used" : "Something wrong");
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);

                _playerModel.Name = nameT;
                _playerModel.Email = email;
                _playerModel.Gender = (GenderEnum) Enum.Parse(typeof(GenderEnum), gender.captionText.text);

                PlayerPrefs.SetString("email", email);
                _hideError();
                
                StartCoroutine(TokenRequest(email, password));
            }
        }

        registrationButton.interactable = true;
    }

    private IEnumerator TokenRequest(string email, string password)
    {
        string authorization = LoginForm.Authenticate(email, password);
        string url = _globalStore.ServerProtocol + _globalStore.ServerUri + "/api/token";

        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.SetRequestHeader("AUTHORIZATION", authorization);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            _showError("Something wrong");
        }
        else
        {
            Debug.Log("Token: " + uwr.downloadHandler.text);

            PlayerForJson forJson = JsonUtility.FromJson<PlayerForJson>(uwr.downloadHandler.text);
                
            _playerModel.Token = forJson.token;
            PlayerPrefs.SetString("token", _playerModel.Token);
            _hideError();

            SocketConnection();
        }

        registrationButton.interactable = true;
    }
    
    private void SocketConnection()
    {
        var serverUrl = _globalStore.ServerProtocol + _globalStore.ServerUri;
        var socket = Socket.Connect(serverUrl);
        
        socket.On(SystemEvents.connect, () => {
            Debug.Log("Socket connected");
            _globalStore.socket = socket;
            _globalStore.SocketSet = true;
            
            _hideError();
            _deactivateMainMenu();
        });
        
        socket.On(SystemEvents.connectError, (System.Exception error) =>
        {
            Debug.Log("Socket connect error " + error);
            _showError("Socket error");
        });
        
        socket.On(SystemEvents.connectTimeOut, () =>
        {
            Debug.Log("Socket connect TimeOut");
            _showError("Socket error");
        });
    }
    
    private void _deactivateMainMenu()
    {
        _globalStore.IsMainScreen = true;
        _globalStore.IsMenuMode = false;
        MainMenuUI.SetActive(false);
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