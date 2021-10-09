using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Assets.Models;
using Assets.Scripts.Data;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UrlManager : MonoBehaviour
{
    [SerializeField] TMP_InputField PasteAreaText;
    [SerializeField] TextMeshProUGUI ShortlinkText;

    const string baseAddress = "https://umuly.com";

    [SerializeField] GameObject urlListParent;
    [SerializeField] GameObject urlListItemPrefab;

    public GameObject myPanel;
    public void HidePanel()
    {
        myPanel.gameObject.SetActive(false);
    }
    public void CloseScene()
    {
        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        var asd = db.Que("select * from Player").FirstOrDefault();
        db.Delete("delete from Player where Id="+asd.Id+"");
        SceneManager.LoadScene(0);
        SceneManager.UnloadSceneAsync(1);
    }
    void Start()
    {
        StartCoroutine(GetMultipleShortRedirectURL());
    }

    public Token tkn;
    public class Token
    {
        public string token { get; set; }
    }

    public void RedirectUrlAdd()
    {
        MRedirectUrl.Form form = new MRedirectUrl.Form();
        form.RedirectUrl = PasteAreaText.text; //Buraya URL gelcek.
        form.Title = ""; //Title 
        form.Description = ""; //Desc
        form.Tags = ""; // Tags
        form.DomainId = ""; // Listede se�ili domainin id si buraya gelcek.
        form.Code = ""; //Code
        //UrlAccessTypes: Everyone: 1, Only Me: 2, Specific Members Only: 3, Members Only: 4 , Only Those Who Have The Link Can Access: 5 Bunlar d��ar�dan al�nacak.
        form.SpecificMembersOnly = "";
        form.UrlType = 1;

        //StartCoroutine(RedirectUrlAdd(redirectUrl));


        string asd = form.UrlAccessType.GetHashCode().ToString();

        WWWForm wwwform = new WWWForm();
        wwwform.AddField("RedirectUrl", form.RedirectUrl);
        wwwform.AddField("Title", form.Title);
        wwwform.AddField("Description", form.Description);
        wwwform.AddField("Tags", form.Tags);
        wwwform.AddField("DomainId", form.DomainId);
        wwwform.AddField("Code", form.Code);
        wwwform.AddField("UrlAccessType", form.UrlAccessType.GetHashCode().ToString());
        wwwform.AddField("SpecificMembersOnly", form.SpecificMembersOnly);
        wwwform.AddField("UrlType", form.UrlType);

        UnityWebRequest www = UnityWebRequest.Post(baseAddress + "/api/url/RedirectUrlAdd", wwwform);

        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        string token = db.Que("select * from Player").FirstOrDefault().Token;

        www.SetRequestHeader("Authorization", "Bearer " + token);
        StartCoroutine(Operation());


        IEnumerator Operation()
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var rsp = JsonConvert.DeserializeObject<MResponseBase<MRedirectUrl.Response>>(www.downloadHandler.text);
                Debug.Log(rsp.item.shortUrl);

                var urlItem = Instantiate(urlListItemPrefab, urlListParent.transform).gameObject;
                urlItem.transform.SetAsFirstSibling();
                urlItem.GetComponentInChildren<TextMeshProUGUI>().text = rsp.item.shortUrl;


            }

        }



    }

    public void GetAllDomains()
    {
        StartCoroutine(Domains());
    }

    public void GetShortUrl()
    {
        StartCoroutine(GetShortUrlById("URL ID GONDER"));
    }


    // B�t�n domainler gelir.
    IEnumerator Domains()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseAddress + "/api/domains");

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

    IEnumerator GetShortUrlById(string urlId)
    {
        UnityWebRequest www = UnityWebRequest.Get(baseAddress + "/api/url/" + urlId);

        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        string token = db.Que("select * from Player").FirstOrDefault().Token;
        www.SetRequestHeader("Authorization", "Bearer " + token);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            Debug.Log("Form upload complete!");
        }
    }
    public void GetMultipleShortRedirectUR()
    {
        //StartCoroutine(GetMultipleShortRedirectURL());
    }
    IEnumerator GetMultipleShortRedirectURL()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseAddress + "/api/url?Skip=0&Sort.Column=createdOn&Sort.Type=1&UrlType=1"); //Skip=0&UrlType=1

        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        string token = db.Que("select * from Player").FirstOrDefault().Token;

        www.SetRequestHeader("Authorization", "Bearer " + token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var rsp = JsonConvert.DeserializeObject<MResponseBase<List<MRedirectUrl.Response>>>(www.downloadHandler.text);

            foreach (var item in rsp.item)
            {
            Debug.Log(item.shortUrl);
                var urlItem = Instantiate(urlListItemPrefab, urlListParent.transform).gameObject;
                urlItem.GetComponentInChildren<TextMeshProUGUI>().text = item.shortUrl;
            }
            
        }
        else
        {


            Debug.Log(www.error);



        }

    }
}
