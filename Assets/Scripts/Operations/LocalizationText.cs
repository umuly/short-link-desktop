using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizationText : MonoBehaviour
{
    public string key;
    void Start()
    {
        ChangeLanguage(index: 0); 
    }
    private void ChangeLanguage(int index) => gameObject.GetComponent<Text>().text = CVSParser.GetTextFromId(index, key);

    private void OnEnable()
    {
        LanguageDropdown.ChangeLanguage += ChangeLanguage;   
            }

    private void OnDisable()
    {
        LanguageDropdown.ChangeLanguage -= ChangeLanguage;
    }

}
