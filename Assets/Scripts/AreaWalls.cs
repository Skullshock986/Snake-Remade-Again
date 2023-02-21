using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AreaWalls : MonoBehaviour
{
    public static AreaWalls _Instance;

    PhotonView PV;
    private void Awake()
    {
        _Instance = this;
        PV = GetComponent<PhotonView>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
