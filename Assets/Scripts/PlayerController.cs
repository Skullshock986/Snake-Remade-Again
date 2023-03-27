using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] PlayerPVScript PVListScript;
    [SerializeField] PlayerItems ItemsScript;
    public List<PhotonView> PVInstances;
    public Dictionary<int, Transform> TransformDict;

    [SerializeField] GameObject food;
    [SerializeField] FoodScript foodScript;

    [SerializeField] GameObject Main;
    [SerializeField] PlayerControllerMain mainScript;

    [SerializeField] bool IsLost;
    [SerializeField] bool IsWon;

    [SerializeField] int TargettedBy;

    [SerializeField] int PlayerCount;

    public int PlayerID;
    [SerializeField] int MyID;
    [SerializeField] int tempValue;
    [SerializeField] Vector3 TargetPosition;
    [SerializeField] int sentInt;
    [SerializeField] bool IsChecked;

    [SerializeField] Vector3 PlayerPosition;
    [SerializeField] GameObject BorderPrefab;
    [SerializeField] GameObject ControllerMain;

    public int checkID;

    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text KOText;
    [SerializeField] TMP_Text KillsText;
    [SerializeField] TMP_Text PlaceText;
    [SerializeField] TMP_Text PlayerNumText;
    [SerializeField] GameObject uiCanvas;
    [SerializeField] GameObject indicator;
    [SerializeField] GameObject KOIndicator;
    [SerializeField] GameObject WinIndicator;
    [SerializeField] GameObject KOImage;
    [SerializeField] GameObject WinImage;

    [SerializeField] GameObject[] UIParts;

    [SerializeField] List<Image> TargetBGs;
    private int BGListPointer;

    [SerializeField] Image PU1;
    [SerializeField] Image PU2;
    [SerializeField] Image PU3;

    [SerializeField] Vector2 _direction;
    private List<Transform> _segments;
    [SerializeField] string DirectionIndicator;
    public float repeat_time = 0.1f;
    public bool Waiting;
    public bool IsSpeedable;
    public int FoodCount;
    public int IncreaseAmt;

    public int Kills;
    public int Place;

    [SerializeField] bool IsPP1Active;
    [SerializeField] bool IsPP2Active;
    [SerializeField] bool IsPP3Active;

    [SerializeField] GameObject AreaWallsObj;

    public List<GameObject> WallsList;

    PhotonView PV;

    #region UNITY

    void Awake()
    {
        instance = this;
        PV = GetComponent<PhotonView>();
        foodScript = food.GetComponent<FoodScript>();
        mainScript = Main.GetComponent<PlayerControllerMain>();
        PlayerPosition = ControllerMain.transform.position;
    }

    void Start()
    {
        // If this isn't the local player, dont run anything and destroy the player's UI canvas
        if (!PV.IsMine)
        {
            Destroy(uiCanvas);
            Destroy(indicator);
            return;
        }

        IsLost = false;
        Kills = 0;
        Place = PhotonNetwork.CurrentRoom.PlayerCount; // TALK BOUT

        PlayerID = 0;
        MyID = PV.ViewID;

        // Reset the direction of the player
        _direction = Vector2.right;
        DirectionIndicator = "None";

        // Make a new list of Transforms and add the position of the player to it
        _segments = new List<Transform>();
        _segments.Add(this.transform);

        Waiting = false;
        IsSpeedable = false;
        FoodCount = 0;

        for(int i = 0; i < 4; i++)
        {
            ColourReset(i);
        }



        IncreaseAmt = 1;

        PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        BGListPointer = -1;
        TargetIncrement();

        // Start the named coroutines
        StartCoroutine("Movement");
        StartCoroutine("Direction");
        StartCoroutine("TargetChange");
        StartCoroutine("PlayerTarget");
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine || IsLost)
        {
            // Do not do anything if the player is not the local player and has lost
            return;
        }

        // If U has been pressed and the player is eligible for a power-up from ItemStack 1, "purchase" a power-up from that stack
        if (Input.GetKeyDown(KeyCode.U) && ItemsScript.heldPU1 == -1 && IsPP1Active)
        {
            ItemsScript.StartPickup(1);
            FoodCount -= 10;
            LengthChange(-10);
            PUCheck();
            ScoreText.text = FoodCount.ToString("00");
        }
        else if (Input.GetKeyDown(KeyCode.U) && ItemsScript.heldPU1 != -1)
        {
            ItemsScript.ActivatePU1();
            ColourReset(1);
        }

        // If I has been pressed and the player is eligible for a power-up from ItemStack 2, "purchase" a power-up from that stack
        if (Input.GetKeyDown(KeyCode.I) && ItemsScript.heldPU2 == -1 && IsPP2Active)
        {
            ItemsScript.StartPickup(2);
            FoodCount -= 15;
            LengthChange(-15);
            PUCheck();
            ScoreText.text = FoodCount.ToString("00");
        }
        else if (Input.GetKeyDown(KeyCode.I) && ItemsScript.heldPU2 != -1)
        {
            ItemsScript.ActivatePU2();
            ColourReset(2);
        }

        // If O has been pressed and the player is eligible for a power-up from ItemStack 3, "purchase" a power-up from that stack
        if (Input.GetKeyDown(KeyCode.O) && ItemsScript.heldPU3 == -1 && IsPP3Active)
        {
            ItemsScript.StartPickup(3);
            FoodCount -= 20;
            LengthChange(-20);
            PUCheck();
            ScoreText.text = FoodCount.ToString("00");
        }
        else if (Input.GetKeyDown(KeyCode.O) && ItemsScript.heldPU3 != -1)
        {
            ItemsScript.ActivatePU3();
            ColourReset(3);
        }

    }

    #endregion

    #region PUNRPC

    [PunRPC]
    private void RPC_Grow()
    {
        // Do not run this function if the player running this is not the local player
        if (!PV.IsMine)
            return;

        // Wait for 0.02 seconds and then instantiate a snake segment to the position of the last item in the _segments list
        StartCoroutine("Wait", 0.02f);
        GameObject SnakeS = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SnakeSegment"), _segments[_segments.Count - 1].position, Quaternion.identity);

        // Add this segment to the list of segments
        Transform segment = SnakeS.transform;
        _segments.Add(segment);

        // Increment the FoodCount by a set amount and allow the player to change speed
        FoodCount += IncreaseAmt;
        ScoreText.text = FoodCount.ToString("00"); // Display the new score of the player to its UI element
        IsSpeedable = true;
    }

    [PunRPC]
    private void RPC_Die()
    {
        // Do not destroy the player's own snake if they are not the player who died
        if (!PV.IsMine)
            return;

        // Go through every transform in _segments
        for (int i = 1; i < _segments.Count; i++)
        {
            // Grab the Photon View of the segment
            GameObject s = _segments[1].gameObject;
            PhotonView sPV = s.GetPhotonView();

            // If the segment is from the player who died, destroy the segment
            if (sPV.IsMine)
            {
                _segments.Remove(s.transform);
                PhotonNetwork.Destroy(s);
            }
            else
            {
                continue;
            }
        }

        
        /*FoodCount = 0;
        ScoreText.text = FoodCount.ToString("00");
        RandomPosSnake();
        repeat_time = 0.1f;*/
        
    }

    [PunRPC]
    private void RPC_CallToPlayer(int CallID, int ReturnID, int _c)
    {
        checkID = PV.ViewID;
        if (this.PV.IsMine)
        {
            CallToPlayer(CallID, ReturnID, _c);
        }
        else
        {
            return;
        }
    }

    [PunRPC]
    private void RPC_ReturnToPlayer(int pvID, int _c)
    {
        if (this.PV.IsMine)
        {
            ReturnToPlayer(_c);
        }
        else
        {
            return;
        }
    }


    [PunRPC]
    private void RPC_Boost(float time, float mult, int ID)
    {
        if (this.PV.IsMine)
        {
            StartCoroutine(Boost(time, mult));
            TargettedBy = ID;
        }
        else
        {
            return;
        }
    }

    [PunRPC]
    private void RPC_WallsChange(float time, float type, int ID)
    {
        if (this.PV.IsMine)
        {
            StartCoroutine(WallsChange(time, type));
            TargettedBy = ID;
        }
        else
        {
            return;
        }
    }

    [PunRPC]
    private void RPC_ScoreChange(float Value, int ID)
    {
        if (this.PV.IsMine) // Run only on the player that this RPC is called to
        {
            // Run the player's local ScoreChange function, passing the integer passed into the RPC as an argument
            ScoreChange((int)Value);
            TargettedBy = ID;
        }
        else
        {
            return;
        }
    }

    [PunRPC]
    private void RPC_LengthChange(float Value, int ID)
    {
        if (this.PV.IsMine)
        {
            LengthChange((int)Value);
            TargettedBy = ID;
        }
        else
        {
            return;
        }
    }

    [PunRPC]
    private void RPC_PlayerDied(int ID)
    {
        // Call the PlayerDied function if the player is the local player and has not lost yet
        if (this.IsLost) { return; }
        else if (this.PV.IsMine)
        {
            PlayerDied(ID);
        }
        else
        {
            return;
        }

    }

    [PunRPC]
    private void RPC_ShowKO()
    {
        // Set the GameObject showing the K.O interface to be active
        KOIndicator.SetActive(true);
    }

    [PunRPC]
    private void RPC_ShowWin()
    {
        // Set the GameObject showing the K.O interface to be active
        WinIndicator.SetActive(true);
    }

    #endregion

    #region FUNCTIONS

    private void ColourReset(int Check)
    {
        switch (Check)
        {
            // Based on what power-up indicator square is set to reset, dim the alpha of that square to 0.3 and set that relative IsPPActive boolean to false
            case 1:
                var tempColour = PU1.color;
                tempColour.a = 0.3f;
                PU1.color = tempColour;
                IsPP1Active = false;
                break;
            case 2:
                tempColour = PU2.color;
                tempColour.a = 0.3f;
                PU2.color = tempColour;
                IsPP2Active = false;
                break;
            case 3:
                tempColour = PU3.color;
                tempColour.a = 0.3f;
                PU3.color = tempColour;
                IsPP3Active = false;
                break;
        }
    }

    private void Spectate()
    {
        SetUIActive(2);
    }

    private void Die()
    {
        // Set the bool checking if the player has lost to true
        this.IsLost = true;

        // Notify all players that this player has died
        PVInstances = PlayerPVScript.GetPlayerViewList();
        foreach (PhotonView pv in PVInstances)
        {
            if (pv.ViewID != PV.ViewID)
            {
                Player player = pv.Controller;
                pv.RPC("RPC_PlayerDied", player, TargettedBy);
            }
        }

        // Activate the Game Over UI and the K.O text with that UI
        SetUIActive(1);
        KOImage.SetActive(true);

        // Display the number of kills a player got as well as their place in the room
        KillsText.text = Kills.ToString("00");
        PlaceText.text = Place.ToString("00");
        PlayerNumText.text = PlayerCount.ToString("00");
    }

    private void Win()
    {
        // Stop all Coroutines currently active
        StopAllCoroutines();

        // Activate the Game Over UI and the Win text with that UI
        SetUIActive(1);
        WinImage.SetActive(true);

        // Call the RPC to show the Win interface to all players in the room
        PV.RPC("RPC_ShowWin", RpcTarget.All);

        // Display the number of kills a player got as well as their place in the room
        KillsText.text = Kills.ToString("00");
        PlaceText.text = Place.ToString("00");
        PlayerNumText.text = PlayerCount.ToString("00");
    }

    public void SetUIActive(int index) 
    {
        // Set every UI GameObject in the UIParts list to inactive and only activate the UI gameobject of the specified index in UIParts
        foreach (GameObject ui in UIParts)
        {
            ui.SetActive(false);
        }
        UIParts[index].SetActive(true);
    }

    // Calls when the Snake Head collider hits another box collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Do not do anything if the Snake head is not from the local player
        if (!PV.IsMine)
        {
            return;
        }

        // If the snake collides with a food GameObject:
        if (other.tag == "Food")
        {
            StopCoroutine("Movement");
            PV.RPC("RPC_Grow", RpcTarget.All);
            StartCoroutine("Movement");
            PUCheck(); // Check if a power-up is able to be collected
        }
        // Otherwise if the snake collides with either the walls or itself:
        else if ((other.tag == "Walls" || other.tag == "ObstacleS"))
        {
            while (!IsLost) // While the Player has not lost yet:
            {
                PV.RPC("RPC_ShowKO", RpcTarget.All); // Call the RPC to show the K.O interface to all players in the room
                StopAllCoroutines(); // Stop all Coroutines currently active
                Die(); // Run the Die function (for the local player only)
                PV.RPC("RPC_Die", RpcTarget.All); // Run the Die RPC (for all players to see)
            }

            /*StopCoroutine("Movement");
            PV.RPC("RPC_Die", RpcTarget.All); // Run the Die RPC (for all players to see)
            StartCoroutine("Movement");*/

            for (int i = 1; i < 4; i++)
            {
                ColourReset(i); // Reset all power-up indicators
            }
        }
        else if (other.tag == "NoWalls")
        {
            Debug.Log(other.gameObject.name.ToString());
            StopCoroutine("Movement");

            switch (other.gameObject.name)
            {
                case "DWall":
                    this.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y + 27.0f,
                    0.0f
                    );
                    break;
                case "LWall":
                    this.transform.position = new Vector3(
                    this.transform.position.x + 43.0f,
                    this.transform.position.y,
                    0.0f
                    );
                    break;
                case "UWall":
                    this.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y - 27.0f,
                    0.0f
                    );
                    break;
                case "RWall":
                    this.transform.position = new Vector3(
                    this.transform.position.x - 43.0f,
                    this.transform.position.y,
                    0.0f
                    );
                    break;
            }
            StartCoroutine("Movement");
        }
    }

    private void RandomPosSnake()
    {
        if (!PV.IsMine)
        {
            return;
        }

        Bounds bounds = foodScript.gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
        while (foodScript.HasInner == true && foodScript.innerArea.bounds.Contains(this.transform.position)) { this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f); }
    }

    private void PUCheck()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (FoodCount >= 10 && IsPP1Active == false) // If the player's score is above 9 and the player doesnt already have a power-up in slot 1:
        {
            // Get PU1 to light up and set IsPP1Active to true
            StartCoroutine("PUActive", PU1);
            IsPP1Active = true;
        }
        else if(FoodCount < 10)
        {
            // If the score is less than 10, dim PU1
            ColourReset(1);
        }
        if (FoodCount >= 15 && IsPP2Active == false) // If the player's score is above 14 and the player doesnt already have a power-up in slot 2:
        {
            // Get PU2 to light up and set IsPP2Active to true
            StartCoroutine("PUActive", PU2);
            IsPP2Active = true;
        }
        else if (FoodCount < 15)
        {
            // If the score is less than 15, dim PU2
            ColourReset(2);
        }
        if (FoodCount >= 20 && IsPP3Active == false) // If the player's score is above 19 and the player doesnt already have a power-up in slot 3:
        {
            // Get PU3 to light up and set IsPP3Active to true
            StartCoroutine("PUActive", PU3);
            IsPP3Active = true;
        }
        else if (FoodCount < 20)
        {
            // If the score is less than 20, dim PU3
            ColourReset(3);
        }
    }

    // Calls when target needs to be changed (on user input)
    private void TargetIncrement()
    {
        // Increment BGListPointer, lopping it back to 0 when it goes over 3
        BGListPointer = (BGListPointer + 1) % 4;

        // Reset the alpha of all targetting UI element images to dim them slightly
        foreach (Image i in TargetBGs)
        {
            var tempColour = i.color;
            tempColour.a = 0.3f;
            i.color = tempColour;
        }

        // Set the alpha of the targetting UI element in the index of BGListPointer to 1, highlighting it
        Image tempImage = TargetBGs[BGListPointer];
        var tempColor = tempImage.color;
        tempColor.a = 1f;
        tempImage.color = tempColor;
    }

    private void CallToPlayer(int CallID, int ReturnID, int _c)
    {
        if (_c > 0)
        {
            // Set the ID of the player to return a value to as the ReturnID passed to it and set sentInt to 0
            int rID = ReturnID;
            sentInt = 0;

                if (_c == 3)
                {
                    // If the integer passed through was 3, send back to the player who sent the initial RPC call -10, only if this player is targetting that player
                    this.PVInstances = PlayerPVScript.GetPlayerViewList();
                    this.sentInt = (this.PVInstances[PlayerID].ViewID == CallID) ? -10 : 0;
                }
                else if (_c == 2)
                {
                    // If the integer passed through was 2, send back to the player who sent the initial RPC call the score this player has
                    this.sentInt = this.FoodCount;
                }
                else if (_c == 1)
                {
                    // If the integer passed through was 1, send back to the player who sent the initial RPC call the number of kills this player has
                    Debug.LogError("sentInt = " + Kills);
                    this.sentInt = this.Kills;
                }
        }

        // Return whatever value was sent to the player who sent the inital RPC call by finding that player's ViewID
        PVInstances = PlayerPVScript.GetPlayerViewList();
        foreach (PhotonView _pv in PVInstances)
        {
            if (_pv.ViewID == ReturnID)
            {
                Player player = _pv.Controller;
                _pv.RPC("RPC_ReturnToPlayer", player, ReturnID, sentInt);
            }
        }
    }

    private void ReturnToPlayer(int _c)
    {
        this.tempValue = _c;
        this.IsChecked = true; // Set the local player's IsChecked to true
    }

    public void ScoreChange(int value)
    {
        // Add the integer passed through to the player's FoodCount
        FoodCount += value;

        // Update the player's Score UI to reflect these changes
        ScoreText.text = FoodCount.ToString("00");

        // Check if the player is still able to "purchase" power-ups or not
        PUCheck();
    }

    public void LengthChange(int value)
    {
        switch (value)
        {
            case int n when (n < 0): // When the player needs to lose length:
                for (int i = 0; i < Mathf.Abs(value); i++)
                {
                    // Destroy the last segment of the player "value" number of times in the network
                    int count = _segments.Count;
                    GameObject s = _segments[count - 1].gameObject;
                    PhotonView sPV = s.GetPhotonView();
                    if (sPV.IsMine)
                    {
                        _segments.Remove(s.transform);
                        PhotonNetwork.Destroy(s);
                    }
                    else
                    {
                        continue;
                    }
                }
                // Set the new player speed to the speed of the player with their new length
                repeat_time = 0.1f / Mathf.Pow(1.25f, (FoodCount / 5));
                break;
            case int n when (n > 0):
                for (int i = 0; i < value; i++)
                {
                    // Instantiate the SnakeSegment for the player "value" number of times (mimicking the method in RPC_Grow)
                    StartCoroutine("Wait", 0.02f);
                    GameObject SnakeS = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SnakeSegment"), _segments[_segments.Count - 1].position, Quaternion.identity);
                    Transform segment = SnakeS.transform;
                    segment.position = _segments[_segments.Count - 1].position;
                    _segments.Add(segment);
                }
                // Set the new player speed to the speed of the player with their new length
                repeat_time = 0.1f / Mathf.Pow(1.25f, (FoodCount / 5));
                break;
        }
    }

    private void PlayerDied(int ID)
    {
        // Reduce this player's current placement in the room 
        Place -= 1;

        // If this player killed the player who died, add one to their killcount
        if (ID == PV.ViewID)
        {
            Kills += 1;
            KOText.text = Kills.ToString("00");
        }

        // If this player is the one player left alive, call the Win function
        if(Place == 1)
        {
            Win();
        }
    }

    #endregion

    #region COROUTINES
    IEnumerator Direction()
    {
        // Only run this for the local player
        while (PV.IsMine)
        {
            // Change direction of the snake according to player input, only if the direction inputted is not the opposite of the current direction
            if (Input.GetKeyDown(KeyCode.W) && DirectionIndicator != "Down")
            {
                _direction = Vector2.up;
                DirectionIndicator = "Up";
                yield return new WaitForSeconds(repeat_time / 1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.S) && DirectionIndicator != "Up")
            {
                _direction = Vector2.down;
                DirectionIndicator = "Down";
                yield return new WaitForSeconds(repeat_time / 1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.A) && DirectionIndicator != "Right")
            {
                _direction = Vector2.left;
                DirectionIndicator = "Left";
                yield return new WaitForSeconds(repeat_time / 1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.D) && DirectionIndicator != "Left")
            {
                _direction = Vector2.right;
                DirectionIndicator = "Right";
                yield return new WaitForSeconds(repeat_time / 1.5f);
            }
            // If nothing is inputted, wait until the next frame and try again
            yield return 0;
        }
    }

    IEnumerator TargetChange()
    {
        // Only do this for the local player
        while (PV.IsMine)
        {
            // If P is pressed, call TargetIncrement
            if (Input.GetKeyDown(KeyCode.P))
            {
                TargetIncrement();
            }
            // If not, wait until the next frame to check again
            yield return 0;
        }
    }

    IEnumerator PlayerTarget()
    {
        while (PV.IsMine)
        {
            // Grab the list of Photon Views from PlayerPVScript
            PVInstances = PlayerPVScript.GetPlayerViewList();

            if (PVInstances.Count > 1)
            {
                switch (BGListPointer)
                {
                    case 0: //Random Player
                        // Choose a random Photon View ID from the Instances list, as long as it is not the local player
                        PlayerID = PVInstances[Random.Range(0, PlayerPVScript.Instances.Count)].ViewID;
                        while (PlayerID == PV.ViewID)
                        {
                            PlayerID = PVInstances[Random.Range(0, PlayerPVScript.Instances.Count)].ViewID;
                        }
                        break;
                    case 1: //Max Kills
                        // Set a MaxValue indicator to 0
                        int MaxValue = 0;
                        foreach (PhotonView pv in PVInstances)
                        {
                                // Iterate through every Player using their ViewID, calling RPC_CallToPlayer
                                Player player = pv.Controller;
                                IsChecked = false; // Set IsChecked to false
                                pv.RPC("RPC_CallToPlayer", player, pv.ViewID, MyID, 1);
                                yield return new WaitUntil(() => IsChecked); // Only continue with the code when IsChecked is true
                                if (this.tempValue > MaxValue)
                                {
                                    // If the value returned is larger than the current max value, make that value the max value and set PlayerID to that player's ViewID
                                    MaxValue = tempValue;
                                    PlayerID = pv.ViewID;
                                }
                        }
                        break;
                    case 2: //Max Score 
                        // Set a MaxValue indicator to 0
                        int MaxValue2 = 0;
                        foreach (PhotonView pv in PVInstances)
                        {
                                // Iterate through every Player using their ViewID, calling RPC_CallToPlayer
                                Player player = pv.Controller;
                                IsChecked = false; // Set IsChecked to false
                                pv.RPC("RPC_CallToPlayer", player, pv.ViewID, MyID, 2);
                                yield return new WaitUntil(() => IsChecked); // Only continue with the code when IsChecked is true
                                if (this.tempValue > MaxValue2)
                                {
                                    // If the value returned is larger than the current max value, make that value the max value and set PlayerID to that player's ViewID
                                    MaxValue2 = this.tempValue;
                                    PlayerID = pv.ViewID;
                                }
                        }
                        break;
                    case 3: //Attacker
                        foreach (PhotonView pv in PVInstances)
                        {
                                // Iterate through every Player using their ViewID, calling RPC_CallToPlayer
                                Player player = pv.Controller;
                                IsChecked = false; // Set IsChecked to false
                                pv.RPC("RPC_CallToPlayer", player, pv.ViewID, MyID, 3);
                                yield return new WaitUntil(() => IsChecked); // Only continue with the code when IsChecked is true
                                if (this.tempValue == -10)
                                {
                                    // If the value returned is -10, set PlayerID to that player's ViewID
                                    PlayerID = pv.ViewID;
                                }
                        }
                        break;
                }

                TransformDict = PlayerPVScript.GetPlayerTransformDict();
                /*for(int i = 0; i < PVInstances.Count; i++)
                {
                    Debug.LogError("Player " + PVInstances[i].ViewID + " At transform " +  TransformDict[PVInstances[i].ViewID].position);
                }*/
                TargetPosition = TransformDict[PlayerID].position;

                // If the BorderPrefab is not already active, activate it
                if (!BorderPrefab.activeInHierarchy) { BorderPrefab.SetActive(true); }

                // Set the position of the BorderPrefab to the Target's position
                BorderPrefab.transform.position = new Vector3(
                    TargetPosition.x,
                    TargetPosition.y,
                    1.0f
                );
            }
            else
            {
                BorderPrefab.SetActive(false);
            }
            yield return new WaitForSeconds(1.0f);
        }

    }

    IEnumerator Movement()
    {
        while (PV.IsMine && (!IsLost || !IsWon))
        {
            for (int i = _segments.Count - 1; i > 0; i--)
            {
                _segments[i].position = _segments[i - 1].position;
            }
            this.transform.position = new Vector3(
                Mathf.Round(this.transform.position.x) + _direction.x,
                Mathf.Round(this.transform.position.y) + _direction.y,
                0.0f
            );

            // If the player's snake is speedable and its score is a multiple of 5 greater than 0 run the following code:
            if (FoodCount > 0 && FoodCount % 5 == 0 && IsSpeedable)
            {
                // Reduce the amount of waiting time between moving the snake one unit by a factor of 1.25
                repeat_time = repeat_time / 1.25f;
                Debug.Log("Speed Up");
                IsSpeedable = false; // Ensure that the snake doesnt infinitely speed up 
            }
            yield return new WaitForSeconds(repeat_time);
        }
    }

    // Allows any function to wait for a specified time before continuing
    IEnumerator Wait(float time)
    {
        Waiting = true;
        yield return new WaitForSecondsRealtime(time);
        Waiting = false;
    }

    IEnumerator PUActive(Image image)
    {
        // Brighten the corresponding power-up indicator square over a short period of time
        for (float f = 0.3f; f <= 1; f += 0.1f)
        {
            var tempColour = image.color;
            tempColour.a = f;
            image.color = tempColour;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public IEnumerator WallsChange(float time, float type)
    {
        // Set the active walls to be the ones at the provided index of the walls list
        WallsList[0].SetActive(false);
        WallsList[(int)type].SetActive(true);
        // Allow the food GameObject to recognise its new surroundings
        foodScript.SetWallsObj();
        foodScript.RandomPos();

        // Wait a set period of time before reverting the active walls to the default AreaWalls and allowing the food to recognise its surroundings again
        yield return new WaitForSeconds(time);

        WallsList[(int)type].SetActive(false);
        WallsList[0].SetActive(true);
        foodScript.SetWallsObj();
        foodScript.RandomPos();
    }

    public IEnumerator Boost(float time, float mult)
    {
        // Divide the current repeat_time by the multiplier passed into the function and wait 10 seconds before reverting repeat_time to normal
        repeat_time = (0.1f / Mathf.Pow(1.25f, (FoodCount / 5))) / mult;
        yield return new WaitForSeconds(time);
        repeat_time = 0.1f / Mathf.Pow(1.25f, (FoodCount / 5));
    }

    #endregion
}