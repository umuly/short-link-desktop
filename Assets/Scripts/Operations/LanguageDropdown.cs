using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown LanguageDrop;
    [SerializeField] TextMeshProUGUI Label;
    // Login Inputs
    [SerializeField] TMP_InputField loginEMailInput;
    [SerializeField] TMP_InputField loginPasswordInput;

    // Register Inputs
    [SerializeField] TMP_InputField registerFullNameInput;
    [SerializeField] TMP_InputField registerEMailInput;
    [SerializeField] TMP_InputField registerPasswordInput;

    // Reset Password Inputs
    [SerializeField] TMP_InputField resetPasswordEMailInput;

    // Change Password Inputs
    [SerializeField] TMP_InputField changePasswordCodeInput;
    [SerializeField] TMP_InputField changePasswordInput;


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
