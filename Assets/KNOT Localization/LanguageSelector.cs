using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Knot.Localization;
using System.Linq;

public class LanguageSelector : MonoBehaviour
{
    void Start()
    {
        Dropdown languageDropdown = GetComponent<Dropdown>();

        languageDropdown.AddOptions(KnotLocalization.Manager.Languages.Select(d => d.NativeName).ToList()); 
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

    }
    private void OnLanguageChanged(int arg0)
    {
        KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[arg0]);
    }
   
    void Update()
    {
        
    }
}
