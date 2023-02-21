using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;

    void Awake()
    {
        // Allows this script to be called by other scripts
        Instance = this; 
    }

    // Calls when a menu needs to be opened by string of menuName
    public void OpenMenu(string menuName)
    {
        for(int i = 0; i < menus.Length; i++)
        {
            if(menus[i].menuName == menuName)
            {
                // If the Menu incremented to is the menu we need, open it
                menus[i].Open();
            }
            else if (menus[i].open)
            {
                // If the Menu incremented to is not the menu we need and it is open, close it
                CloseMenu(menus[i]);
            }
        }
    }

    // Calls when a menu needs to be opened by an index in list menu
    public void OpenMenu(Menu menu)
    {
        // Works similar to the above function, just using the index of list menu rather than a string
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    // Calls when a menu needs to be closed
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
