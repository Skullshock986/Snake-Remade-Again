using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialise a list of players, an indicator of the local player, as well as an indicator of the index of the local player
        Player[] RoomPlayers = PhotonNetwork.PlayerList;
        Player LocalPlayer = PhotonNetwork.LocalPlayer;
        int PlayerIndex = 0;
        // Check what index in the list of players in a Room the local player is in
        for (int i = 0; i < RoomPlayers.Length; i++)
        {
            if (RoomPlayers[i] == LocalPlayer)
            {
                PlayerIndex = i;
            }
        }

        if (PV.IsMine) // If this is the local player:
        {
            // Get the transform of the PlayerCintroller's spawn via a separate script
            Transform SpawnPoint = SpawnManager.Instance.GetSpawnPoint(PlayerIndex);
            CreatePlayer(SpawnPoint); 
        }
    }

    void CreatePlayer(Transform s)
    {
        // Spawns the PlayerController at the transform position and rotation provided (Quaternion.identity = no rotation)
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerControllerMain"), s.position, Quaternion.identity);
    }

}
