using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerControllerMain : MonoBehaviour
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
}
