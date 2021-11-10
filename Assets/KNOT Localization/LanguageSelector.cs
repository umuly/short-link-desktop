using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Knot.Localization;
using System.Linq;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Dropdown))]

public class LanguageSelector : MonoBehaviour
{
    const string PrefName = "optionvalue";

    private TMP_Dropdown _dropdown;
    void Start()
    {
        int defultT = 0;
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Afrikaans:
                break;
            case SystemLanguage.Arabic:
                break;
            case SystemLanguage.Basque:
                break;
            case SystemLanguage.Belarusian:
                break;
            case SystemLanguage.Bulgarian:
                break;
            case SystemLanguage.Catalan:
                break;
            case SystemLanguage.Chinese:
                break;
            case SystemLanguage.Czech:
                break;
            case SystemLanguage.Danish:
                break;
            case SystemLanguage.Dutch:
                break;
            case SystemLanguage.English:
                defultT = 0;
                break;
            case SystemLanguage.Estonian:
                break;
            case SystemLanguage.Faroese:
                break;
            case SystemLanguage.Finnish:
                break;
            case SystemLanguage.French:
                defultT = 3;
                break;
            case SystemLanguage.German:
                defultT = 2;
                break;
            case SystemLanguage.Greek:
                break;
            case SystemLanguage.Hebrew:
                break;
            case SystemLanguage.Hungarian:
                break;
            case SystemLanguage.Icelandic:
                break;
            case SystemLanguage.Indonesian:
                break;
            case SystemLanguage.Italian:
                defultT = 6;
                break;
            case SystemLanguage.Japanese:
                break;
            case SystemLanguage.Korean:
                break;
            case SystemLanguage.Latvian:
                break;
            case SystemLanguage.Lithuanian:
                break;
            case SystemLanguage.Norwegian:
                break;
            case SystemLanguage.Polish:
                break;
            case SystemLanguage.Portuguese:
                defultT = 5;
                break;
            case SystemLanguage.Romanian:
                break;
            case SystemLanguage.Russian:
                defultT = 7;
                break;
            case SystemLanguage.SerboCroatian:
                break;
            case SystemLanguage.Slovak:
                break;
            case SystemLanguage.Slovenian:
                break;
            case SystemLanguage.Spanish:
                defultT = 4;
                break;
            case SystemLanguage.Swedish:
                break;
            case SystemLanguage.Thai:
                break;
            case SystemLanguage.Turkish:
                defultT = 1;
                break;
            case SystemLanguage.Ukrainian:
                break;
            case SystemLanguage.Vietnamese:
                break;
            case SystemLanguage.ChineseSimplified:
                break;
            case SystemLanguage.ChineseTraditional:
                break;
            case SystemLanguage.Unknown:
                break;
            default:
                break;
        }


        TMP_Dropdown languageDropdown = GetComponent<TMP_Dropdown>();
        languageDropdown.AddOptions(KnotLocalization.Manager.Languages.Select(d => d.NativeName).ToList()); 
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        _dropdown.value = PlayerPrefs.GetInt(PrefName, defultT);

    }
    private void OnLanguageChanged(int arg0)
    {
        KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[arg0]);
    }

    void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();

        _dropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(PrefName, _dropdown.value);
            PlayerPrefs.Save();
        }));
    }


}
