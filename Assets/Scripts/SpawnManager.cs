using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int PlayerIndex;

    public static SpawnManager Instance;

    public SpawnPoint[] SpawnPoints;

    public Player[] RoomPlayers;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        RoomPlayers = PhotonNetwork.PlayerList; // Grabs a list of all players in the room
        Instance = this; // Makes this script a singleton
    }

    public Transform GetSpawnPoint(int i)
    {
        // Output (in the debug console) and return the transform of the corresponding spawnpoint to the player calling the function
        Debug.Log(SpawnPoints[i].transform.position + " By player " + RoomPlayers[i]);
        return SpawnPoints[i].transform;
    }
}
