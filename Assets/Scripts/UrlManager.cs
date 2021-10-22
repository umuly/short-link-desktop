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
using UnityEditor;

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
    [SerializeField] TextMeshProUGUI domainText;
    [SerializeField] TMP_InputField codeInput;
    [SerializeField] TMP_Dropdown accessTypeDropdown;
    [SerializeField] TMP_InputField specificMembers;

    // Domains
    List<MDomain.Response> allDomains;

    // Others
    EventSystem system;
    bool isEdit = false;
    [SerializeField] GameObject deleteControlPanel;
    [SerializeField] TextMeshProUGUI createUrlBannerText;
    string lastUrlId = "";
    string tempDomainText = "";
    [SerializeField] GameObject createPlus;

    // Short Urls List Panel
    [SerializeField] GameObject addLinkPanel;
    [SerializeField] GameObject contentContainer;
    [SerializeField] GameObject contentContainerItem;
    [SerializeField] Button skipButton;
    [SerializeField] List<GameObject> allShortUrls = null;
    [SerializeField] int redirectUrlSkip = 0;
    int skipCount = 0;
    bool skip = false;
    [SerializeField] MResponseBase<MRedirectUrl.Response> lastResponse = null;

    // Animators
    [SerializeField] Animator copyAnimation;
    [SerializeField] GameObject loadingAnimationPrefab;

    // Alert Panel
    [SerializeField] Animator errorAnimation;
    [SerializeField] GameObject errorMessageTextPrefab;
    [SerializeField] GameObject errorMessageTextParent;
    [SerializeField] List<GameObject> errorMessageTexts;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        StartCoroutine(GetMultipleShortRedirectURL());
        GetAllDomains();
        system = EventSystem.current;
        tempDomainText = domainText.text;
        codeInput.onValueChanged.AddListener(delegate
        {
            domainText.text = tempDomainText + "/" + codeInput.text;
        });
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
        loadingAnimationPrefab.SetActive(true);

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

        UnityWebRequest request;

        if (isEdit)
        {
            wwwform.AddField("Id", lastResponse.item.id);
            request = UnityWebRequest.Post(baseAddress + "/api/url/RedirectUrlUpdate", wwwform);
        }
        else
        {
            createUrlBannerText.text = "CREATE SHORT URL";
            request = UnityWebRequest.Post(baseAddress + "/api/url/RedirectUrlAdd", wwwform);
        }

        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        string token = db.Que("select * from Player").FirstOrDefault().Token;

        request.SetRequestHeader("Authorization", "Bearer " + token);
        StartCoroutine(Operation());


        IEnumerator Operation()
        {
            yield return request.SendWebRequest();
            yield return new WaitForSecondsRealtime(2);

            try
            {
                var response = JsonConvert.DeserializeObject<MResponseBase<MRedirectUrl.Response>>(request.downloadHandler.text);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (request.responseCode == 200)
                    {
                        foreach (var item in allShortUrls)
                        {
                            Destroy(item.gameObject);
                        }

                        allShortUrls.Clear();
                        GetMultipleShortRedirectUR();
                        addLinkPanel.GetComponent<Animator>().SetTrigger("Close");
                        isEdit = false;
                        createUrlBannerText.text = "CREATE SHORT URL";
                        ConvertErrorsToString(null, "Short link successfully create!");
                    }
                    else
                    {
                        ConvertErrorsToString(response.errors, response.statusText);
                    }
                }
                else
                {
                    ConvertErrorsToString(response.errors, response.statusText);
                }

                loadingAnimationPrefab.SetActive(false);
            }
            catch (Exception)
            {
                ConvertErrorsToString(null, request.downloadHandler.text);
                loadingAnimationPrefab.SetActive(false);
            }
        }
    }

    public void OpenUrl(string uri)
    {
        Application.OpenURL(uri);
    }

    public void GetAllDomains()
    {
        loadingAnimationPrefab.SetActive(true);
        StartCoroutine(Domains());
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
            //Debug.Log(www.error);
            loadingAnimationPrefab.SetActive(false);
        }
        else
        {
            var rsp = JsonConvert.DeserializeObject<MResponseBase<List<MDomain.Response>>>(www.downloadHandler.text);
            domainIdDropdown.ClearOptions();
            allDomains = new List<MDomain.Response>();

            domainText.text = rsp.item[0].domainUrl;
            tempDomainText = domainText.text;
            foreach (var item in rsp.item)
            {
                allDomains.Add(item);
                domainIdDropdown.AddOptions(new List<string> { item.domainUrl });
            }

            domainIdDropdown.onValueChanged.AddListener(delegate
            {
                domainText.text = allDomains[domainIdDropdown.value].domainUrl + "/" + codeInput.text;
            });
            loadingAnimationPrefab.SetActive(false);
        }
    }

    public void GetShortUrl(string urlId)
    {
        loadingAnimationPrefab.SetActive(true);
        isEdit = true;
        createUrlBannerText.text = "UPDATE SHORT URL";
        StartCoroutine(GetShortUrlById(urlId));
    }

    IEnumerator GetShortUrlById(string urlId)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseAddress + "/api/url/" + urlId);

        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        string token = db.Que("select * from Player").FirstOrDefault().Token;
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        try
        {
            MResponseBase<MRedirectUrl.Response> response = JsonConvert.DeserializeObject<MResponseBase<MRedirectUrl.Response>>(request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                ClearInputs();

                if (request.responseCode == 200)
                {
                    lastResponse = null;
                    lastResponse = response;
                    redirectUrlInput.text = response.item.redirectUrl;

                    redirectUrlType.value = response.item.urlType - 1;
                    domainIdDropdown.value = allDomains.IndexOf(allDomains.FirstOrDefault(k => k.id == response.item.domainID));
                    domainText.text = response.item.shortUrl;
                    tempDomainText = allDomains.FirstOrDefault(k => k.id == response.item.domainID).domainUrl;

                    titleInput.text = response.item.title;
                    descriptionInput.text = response.item.description;
                    tagsInput.text = response.item.tags;
                    codeInput.text = response.item.code;
                    accessTypeDropdown.value = response.item.urlAccessType - 1;
                    specificMembers.text = response.item.specificMembersOnly;

                    addLinkPanel.GetComponent<Animator>().SetTrigger("Open");
                }
                else
                {
                    ConvertErrorsToString(response.errors, response.statusText);
                }
            }
            else
            {
                ConvertErrorsToString(response.errors, response.statusText);
            }

            loadingAnimationPrefab.SetActive(false);
        }
        catch (Exception)
        {
            ConvertErrorsToString(null, request.downloadHandler.text);
            loadingAnimationPrefab.SetActive(false);
        }
    }

    public void GetMultipleShortRedirectUR()
    {
        loadingAnimationPrefab.SetActive(true);
        StartCoroutine(GetMultipleShortRedirectURL());
    }

    IEnumerator GetMultipleShortRedirectURL()
    {
        UnityWebRequest request = UnityWebRequest.Get(baseAddress + "/api/url?Skip=" + redirectUrlSkip + "&Sort.Column=createdOn&Sort.Type=1&UrlType=1&Status=1");

        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        string token = db.Que("select * from Player").FirstOrDefault().Token;

        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            MResponseBase<List<MRedirectUrl.Response>> rsp = JsonConvert.DeserializeObject<MResponseBase<List<MRedirectUrl.Response>>>(request.downloadHandler.text);

            if (rsp.itemCount == 0)
            {
                createPlus.SetActive(true);
            }
            else
            {
                createPlus.SetActive(false);
            }

            skipCount = rsp.skipCount;

            if (!skip)
            {
                foreach (var item in allShortUrls)
                {
                    Destroy(item.gameObject);
                }

                allShortUrls.Clear();
            }

            skip = false;

            foreach (MRedirectUrl.Response response in rsp.item)
            {
                var urlItem = Instantiate(contentContainerItem, contentContainer.transform).gameObject;
                allShortUrls.Add(urlItem);
                urlItem.name = response.id;
                TextMeshProUGUI[] textMeshProUGUIs = urlItem.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var textMeshProUGUI in textMeshProUGUIs)
                {
                    if (textMeshProUGUI.name == "Count - Text")
                    {
                        textMeshProUGUI.text = allShortUrls.Count + ".";
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
                    else if (textMeshProUGUI.name == "Grafik - Text")
                    {
                        textMeshProUGUI.text = response.visitCount + " - " + response.uniqueVisitorCount;
                    }
                    else if (textMeshProUGUI.name == "Bulhorn - Text")
                    {
                        textMeshProUGUI.text = response.revenueAmount.ToString("$ #,0.00");
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

                // Butonlar d�n�l�r iste�e ba�l� event atanabilir.
                Button[] buttons = urlItem.GetComponentsInChildren<Button>();
                foreach (var button in buttons)
                {
                    if (button.name == "Copy")
                    {
                        button.onClick.AddListener(() => CopyShortURL(response.shortUrl));
                        button.onClick.AddListener(() => copyAnimation.SetTrigger("Open"));
                    }
                    else if (button.name == "Share")
                    {
                        string text = "";

                        if (!string.IsNullOrEmpty(response.title))
                        {
                            text += response.title + "\n";
                        }

                        if (!string.IsNullOrEmpty(response.description))
                        {
                            text += response.description + "\n";
                        }

                        button.onClick.AddListener(() => StartCoroutine(ShareUrl(text, "", response.shortUrl)));
                    }
                    else if (button.name == "Trash")
                    {
                        // Trash API
                        button.onClick.AddListener(() =>
                        {
                            deleteControlPanel.SetActive(true);
                            lastUrlId = urlItem.name;
                        });
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

            if (skipCount > 1)
            {
                skipButton.gameObject.SetActive(true);
                skipButton.transform.SetAsLastSibling();
            }

            if (skipCount <= redirectUrlSkip + 1)
            {
                skipButton.gameObject.SetActive(false);
            }
            loadingAnimationPrefab.SetActive(false);
        }
        else
        {
            loadingAnimationPrefab.SetActive(false);
        }
    }

    private IEnumerator ShareUrl(string text, string subject, string url)
    {
        yield return new WaitForEndOfFrame();

        new NativeShare()
            //.SetSubject(subject)
            .SetText(text)
            .SetUrl(url)
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget + ", Data: " + url))
            .Share();

        //Share on WhatsApp only, if installed(Android only)
        //if (NativeShare.TargetExists("com.whatsapp"))
        //    new NativeShare().AddFile(filePath).AddTarget("com.whatsapp").Share();
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
        loadingAnimationPrefab.SetActive(true);
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
        redirectUrlInput.text = "https://";
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

    public void SkipUrls()
    {
        skip = true;

        if (skipCount > redirectUrlSkip + 1)
        {
            redirectUrlSkip++;
            GetMultipleShortRedirectUR();
        }
    }

    public void ToggleDeletePanel(int id)
    {
        switch (id)
        {
            case 0:
                deleteControlPanel.SetActive(false);
                break;
            case 1:
                loadingAnimationPrefab.SetActive(true);
                StartCoroutine(DeleteShortUrl(lastUrlId));
                break;
        }
    }

    public void ConvertErrorsToString(Dictionary<string, string[]> errors, string statusText)
    {
        foreach (var item in errorMessageTexts)
        {
            Destroy(item.gameObject);
        }

        errorMessageTexts.Clear();

        if (errors != null)
        {
            foreach (string[] property in errors.Values)
            {
                foreach (var error in property)
                {
                    var gameObject = Instantiate(errorMessageTextPrefab, errorMessageTextParent.transform);
                    errorMessageTexts.Add(gameObject);
                    gameObject.transform.SetAsLastSibling();
                    gameObject.GetComponent<TextMeshProUGUI>().text = errorMessageTexts.Count + ". " + error;
                }
            }
        }

        if (!string.IsNullOrEmpty(statusText))
        {
            var gameObject = Instantiate(errorMessageTextPrefab, errorMessageTextParent.transform);
            errorMessageTexts.Add(gameObject);
            gameObject.transform.SetAsLastSibling();
            gameObject.GetComponent<TextMeshProUGUI>().text = errorMessageTexts.Count + ". " + statusText;
        }

        errorAnimation.SetTrigger("Open");
    }

    IEnumerator DeleteShortUrl(string urlId)
    {
        UnityWebRequest request = UnityWebRequest.Delete(baseAddress + "/api/url/" + urlId);

        Shortlinkdb<Player> db = new Shortlinkdb<Player>();
        string token = db.Que("select * from Player").FirstOrDefault().Token;

        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            yield return new WaitForSecondsRealtime(2);
            Destroy(GameObject.Find(lastUrlId));
            GetMultipleShortRedirectUR();
            deleteControlPanel.SetActive(false);
            loadingAnimationPrefab.SetActive(false);
        }
        else
        {
            ConvertErrorsToString(null, "Error on deleting! Please refresh the page.");
            yield return new WaitForSecondsRealtime(2);
            GetMultipleShortRedirectUR();
            deleteControlPanel.SetActive(false);
            loadingAnimationPrefab.SetActive(false);
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
