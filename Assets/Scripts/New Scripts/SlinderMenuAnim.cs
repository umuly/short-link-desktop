using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlinderMenuAnim : MonoBehaviour
{
    public GameObject PanelMenu;

    public void ShowHideMenu()
    {
        if (PanelMenu != null)
        {
            Animator animator = PanelMenu.GetComponent<Animator>();
            if (animator != null)
            {
                bool isOpen = animator.GetBool("closeOpen");
                animator.SetBool("closeOpen", !isOpen);
            }
        }
    }
}