using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    static public Action <int> ChangeLanguage;
    public Dropdown dropdown;
    public Text label;

    public void LanguageChange()
    {
        if (ChangeLanguage != null)
            ChangeLanguage(dropdown.value);
        dropdown.captionText.text = CVSParser.GetAvailableLanguages()[dropdown.value];
 }
    void Start()
    {
        PopulateDropdown();
    }

   void PopulateDropdown()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(CVSParser.GetAvailableLanguages());
        LanguageChange();
    }
}
