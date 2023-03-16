using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    public FoodInfo foodInfo;

    [SerializeField] List<GameObject> WallsList;
    public GameObject AreaWallsObj;
    public BoxCollider2D gridArea;
    public BoxCollider2D innerArea;

    public bool HasInner;

    public PhotonView PV;

    void Awake()
    {
        SetWallsObj();
        PV = GetComponent<PhotonView>();
        RandomPos();
    }

    void Start()
    {
        
    }

    public void SetWallsObj()
    {
        foreach(GameObject wall in WallsList)
        {
            if (wall.activeSelf)
            {
                AreaWallsObj = wall;
            }
        }
        gridArea = AreaWallsObj.GetComponentsInChildren<BoxCollider2D>()[0];


        if(AreaWallsObj.GetComponentsInChildren<BoxCollider2D>().Length  == 2)
        {
            innerArea = AreaWallsObj.GetComponentsInChildren<BoxCollider2D>()[1];
            HasInner = true;
        }
        else
        {
            HasInner = false;
        }
    }

    public void RandomPos()
    {
        Bounds bounds = gridArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
        while (HasInner == true && innerArea.bounds.Contains(this.transform.position)) {this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collided");
        if (other.tag == "Snake")
        {
            if (AreaWallsObj.activeSelf == false)
                SetWallsObj();
            RandomPos();
        }
    }
}
