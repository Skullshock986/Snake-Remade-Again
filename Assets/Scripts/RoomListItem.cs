using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public RoomInfo info;

    // Calls when a room list item has been instantiated 
    public void SetUp(RoomInfo _info)
    {
        // Sets the text UI to the room's name
        info = _info;
        text.text = _info.Name;
    }

    // Calls when the button is clicked
    public void OnClick()
    {
        // Calls the JoinRoom function in the Launcher Script
        Launcher.Instance.JoinRoom(info);
    }
}
