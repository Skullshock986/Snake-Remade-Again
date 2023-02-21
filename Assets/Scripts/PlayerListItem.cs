using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;

    // Calls when a player list item has been instantiated 
    public void SetUp(Player _player)
    {
        // Sets the text UI to the player's name
        player = _player;
        text.text = _player.NickName;
    }

    // Calls when a player has left a room (takes the player that left as an argument)
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // If the player that left 
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }
}
