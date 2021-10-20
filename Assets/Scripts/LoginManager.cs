using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using Assets.Models;
using Newtonsoft.Json;
using System.Data;
using Mono.Data.Sqlite;
using Assets.Scripts.Data;
using System.Linq;
using Assets.Scripts.Models;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LoginManager : MonoBehaviour
{
    // Login Inputs
    [SerializeField] TMP_InputField loginEMailInput;
    [SerializeField] TMP_InputField loginPasswordInput;

    // Register Inputs
    [SerializeField] TMP_InputField registerFullNameInput;
    [SerializeField] TMP_InputField registerEMailInput;
    [SerializeField] TMP_InputField registerPasswordInput;

    // Reset Password Inputs
    [SerializeField] TMP_InputField resetPasswordEMailInput;

    // Change Password Inputs
    [SerializeField] TMP_InputField changePasswordCodeInput;
    [SerializeField] TMP_InputField changePasswordInput;

    // Panels
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject registerPanel;
    [SerializeField] GameObject resetPasswordPanel;
    [SerializeField] GameObject changePasswordPanel;

    // Others
    EventSystem system;
    [SerializeField] GameObject loadingAnimationPrefab;

    // Alert Panel
    [SerializeField] Animator errorAnimation;
    [SerializeField] GameObject errorMessageTextPrefab;
    [SerializeField] GameObject errorMessageTextParent;
    [SerializeField] List<GameObject> errorMessageTexts;

    void Awake()
    {
        try
        {
            Shortlinkdb<Player> shortlinkdb = new Shortlinkdb<Player>();
            shortlinkdb.CreateTable("CREATE TABLE IF NOT EXISTS Player (Id INTEGER NOT NULL, Token TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT)) ;");

            var asd = shortlinkdb.Que("select * from Player").FirstOrDefault();
            if (asd != null)
            {
                StartCoroutine(LoadAsynchronously(1, 0));
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        system = EventSystem.current;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (system.currentSelectedGameObject != null)
            {
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

                if (next != null)
                {

                    InputField inputfield = next.GetComponent<InputField>();
                    if (inputfield != null)
                        inputfield.OnPointerClick(new PointerEventData(system));

                    system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (loginPanel.activeSelf)
            {
                Login();
            }
            else if (registerPanel.activeSelf)
            {
                Reqister();
            }
            else if (resetPasswordPanel.activeSelf)
            {
                ResetPassword();
            }
            else if (changePasswordPanel.activeSelf)
            {
                ChangePassword();
            }
        }

    }

    public void Login()
    {
        loadingAnimationPrefab.SetActive(true);
        StartCoroutine(Login("https://umuly.com/api/Token?Email=" + loginEMailInput.text + "&Password=" + loginPasswordInput.text));
    }

    IEnumerator Login(string uri)
    {
        var request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var a = JsonConvert.DeserializeObject<MResponseBase<MUser.Response>>(request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                try
                {

                    var token = JsonConvert.DeserializeObject<MToken>(request.downloadHandler.text);

                    Shortlinkdb<Player> shortlinkdb = new Shortlinkdb<Player>();
                    var asd = shortlinkdb.Que("select * from Player");
                    bool isToken = asd.Any();
                    if (isToken == true)
                    {
                        int userId = asd.FirstOrDefault().Id;
                        shortlinkdb.Update("update Player set Token = '" + token.token + "' where Id = " + userId + "");
                    }
                    else
                    {
                        shortlinkdb.Insert("insert into Player (Token) values ('" + token.token + "')");
                    }

                    StartCoroutine(LoadAsynchronously(1, 0));
                }
                catch (Exception ex)
                {
                    loadingAnimationPrefab.SetActive(false);
                    ShowError(ex.Message);
                }
            }
            else
            {
                loadingAnimationPrefab.SetActive(false);
                ShowError(a.statusText);
            }
        }
        else
        {
            loadingAnimationPrefab.SetActive(false);
            ShowError(request.downloadHandler.text.Trim('"'));
        }
    }

    public void Reqister()
    {
        loadingAnimationPrefab.SetActive(true);
        MUser.Form user = new MUser.Form();
        user.name = registerFullNameInput.text;
        user.email = registerEMailInput.text;
        user.password = registerPasswordInput.text;

        StartCoroutine(Register("http://umuly.com/api/user", JsonConvert.SerializeObject(user)));
    }

    IEnumerator Register(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        var a = JsonConvert.DeserializeObject<MResponseBase<MUser.Response>>(request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 200)
            {
                ChangePanel(1);
            }
        }
        else
        {
            if (a.errors != null)
            {
                if (a.errors.Name != null)
                {
                    ShowError(a.errors.Name);
                }
                if (a.errors.Email != null)
                {
                    ShowError(a.errors.Email);
                }
                if (a.errors.Password != null)
                {
                    ShowError(a.errors.Password);
                }
            }
            else
            {
                if (request.downloadHandler.text.Contains('{'))
                {
                    ShowError(a.statusText);
                }
                else
                {
                    ShowError(request.downloadHandler.text);
                }
            }
        }

        loadingAnimationPrefab.SetActive(false);
    }

    public void ResetPassword()
    {
        loadingAnimationPrefab.SetActive(true);
        MUser.Form user = new MUser.Form();
        user.email = resetPasswordEMailInput.text;

        StartCoroutine(ResetPassword("https://umuly.com/api/user/reset-password", JsonUtility.ToJson(user)));
    }

    IEnumerator ResetPassword(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        var a = JsonConvert.DeserializeObject<MResponseBase<MUser.Response>>(request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 200)
            {
                ChangePanel(4);
            }
            else
            {
                ShowError(a.statusText);
            }
        }
        else
        {
            if (request.downloadHandler.text.Contains('{'))
            {
                ShowError(a.statusText);
            }
            else
            {
                ShowError(request.downloadHandler.text);
            }
        }

        loadingAnimationPrefab.SetActive(false);
    }

    public void ChangePassword()
    {
        loadingAnimationPrefab.SetActive(true);
        MUser.Form changePasswordForm = new MUser.Form();
        changePasswordForm.email = resetPasswordEMailInput.text;
        changePasswordForm.password = changePasswordInput.text;
        changePasswordForm.code = changePasswordCodeInput.text;

        StartCoroutine(ChangePassword("https://umuly.com/api/user/change-password", JsonUtility.ToJson(changePasswordForm)));
    }

    IEnumerator ChangePassword(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        var a = JsonConvert.DeserializeObject<MResponseBase<MUser.Response>>(request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 200)
            {
                ChangePanel(1);
                resetPasswordEMailInput.text = "";
            }
            else
            {
                ShowError(a.statusText);
            }
        }
        else
        {
            if (a.errors != null)
            {
                if (a.errors.Name != null)
                {
                    ShowError(a.errors.Name);
                }
                if (a.errors.Email != null)
                {
                    ShowError(a.errors.Email);
                }
                if (a.errors.Password != null)
                {
                    ShowError(a.errors.Password);
                }
                if (a.errors.Code != null)
                {
                    ShowError(a.errors.Code);
                }
            }
            else
            {
                if (request.downloadHandler.text.Contains('{'))
                {
                    ShowError(a.statusText);
                }
                else
                {
                    ShowError(request.downloadHandler.text);
                }
            }
        }

        loadingAnimationPrefab.SetActive(false);
    }

    public void ShowError(List<string> errors)
    {
        foreach (var item in errorMessageTexts)
        {
            Destroy(item.gameObject);
        }

        errorMessageTexts.Clear();

        foreach (var error in errors)
        {
            var gameObject = Instantiate(errorMessageTextPrefab, errorMessageTextParent.transform);
            errorMessageTexts.Add(gameObject);
            gameObject.transform.SetAsLastSibling();
            gameObject.GetComponent<TextMeshProUGUI>().text = errorMessageTexts.Count + ". " + error;
            errorAnimation.SetTrigger("Open");
        }
    }

    public void ShowError(string error)
    {
        foreach (var item in errorMessageTexts)
        {
            Destroy(item.gameObject);
        }

        errorMessageTexts.Clear();

        var gameObject = Instantiate(errorMessageTextPrefab, errorMessageTextParent.transform);
        errorMessageTexts.Add(gameObject);
        gameObject.transform.SetAsLastSibling();
        gameObject.GetComponent<TextMeshProUGUI>().text = errorMessageTexts.Count + ". " + error;
        errorAnimation.SetTrigger("Open");
    }

    public void ChangePanel(int panelId)
    {
        loginEMailInput.text = "";
        loginPasswordInput.text = "";
        registerFullNameInput.text = "";
        registerEMailInput.text = "";
        registerPasswordInput.text = "";
        changePasswordCodeInput.text = "";
        changePasswordInput.text = "";

        resetPasswordPanel.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        changePasswordPanel.SetActive(false);

        switch (panelId)
        {
            case 1:
                loginPanel.SetActive(true);
                break;
            case 2:
                registerPanel.SetActive(true);
                break;
            case 3:
                resetPasswordPanel.SetActive(true);
                break;
            case 4:
                changePasswordPanel.SetActive(true);
                break;
        }
    }

    IEnumerator LoadAsynchronously(int sceneBuildIndex, int sceneBuildIndexToClose)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);

        while (!operation.isDone)
        {
            yield return null;
        }

        loadingAnimationPrefab.SetActive(false);
        SceneManager.UnloadSceneAsync(sceneBuildIndexToClose);
    }
}
