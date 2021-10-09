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
using Assets.Scripts.Models;
using Assets.Scripts.Enums;
using System;

public class UrlManager : MonoBehaviour
{
    [SerializeField] TMP_InputField PasteAreaText;
    [SerializeField] TextMeshProUGUI ShortlinkText;

    const string baseAddress = "https://umuly.com";

    [SerializeField] GameObject urlListParent;
    [SerializeField] GameObject urlListItemPrefab;

    // Redirect URL Add
    [SerializeField] TMP_InputField redirectUrlInput;
    [SerializeField] TMP_InputField titleInput;
    [SerializeField] TMP_InputField descriptionInput;
    [SerializeField] TMP_InputField tagsInput;
    [SerializeField] TMP_Dropdown domainIdDropdown;
    [SerializeField] TMP_InputField codeInput;
    [SerializeField] TMP_Dropdown urlTypeDropdown;

    // Domains
    List<MDomain.Response> allDomains;

    public GameObject myPanel;
    public void HidePanel()
    {
        myPanel.gameObject.SetActive(false);
    }
    public void CloseScene()
    {
        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        var asd = db.Que("select * from Player").FirstOrDefault();
        db.Delete("delete from Player where Id=" + asd.Id + "");

        SceneManager.LoadScene(0, LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
        SceneManager.UnloadSceneAsync(1);
    }

    void Start()
    {
        StartCoroutine(GetMultipleShortRedirectURL());
        GetAllDomains();
    }

    public Token tkn;
    public class Token
    {
        public string token { get; set; }
    }

    public void RedirectUrlAdd()
    {
        MRedirectUrl.Form form = new MRedirectUrl.Form();
        form.RedirectUrl = PasteAreaText.text;
        form.Title = titleInput.text;
        form.Description = descriptionInput.text;
        form.Tags = tagsInput.text;
        form.DomainId = allDomains.Where(k=>k.name == domainIdDropdown.captionText.text).FirstOrDefault().id;
        form.Code = codeInput.text;
        form.SpecificMembersOnly = ((int)Enum.GetValues(typeof(EnUrlAccessTypes)).GetValue(urlTypeDropdown.value)).ToString();
        form.UrlType = urlTypeDropdown.value;

        Debug.Log(form.DomainId);


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

    IEnumerator Domains()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseAddress + "/api/domains");

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
            var rsp = JsonConvert.DeserializeObject<MResponseBase<List<MDomain.Response>>>(www.downloadHandler.text);
            domainIdDropdown.ClearOptions();
            allDomains = new List<MDomain.Response>();

            foreach (var item in rsp.item)
            {
                Debug.Log("Domain" + item.id);
                allDomains.Add(item);
                domainIdDropdown.AddOptions(new List<string> { item.name });
            }


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
