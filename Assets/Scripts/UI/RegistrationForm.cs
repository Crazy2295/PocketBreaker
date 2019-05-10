using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    }

    public void RegistrationButtonClick()
    {
        registrationButton.interactable = false;
        StartCoroutine(RegistrationRequest());
    }

    private IEnumerator RegistrationRequest()
    {
        if (nameText.text.Length == 0 || emailText.text.Length == 0 ||
            passwordText.text.Length == 0 || repeatPasswordText.text.Length == 0)
        {
            _showError("Missing data");
        }
        else if (passwordText.text != repeatPasswordText.text)
        {
            _showError("Passwords not equal");
        }
        else
        {
            _hideError();

            WWWForm form = new WWWForm();
            form.AddField("name", nameText.text);
            form.AddField("password", passwordText.text);
            form.AddField("email", emailText.text);
            form.AddField("gender", (int) (GenderEnum) Enum.Parse(typeof(GenderEnum), gender.captionText.text));
            form.AddField("birthdate", "2019-05-10");

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

                _playerModel.Name = nameText.text;
                _playerModel.Email = emailText.text;
                _playerModel.Gender = (GenderEnum) Enum.Parse(typeof(GenderEnum), gender.captionText.text);

                _hideError();
                
                StartCoroutine(TokenRequest());
            }
        }

        registrationButton.interactable = true;
    }

    private IEnumerator TokenRequest()
    {
        string authorization = _Authenticate(emailText.text, passwordText.text);
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
            _hideError();

            _globalStore.IsMainScreen = true;
            _globalStore.IsMenuMode = false;
            MainMenuUI.SetActive(false);
        }

        registrationButton.interactable = true;
    }

    private string _Authenticate(string email, string password)
    {
        string auth = email + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
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