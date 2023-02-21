using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Assignment : MonoBehaviour
{
    public GameObject AssignWalls()
    {
        GameObject[] Walls = GameObject.FindGameObjectsWithTag("ObstacleW");
        foreach (GameObject w in Walls)
        {
            Debug.Log(w.transform.position);
            PhotonView wPV = w.GetPhotonView();
            if (!wPV.IsMine)
            {
                Debug.Log("Walls are not mine");
                continue;
            }
            else if (wPV.IsMine)
            {
                Debug.Log("Walls are mine");
                return w;
            }
        }
        return null;
    }

    public GameObject AssignFood()
    {
        GameObject[] Food = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject f in Food)
        {
            Debug.Log(f.transform.position);
            PhotonView fPV = f.GetPhotonView();
            if (fPV.IsMine)
            {
                Debug.Log("Food is mine");
                return f;
            }
            else
            {
                Debug.Log("Food is not mine");
                continue;
            }
        }
        return null;
    }

    public void DestroySegments(List<Transform> _segments)
    {
        
    }
}
