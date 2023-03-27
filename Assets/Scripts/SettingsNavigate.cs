using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsNavigate : MonoBehaviour
{

    public List<GameObject> menus;
    public GameObject mainSettings;

    public void ActivateSettings()
    {
        // Open the main settings menu
        mainSettings.SetActive(true);
        menus[0].SetActive(true);
    }

    public void DeactivateSettings()
    {
        // Close the main settings menu
        mainSettings.SetActive(false);
    }

    public void SwitchSettingsMenus(int menuIndex)
    {
        // Close all sub-settings menus and open the menu corresponding to the provided index
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }

        menus[menuIndex].SetActive(true);
    }

    public void BackToMain()
    {
        // Close all sub-settings menus
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[0].SetActive(true);
    }
}
