using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareOpener : MonoBehaviour
{
    public GameObject Panel;
    public void OpenPanel()
    {
        bool isActive = Panel.activeSelf;
        Panel.SetActive(!isActive);
    }
}
