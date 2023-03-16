using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerPVScript : MonoBehaviour
{
    PhotonView photonView;
    Transform ParentT;
    public static List<PhotonView> Instances = new List<PhotonView>();
    public static List<Transform> TransformInstances = new List<Transform>();
    public static Dictionary<int, Transform> TransformDictionary = new Dictionary<int, Transform>();
    public static PlayerPVScript LocalInstance;
    public List<PhotonView> InstancesCopy;

    void Awake()
    {
        photonView = gameObject.GetComponent<PhotonView>();

        ParentT = this.gameObject.transform.parent;

        Instances.Add(photonView);
        TransformInstances.Add(ParentT);

        TransformDictionary.Add(photonView.ViewID, ParentT);

        if (photonView.IsMine)
        {
            LocalInstance = this;
        }
    }

    void OnDestroy()
    {
        Instances.Remove(photonView);
        TransformDictionary.Remove(photonView.ViewID);
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

    public static List<PhotonView> GetPlayerViewList()
    {
        return Instances;
    }

    public static Dictionary<int, Transform> GetPlayerTransformDict()
    {
        return TransformDictionary;
    }
}
