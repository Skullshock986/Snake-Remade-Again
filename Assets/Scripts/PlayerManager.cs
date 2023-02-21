using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    Player[] RoomPlayers;
    Player LocalPlayer;
    int PlayerIndex;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        RoomPlayers = PhotonNetwork.PlayerList;
        LocalPlayer = PhotonNetwork.LocalPlayer;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerIndex = 0;
        for (int i = 0; i < RoomPlayers.Length; i++)
        {
            if (RoomPlayers[i] == LocalPlayer)
            {
                PlayerIndex = i;
            }
        }

        if (PV.IsMine)
        {
            Transform SpawnPoint = SpawnManager.Instance.GetSpawnPoint(PlayerIndex);
            CreatePlayer(SpawnPoint);
        }
    }

    void CreatePlayer(Transform s)
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerControllerMain"), s.position, Quaternion.identity);
    }

}
