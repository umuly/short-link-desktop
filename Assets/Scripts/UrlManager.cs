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
using UnityEngine.UI;
using DanielLochner.Assets.SimpleSideMenu;
using UnityEngine.EventSystems;

public class UrlManager : MonoBehaviour
{
    // Base UMULY Address
    const string baseAddress = "https://umuly.com";

    // Redirect URL Panel
    [SerializeField] TMP_InputField redirectUrlInput;
    [SerializeField] TMP_InputField titleInput;
    [SerializeField] TMP_InputField descriptionInput;
    [SerializeField] TMP_InputField tagsInput;
    [SerializeField] TMP_Dropdown domainIdDropdown;
    [SerializeField] TMP_InputField codeInput;
    [SerializeField] TMP_Dropdown specificMembersOnlyDropdown;
    bool toggleOptions = false;
    [SerializeField] List<GameObject> optionsItems;
    [SerializeField] TextMeshProUGUI toggleOptionsButtonText;

    // My Panel
    [SerializeField] GameObject shortUrlsListParent;
    [SerializeField] GameObject urlListItemPrefab;

    // Domains
    List<MDomain.Response> allDomains;

    // Menu Items
    [SerializeField] GameObject menu;
    [SerializeField] SimpleSideMenu menuScript;
    [SerializeField] GameObject menuButton;
    [SerializeField] Sprite hamburgerImage;
    [SerializeField] Sprite rightArrowImage;

    // Panels
    [SerializeField] GameObject myPanel;
    [SerializeField] GameObject redirectUrlAddPanel;

    // Others
    EventSystem system;

    void Start()
    {
        StartCoroutine(GetMultipleShortRedirectURL());
        GetAllDomains();
        menuScript = menu.GetComponent<SimpleSideMenu>();
        menuScript.onStateChanging.AddListener(()=> ToggleMenu());
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
    }

    public Token tkn;
    public class Token
    {
        public string token { get; set; }
    }

    public void RedirectUrlAdd()
    {
        MRedirectUrl.Form form = new MRedirectUrl.Form();
        form.RedirectUrl = redirectUrlInput.text;
        form.Title = titleInput.text;
        form.Description = descriptionInput.text;
        form.Tags = tagsInput.text;
        form.DomainId = allDomains.Where(k => k.name == domainIdDropdown.captionText.text).FirstOrDefault().id;
        form.Code = codeInput.text;
        form.SpecificMembersOnly = ((int)Enum.GetValues(typeof(EnUrlAccessTypes)).GetValue(specificMembersOnlyDropdown.value)).ToString();
        form.UrlType = 1;

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

                var urlItem = Instantiate(urlListItemPrefab, shortUrlsListParent.transform).gameObject;
                urlItem.transform.SetAsFirstSibling();
                urlItem.GetComponentInChildren<TextMeshProUGUI>().text = rsp.item.shortUrl;
                urlItem.GetComponent<Button>().onClick.AddListener(() => CopyShortURL(rsp.item.shortUrl));
                ChangePanel(1);
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
        StartCoroutine(GetMultipleShortRedirectURL());
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
                var urlItem = Instantiate(urlListItemPrefab, shortUrlsListParent.transform).gameObject;
                urlItem.GetComponentInChildren<TextMeshProUGUI>().text = item.shortUrl;
                urlItem.GetComponent<Button>().onClick.AddListener(() => CopyShortURL(item.shortUrl));
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void ToggleOptions()
    {
        titleInput.text = "";
        descriptionInput.text = "";
        specificMembersOnlyDropdown.value = 0;

        if (toggleOptions)
        {
            toggleOptions = false;
            foreach (var item in optionsItems)
            {
                item.SetActive(false);
            }

            toggleOptionsButtonText.text = "Show Options";
        }
        else
        {
            toggleOptions = true;
            foreach (var item in optionsItems)
            {
                item.SetActive(true);
            }

            toggleOptionsButtonText.text = "Hide Options";
        }
    }

    public void ToggleMenu()
    {
        if (menuScript.CurrentState == SimpleSideMenu.State.Open)
        {
            menuButton.GetComponent<Image>().sprite = hamburgerImage;
        }
        else
        {
            menuButton.GetComponent<Image>().sprite = rightArrowImage;
        }
    }

    public void ChangePanel(int panelId)
    {
        redirectUrlInput.text = "";
        codeInput.text = "";

        myPanel.SetActive(false);
        redirectUrlAddPanel.SetActive(false);

        toggleOptions = false;
        foreach (var item in optionsItems)
        {
            item.SetActive(false);
        }

        toggleOptionsButtonText.text = "Show Options";

        switch (panelId)
        {
            case 1:
                myPanel.SetActive(true);
                break;
            case 2:
                redirectUrlAddPanel.SetActive(true);
                break;
            case 3:
                menuScript.ToggleState();
                Shortlinkdb<Player> db = new Shortlinkdb<Player>();
                var asd = db.Que("select * from Player").FirstOrDefault();
                db.Delete("delete from Player where Id=" + asd.Id + "");

                StartCoroutine(LoadAsynchronously(0, 1));
                break;
        }

        if (menuScript.CurrentState == SimpleSideMenu.State.Open)
        {
            menuScript.ToggleState();
        }
    }

    private void CopyShortURL(string shortUrl)
    {
        TextEditor editor = new TextEditor
        {
            text = shortUrl
        };

        editor.SelectAll();
        editor.Copy();
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
