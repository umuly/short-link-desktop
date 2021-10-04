using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UrlManager : MonoBehaviour
{
    [SerializeField] TMP_InputField PasteAreaText;
    
    void Start()
    {
        Debug.Log(tkn.token);
    }


    public void RedirectUrlAdd()
    {
        FRedirectUrl redirectUrl = new FRedirectUrl();
        redirectUrl.RedirectUrl =  PasteAreaText.text; //Buraya URL gelcek.
        redirectUrl.Title = ""; //Title 
        redirectUrl.Description = ""; //Desc
        redirectUrl.Tags = ""; // Tags
        redirectUrl.DomainId = ""; // Listede seçili domainin id si buraya gelcek.
        redirectUrl.Code = ""; //Code
        //UrlAccessTypes: Everyone: 1, Only Me: 2, Specific Members Only: 3, Members Only: 4 , Only Those Who Have The Link Can Access: 5 Bunlar dýþarýdan alýnacak.
        redirectUrl.UrlAccessType = "5";
        redirectUrl.SpecificMembersOnly = "";
        redirectUrl.UrlType = "1";

        StartCoroutine(RedirectUrlAdd(JsonUtility.ToJson(redirectUrl)));
    }

    public void GetAllDomains()
    {
        FDomain domain = new FDomain();
        domain.Authorization = "Bearer token"; //Bearer boþluk token gelecek.

        StartCoroutine(Domains(JsonUtility.ToJson(domain)));
    }

    IEnumerator RedirectUrlAdd(string bodyJsonString)
    {

        WWWForm form = new WWWForm();
        form.AddField("RedirectUrl", PasteAreaText.text);
        form.AddField("Title", "");
        form.AddField("Description","");
        form.AddField("Tags", "");
        form.AddField("DomainId", "");
        form.AddField("Code", "");
        form.AddField("UrlAccessType", "5");
        form.AddField("SpecificMembersOnly", "");
        form.AddField("UrlType", "1");

        UnityWebRequest www = UnityWebRequest.Post("https://umuly.com/api/url/RedirectUrlAdd", form);

        www.SetRequestHeader("Authorization", "Bearer " + tkn.token);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
             
            Debug.Log("Form upload complete!");
        }

    }
   

// Bütün domainler gelir.
IEnumerator Domains(string bodyJsonString)
    {




        var request = new UnityWebRequest("http://umuly.com/api/domains", UnityWebRequest.kHttpVerbGET);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 200)
            {
                Debug.Log("Baþarýlý!" + request.downloadHandler.text);
                //Domainleri listeye yazdýr.
            }
            else
            {
                Debug.Log("Uyarý: " + request.downloadHandler.text);
            }
        }
        else
        {
            Debug.Log("Bilinmeyen Hata: " + request.downloadHandler.text);
        }
    }
    public static Token tkn { get; set; }
}

public class Token
{
    public string token;
}

public class FRedirectUrl
{
    
    public string RedirectUrl;
    public string Title;
    public string Description;
    public string Tags;
    public string DomainId;
    public string Code;
    public string UrlAccessType;
    public string SpecificMembersOnly;
    public string UrlType;
}

public class FDomain
{
    public string Authorization;
}
