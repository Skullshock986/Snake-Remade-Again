using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    public FoodInfo foodInfo;

    public GameObject AreaWallsObj;
    public BoxCollider2D gridArea;

    public PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        gridArea = AreaWallsObj.GetComponentInChildren<BoxCollider2D>();
        RandomPos();
    }

    void Start()
    {
        
    }
    public void RandomPos()
    {
        Bounds bounds = gridArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collided");
        if (other.tag == "Snake")
        {
            RandomPos();
        }
    }
}
