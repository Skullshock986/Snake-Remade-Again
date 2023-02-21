using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;

    // Calls when a Menu needs to be opened
    public void Open()
    {   
        // Activate the Menu's parent gameObject
        gameObject.SetActive(true);
        open = true;
    }

    // Calls when a Menu needs to be Closed
    public void Close()
    {
        // Deactivate the Menu's parent gameObject
        gameObject.SetActive(false);
        open = false;
    }
}
