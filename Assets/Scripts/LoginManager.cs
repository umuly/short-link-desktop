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
using System.Threading;

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
    [SerializeField] TextMeshProUGUI errorText;

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
            errorText.text = ex.Message;
        }
    }

    public void Login()
    {
        errorText.text = "";
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
            if (request.responseCode == 200)
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
            else
            {
                errorText.text = "Unknown Error!";
            }
        }
        else
        {
            errorText.text = "Unknown Error!";
        }
    }

    public void Reqister()
    {
        errorText.text = "";

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

        if (request.result == UnityWebRequest.Result.Success)
        {
            ///////////////////////////////////////////////////////////////////////
            if (request.responseCode == 200)
            {

            }
            else
            {

            }
        }
        else
        {
            errorText.text = "Unknown Error!";
        }
    }

    public void ResetPassword()
    {
        errorText.text = "";

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

        if (request.result == UnityWebRequest.Result.Success)
        {
            ///////////////////////////////////////////////////////////////////////
            if (request.responseCode == 200)
            {
                ChangePanel(4);
            }
            else
            {

            }
        }
        else
        {
            errorText.text = "Unknown Error!";
        }
    }

    public void ChangePassword()
    {
        errorText.text = "";

        MUser.Form changePasswordForm = new MUser.Form();
        changePasswordForm.email = resetPasswordEMailInput.text;
        changePasswordForm.password = changePasswordCodeInput.text;
        changePasswordForm.code = changePasswordInput.text;

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
        if (request.result == UnityWebRequest.Result.Success)
        {
            ///////////////////////////////////////////////////////////////////////
            if (request.responseCode == 200)
            {
                if (request.responseCode == 200)
                {
                    ChangePanel(1);
                }
            }
            else
            {

            }
        }
        else
        {
            errorText.text = "Unknown Error!";
        }
    }

    public void ChangePanel(int panelId)
    {
        errorText.text = "";
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

        SceneManager.UnloadSceneAsync(sceneBuildIndexToClose);
    }
}
