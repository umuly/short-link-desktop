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
        TMP_Dropdown languageDropdown = GetComponent<TMP_Dropdown>();

        languageDropdown.AddOptions(KnotLocalization.Manager.Languages.Select(d => d.NativeName).ToList()); 
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        _dropdown.value = PlayerPrefs.GetInt(PrefName, 0);
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
