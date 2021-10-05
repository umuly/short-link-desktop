using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class UrlManager : MonoBehaviour
{
    [SerializeField] TMP_InputField PasteAreaText;
    [SerializeField] TextMeshProUGUI ShortlinkText;

  
   void Start()
    {
        Debug.Log(tkn.token);
    }


    public void RedirectUrlAdd()
    {
        FRedirectUrl redirectUrl = new FRedirectUrl();
        redirectUrl.RedirectUrl = PasteAreaText.text; //Buraya URL gelcek.
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
        StartCoroutine(Domains());
    }

    public void GetShortUrl()
    {
        StartCoroutine(GetShortUrlById("URL ID GONDER"));
    }

    IEnumerator RedirectUrlAdd(string bodyJsonString)
    {

        WWWForm form = new WWWForm();
        form.AddField("RedirectUrl", PasteAreaText.text);
        form.AddField("Title", "");
        form.AddField("Description", "");
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
            ResponseBase<RpRedirectUrl> rp = new ResponseBase<RpRedirectUrl>();
         var rpsl=    JsonUtility.FromJson<ResponseBase<RpRedirectUrl>>(www.downloadHandler.text);
            Debug.Log(rpsl.item);
        }

    }

    // Bütün domainler gelir.
    IEnumerator Domains()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://umuly.com/api/domains");

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

    public static Token tkn { get; set; }

    IEnumerator GetShortUrlById(string urlId)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://umuly.com/api/url/" + urlId);

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
