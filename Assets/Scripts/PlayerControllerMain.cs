using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerMain : MonoBehaviourPunCallbacks
{
    public static PlayerControllerMain Instance;
    GameObject Player;

    PhotonView PV;

    void Awake()
    {
        Instance = this;
        Player = this.gameObject;

        PV = GetComponent<PhotonView>();
        
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            return;
        }
    }

    // Calls when a player leaves a room (for the player only) (Button Trigger)
    public void LeaveRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom(); // Notify the server that the player has left
        SceneManager.LoadScene("MPMenu");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }
}
