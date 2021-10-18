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
using UnityEngine.EventSystems;
using System.IO;

public class UrlManager : MonoBehaviour
{
    // Base UMULY Address
    const string baseAddress = "https://umuly.com";

    // Redirect URL Panel
    [SerializeField] TMP_Dropdown redirectUrlType;
    [SerializeField] TMP_InputField redirectUrlInput;
    [SerializeField] TMP_InputField titleInput;
    [SerializeField] TMP_InputField descriptionInput;
    [SerializeField] TMP_InputField tagsInput;
    [SerializeField] TMP_Dropdown domainIdDropdown;
    [SerializeField] TMP_InputField codeInput;
    [SerializeField] TMP_Dropdown accessTypeDropdown;
    [SerializeField] TMP_InputField specificMembers;

    // Domains
    List<MDomain.Response> allDomains;

    // Others
    EventSystem system;

    // Short Urls List Panel
    [SerializeField] GameObject addLinkPanel;
    [SerializeField] GameObject contentContainer;
    [SerializeField] GameObject contentContainerItem;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        StartCoroutine(GetMultipleShortRedirectURL());
        GetAllDomains();
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

    public void RedirectUrlAdd()
    {
        MRedirectUrl.Form form = new MRedirectUrl.Form();
        form.RedirectUrl = redirectUrlInput.text;
        form.Title = titleInput.text;
        form.Description = descriptionInput.text;
        form.Tags = tagsInput.text;
        form.DomainId = allDomains.Where(k => k.domainUrl == domainIdDropdown.captionText.text).FirstOrDefault().id;
        form.Code = codeInput.text;
        form.UrlAccessType = (EnUrlAccessTypes)Enum.GetValues(typeof(EnUrlAccessTypes)).GetValue(accessTypeDropdown.value);
        form.SpecificMembersOnly = specificMembers.text;
        form.UrlType = redirectUrlType.value + 1;

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

                var urlItem = Instantiate(contentContainerItem, contentContainer.transform).gameObject;

                GetMultipleShortRedirectUR();
                addLinkPanel.GetComponent<Animator>().SetTrigger("Open");
            }
        }
    }

    public void OpenUrl(string uri)
    {
        Application.OpenURL(uri);
    }

    public void GetAllDomains()
    {
        StartCoroutine(Domains());
    }

    public void GetShortUrl(string urlId)
    {
        StartCoroutine(GetShortUrlById(urlId));
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
                domainIdDropdown.AddOptions(new List<string> { item.domainUrl });
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
            ClearInputs();
            MResponseBase<MRedirectUrl.Response> rsp = JsonConvert.DeserializeObject<MResponseBase<MRedirectUrl.Response>>(www.downloadHandler.text);
            redirectUrlInput.text = rsp.item.shortUrl;
            titleInput.text = rsp.item.title;
            descriptionInput.text = rsp.item.description;
            tagsInput.text = rsp.item.tags;
            codeInput.text = rsp.item.code;
            redirectUrlType.value = rsp.item.urlType - 1;
            domainIdDropdown.captionText.text = allDomains.FirstOrDefault(k => k.domainUrl == domainIdDropdown.captionText.text).domainUrl;
            accessTypeDropdown.value = rsp.item.urlAccessType - 1;
            specificMembers.text = rsp.item.specificMembersOnly;

            addLinkPanel.GetComponent<Animator>().SetTrigger("Open");
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
            MResponseBase<List<MRedirectUrl.Response>> rsp = JsonConvert.DeserializeObject<MResponseBase<List<MRedirectUrl.Response>>>(www.downloadHandler.text);


            foreach (MRedirectUrl.Response response in rsp.item)
            {
                var urlItem = Instantiate(contentContainerItem, contentContainer.transform).gameObject;
                TextMeshProUGUI[] textMeshProUGUIs = urlItem.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var textMeshProUGUI in textMeshProUGUIs)
                {
                    if (textMeshProUGUI.name == "Count - Text")
                    {
                        textMeshProUGUI.text = (rsp.item.IndexOf(response) + 1).ToString() + '.';
                    }
                    else if (textMeshProUGUI.name == "Active - Text")
                    {
                        switch (response.status)
                        {
                            case -4:
                                textMeshProUGUI.text = "Error";
                                break;
                            case -3:
                                textMeshProUGUI.text = "Passive";
                                break;
                            case -2:
                                textMeshProUGUI.text = "Waiting For Delete";
                                break;
                            case -1:
                                textMeshProUGUI.text = "Abuse";
                                break;
                            case 1:
                                textMeshProUGUI.text = "Active";
                                break;
                        }
                    }
                    else if (textMeshProUGUI.name == "Date - Text")
                    {
                        textMeshProUGUI.text = response.createdOn.ToString("MM/dd/yyyy HH:mm");
                    }
                    else if (textMeshProUGUI.name == "Link - Text")
                    {
                        textMeshProUGUI.text = response.code;
                    }
                    else if (textMeshProUGUI.name == "Title - Text")
                    {
                        textMeshProUGUI.text = response.title;
                    }
                    else if (textMeshProUGUI.name == "Description - Text")
                    {
                        textMeshProUGUI.text = response.description;
                    }
                    else if (textMeshProUGUI.name == "Tag - Text")
                    {
                        textMeshProUGUI.text = response.tags;
                    }
                }

                LayoutElement[] layoutElements = urlItem.GetComponentsInChildren<LayoutElement>();
                foreach (var layoutElement in layoutElements)
                {
                    if (layoutElement.name == "Title")
                    {
                        if (string.IsNullOrEmpty(response.title))
                        {
                            layoutElement.gameObject.SetActive(false);
                        }
                    }
                    else if (layoutElement.name == "Description")
                    {
                        if (string.IsNullOrEmpty(response.description))
                        {
                            layoutElement.gameObject.SetActive(false);
                        }
                    }
                    else if (layoutElement.name == "Tags")
                    {
                        if (string.IsNullOrEmpty(response.tags))
                        {
                            layoutElement.gameObject.SetActive(false);
                        }
                    }
                }

                Button[] buttons = urlItem.GetComponentsInChildren<Button>();
                foreach (var button in buttons)
                {
                    if (button.name == "Copy")
                    {
                        button.onClick.AddListener(() => CopyShortURL(response.shortUrl));
                    }
                    else if (button.name == "Share")
                    {
                        button.onClick.AddListener(() => StartCoroutine(ShareUrl("Short URL", "Short URL", response.shortUrl)));
                    }
                    else if (button.name == "Trash")
                    {
                        // Trash API
                        button.onClick.AddListener(() => Destroy(urlItem));
                    }
                    else if (button.name == "Edit")
                    {
                        // Edit API
                        button.onClick.AddListener(() => GetShortUrl(response.id));
                    }
                    else if (button.name == "Link Button")
                    {
                        button.onClick.AddListener(() => Application.OpenURL(response.shortUrl));
                    }
                    else if (button.name == "Bullhorn")
                    {
                        button.onClick.AddListener(() => Application.OpenURL("https://umuly.com/panel/my-short-urls"));
                    }
                    else if (button.name == "Grafik")
                    {
                        button.onClick.AddListener(() => Application.OpenURL("https://umuly.com/panel/my-short-urls"));
                    }
                }
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    private IEnumerator ShareUrl(string text, string subject, string url)
    {
        yield return new WaitForEndOfFrame();

        //new NativeShare()
        //    .SetSubject(subject).SetText(text).SetUrl(url)
        //    .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget + ", Data: " + url))
        //    .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }

    public void SetUrlType()
    {
        switch (redirectUrlType.value)
        {
            case 0:
                break;
            case 1:
                redirectUrlType.value = 0;
                Application.OpenURL("https://umuly.com/panel/my-short-urls");
                break;
            case 2:
                redirectUrlType.value = 0;
                Application.OpenURL("https://umuly.com/panel/my-short-urls");
                break;
            case 3:
                redirectUrlType.value = 0;
                Application.OpenURL("https://umuly.com/panel/my-short-urls");
                break;
            case 4:
                redirectUrlType.value = 0;
                Application.OpenURL("https://umuly.com/panel/my-short-urls");
                break;
            case 5:
                redirectUrlType.value = 0;
                Application.OpenURL("https://umuly.com/panel/my-short-urls");
                break;
            default:
                redirectUrlType.value = 0;
                break;
        }
    }

    public void LogOut()
    {
        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        var asd = db.Que("select * from Player").FirstOrDefault();
        db.Delete("delete from Player where Id=" + asd.Id + "");

        StartCoroutine(LoadAsynchronously(0, 1));
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

    public void ClearInputs()
    {
        redirectUrlInput.text = "";
        titleInput.text = "";
        descriptionInput.text = "";
        tagsInput.text = "";
        codeInput.text = "";
        redirectUrlType.value = 0;
        domainIdDropdown.value = 0;
        accessTypeDropdown.value = 0;
    }

    public void SpecificMemberToggle(GameObject specificPanel)
    {
        if (accessTypeDropdown.captionText.text == "Specific Members Only")
        {
            specificPanel.SetActive(true);
        }
        else
        {
            specificPanel.SetActive(false);
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
