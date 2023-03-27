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

    public GameObject PickupEffect;

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
        // Check each walls GameObject in the list of walls to get the one that is active in the game scene
        foreach(GameObject wall in WallsList)
        {
            if (wall.activeSelf)
            {
                AreaWallsObj = wall;
            }
        }
        // Assign the gridArea of the walls to the first BoxCollider2D the code finds in the children of the walls GameObject
        gridArea = AreaWallsObj.GetComponentsInChildren<BoxCollider2D>()[0];

        // If the walls GameObject has more than 1 BoxCollider2D component in its children, assign innerArea to the second BoxCollider2D component
        if(AreaWallsObj.GetComponentsInChildren<BoxCollider2D>().Length  == 2)
        {
            innerArea = AreaWallsObj.GetComponentsInChildren<BoxCollider2D>()[1];
            HasInner = true; // Notify the code that the player has inner walls currently active
        }
        else
        {
            HasInner = false;
        }
    }

    public void RandomPos()
    {
        // Grab the bounds of the Grid Area and get random x and y values from these bounds
        Bounds bounds = gridArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        // Set the positon of the food to the x and y values obtained
        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);

        // Keep randomising the food's position until it is no longer inside the middle wall in HardWalls, should it be active
        while (HasInner == true && innerArea.bounds.Contains(this.transform.position)) {this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);}
    }

    // Calls when the food collider hits another box collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tell the player that they have collided with something
        Debug.Log("collided");

        // Only run this part if the food has collided with a snake head
        if (other.tag == "Snake")
        {
            Instantiate(PickupEffect, transform.position, transform.rotation);
            if (AreaWallsObj.activeSelf == false)
                SetWallsObj();
            RandomPos();
        }
    }
}
