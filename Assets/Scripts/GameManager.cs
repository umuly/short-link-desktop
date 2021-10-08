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

public class GameManager : MonoBehaviour
{
    // Register
    [SerializeField] TMP_InputField nameText;
    [SerializeField] TMP_InputField emailText;
    [SerializeField] TMP_InputField passwordText;

    // Login
    [SerializeField] TMP_InputField emaillogintext;
    [SerializeField] TMP_InputField passwordloginText;


    // Reset Password
    [SerializeField] TMP_InputField emailforgottext;
    [SerializeField] TMP_InputField passwordResetInput;
    [SerializeField] TMP_InputField codeResetInput;

    // Panels
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject registerPanel;
    [SerializeField] GameObject forgotPanel;
    [SerializeField] GameObject changePanel;

    [SerializeField] TextMeshProUGUI errortext;

    public string tkn;

    void Start()
    {
        Shortlinkdb<Player> shortlinkdb = new Shortlinkdb<Player>();
        var asd = shortlinkdb.Que("select * from Player").FirstOrDefault();
        if (asd != null)
        {
            SceneManager.LoadScene(1);
        }

    }

    public void Login()
    {
        StartCoroutine(Login("https://umuly.com/api/Token?Email=" + emaillogintext.text + "&Password=" + passwordloginText.text));
    }

    IEnumerator Login(string uri)
    {
        var request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        Debug.Log("Request Sended!");

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

                SceneManager.LoadScene(1);
            }
            else
            {
                errortext.text = request.downloadHandler.text;
            }
        }
        else
        {
            errortext.text = request.downloadHandler.text;
        }
    }

    public void Reqister()
    {
        MUser.Form user = new MUser.Form();
        user.name = nameText.text;
        user.email = emailText.text;
        user.password = passwordText.text;

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

        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 500)
            {
                errortext.text = "Unknown Error!";
                errortext.gameObject.SetActive(true);
            }
            else
            {
                errortext.text = request.downloadHandler.text;
                errortext.gameObject.SetActive(true);
            }

            Debug.Log("Status Code: " + request.responseCode + " Response: " + request.downloadHandler.text);
        }
        else
        {
            errortext.text = request.downloadHandler.text;
            errortext.gameObject.SetActive(true);

            Debug.Log("Status Code: " + request.responseCode + " Response: " + request.downloadHandler.text);

        }
    }

    public void ResetPassword()
    {
        MUser.Form user = new MUser.Form();
        user.email = emailforgottext.text;

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

        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 500)
            {
                errortext.text = "Unknown Error!";
                errortext.gameObject.SetActive(true);
            }
            else
            {
                errortext.text = request.downloadHandler.text;
                errortext.gameObject.SetActive(true);
            }

            Debug.Log("Status Code: " + request.responseCode + " Response: " + request.downloadHandler.text);
        }
        else
        {
            errortext.text = request.downloadHandler.text;
            errortext.gameObject.SetActive(true);

            Debug.Log("Status Code: " + request.responseCode + " Response: " + request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                changePanel.SetActive(true);
            }
        }
    }

    public void ChangePassword()
    {
        MUser.Form changePasswordForm = new MUser.Form();
        changePasswordForm.email = emailforgottext.text;
        changePasswordForm.password = passwordResetInput.text;
        changePasswordForm.code = codeResetInput.text;

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

        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 500)
            {
                errortext.text = "Unknown Error!";
                errortext.gameObject.SetActive(true);
            }
            else
            {
                errortext.text = request.downloadHandler.text;
                errortext.gameObject.SetActive(true);
            }

            Debug.Log("Status Code: " + request.responseCode + " Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode + " Response: " + request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                changePanel.SetActive(false);
                loginPanel.SetActive(true);
            }
        }
    }

    public void ChangePanel(int panelId)
    {
        forgotPanel.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);

        switch (panelId)
        {
            case 1:
                loginPanel.SetActive(true);
                break;
            case 2:
                registerPanel.SetActive(true);
                break;
            case 3:
                forgotPanel.SetActive(true);

                break;
            default:
                break;
        }
    }

    void ReadToken()
    {

        string conn = "URI=file:" + Application.dataPath + "/shortlinkdb.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * " + "FROM token";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            string token = reader.GetString(0);

            Debug.Log("Token = " + token);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    void AddToken()
    {

        string conn = "URI=file:" + Application.dataPath + "/shortlinkdb.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "INSERT INTO token VALUES ('')";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
}
