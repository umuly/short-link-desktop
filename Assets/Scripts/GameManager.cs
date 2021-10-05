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

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nameText;
    [SerializeField] TMP_InputField emailText;
    [SerializeField] TMP_InputField passwordText;
    [SerializeField] TMP_InputField emaillogintext;
    [SerializeField] TMP_InputField passwordloginText;
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject registerPanel;
    [SerializeField] GameObject forgotPanel;
    [SerializeField] TextMeshProUGUI errortext;

    private static readonly HttpClient client = new HttpClient();
    void Start()
    {
        emaillogintext.text = "caglar.cakmak@umuly.com";
        passwordloginText.text = "Caglar19.";

    }


    void Update()
    {

    }
    public void Login()
    {
        StartCoroutine(GetRequest("https://umuly.com/api/Token?Email=" + emaillogintext.text + "&Password=" + passwordloginText.text));
    }


    public void Reqister()
    {
        MRegister.Form user = new MRegister.Form();
        user.name = nameText.text;
        user.email = emailText.text;
        user.password = passwordText.text;

        StartCoroutine(Post("http://umuly.com/api/user", JsonConvert.SerializeObject(user)));
    }


    IEnumerator Post(string url, string bodyJsonString)
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
            //var rb = ResponseBase<string>.CreateFromJSON(request.downloadHandler.text);
            //Debug.Log(rb.statusText);
        }

        else
        {
            errortext.text = request.downloadHandler.text;
            errortext.gameObject.SetActive(true);

            Debug.Log("Status Code: " + request.responseCode + " Response: " + request.downloadHandler.text);
            //var rb = ResponseBase<string>.CreateFromJSON(request.downloadHandler.text);
            //Debug.Log(rb.statusText);

        }
    }
    IEnumerator GetRequest(string uri)
    {
        var request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        Debug.Log("Request Sended!");
        
        if (request.result== UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 200)

            {
                SceneManager.LoadScene(1);
                Token token = JsonUtility.FromJson<Token>(request.downloadHandler.text);
                UrlManager.tkn = token;
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
}
