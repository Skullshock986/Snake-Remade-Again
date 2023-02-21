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
        RoomPlayers = PhotonNetwork.PlayerList;
        Instance = this;
        Debug.Log(RoomPlayers);
    }

    public void IncrementSpawn()
    {
        PV.RPC("RPC_Increment", RpcTarget.All);
    }

    public Transform GetSpawnPoint(int i)
    {
        Debug.Log(SpawnPoints[i].transform.position + " By player " + RoomPlayers[i]);
        return SpawnPoints[i].transform;
    }

    [PunRPC]
    void RPC_Increment()
    {
        PlayerIndex += 1;
        Debug.Log("Incremented to " + PlayerIndex);
    }
}
