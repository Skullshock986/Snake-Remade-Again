using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerPVScript : MonoBehaviour
{
    PhotonView photonView;
    public static List<PhotonView> Instances = new List<PhotonView>();
    public static PlayerPVScript LocalInstance;

    void Awake()
    {
        photonView = gameObject.GetComponent<PhotonView>();

        Instances.Add(photonView);

        if (photonView.IsMine)
        {
            LocalInstance = this;
        }
    }

    void OnDestroy()
    {
        Instances.Remove(photonView);
    }

    public PhotonView GetPhotonView()
    {
        return photonView;
    }

    public static PhotonView GetPlayerView(Player owner)
    {
        for (int i = 0; i < Instances.Count; i++)
        {
            if (Instances[i].Owner == owner)
            {
                return Instances[i];
            }
        }

        return null;
    }

    public static PhotonView GetPlayerView(int photonViewIndex)
    {
        for (int i = 0; i < Instances.Count; i++)
        {
            if (Instances[i].ViewID == photonViewIndex)
            {
                return Instances[i];
            }
        }

        return null;
    }
}
