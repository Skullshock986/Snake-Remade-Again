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
    [SerializeField] TMP_Text KillsText;
    [SerializeField] TMP_Text PlaceText;
    [SerializeField] TMP_Text PlayerNumText;
    [SerializeField] GameObject uiCanvas;
    [SerializeField] GameObject KOIndicator;
    [SerializeField] GameObject KOImage;
    [SerializeField] GameObject WinImage;

    [SerializeField] GameObject[] UIParts;

    [SerializeField] List<Image> TargetBGs;
    private int BGListPointer;

    [SerializeField] Image PU1;
    [SerializeField] Image PU2;
    [SerializeField] Image PU3;

    [SerializeField] Vector2 _direction = Vector2.right;
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
        if (!PV.IsMine)
        {
            Destroy(uiCanvas);
            return;
        }

        IsLost = false;
        Kills = 0;
        Place = PhotonNetwork.CurrentRoom.PlayerCount;

        PlayerID = 0;
        MyID = PV.ViewID;

        DirectionIndicator = "None";

        _segments = new List<Transform>();
        _segments.Add(this.transform);

        Waiting = false;
        IsSpeedable = false;
        FoodCount = 0;

        for(int i = 0; i < 4; i++)
        {
            ColourReset(i);
        }

        BGListPointer = -1;

        IncreaseAmt = 1;
        StartCoroutine("Movement");
        StartCoroutine("Direction");

        TargetIncrement();
        StartCoroutine("TargetChange");
        StartCoroutine("PlayerTarget");
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine || IsLost)
        {
            return;
        }

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

        if (Input.GetKeyDown(KeyCode.O) && ItemsScript.heldPU3 == -1 && IsPP3Active)
        {
            ItemsScript.StartPickup(3);
            LengthChange(-20);
            FoodCount -= 20;
            PUCheck();
            ScoreText.text = FoodCount.ToString("00");
        }
        else if (Input.GetKeyDown(KeyCode.O) && ItemsScript.heldPU3 != -1)
        {
            ItemsScript.ActivatePU2();
            ColourReset(3);
        }

    }

    #endregion

    #region PUNRPC

    [PunRPC]
    private void RPC_Grow()
    {
        if (!PV.IsMine)
            return;

        StartCoroutine("Wait", 0.02f);
        GameObject SnakeS = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SnakeSegment"), _segments[_segments.Count - 1].position, Quaternion.identity);
        Transform segment = SnakeS.transform;
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);
        FoodCount += IncreaseAmt;
        ScoreText.text = FoodCount.ToString("00");
        IsSpeedable = true;
    }

    [PunRPC]
    private void RPC_Die()
    {
        if (!PV.IsMine)
            return;

        for (int i = 1; i < _segments.Count; i++)
        {
            GameObject s = _segments[1].gameObject;
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

        /*
        int count = _segments.Count;
        for (int i = 1; i < count; i++)
        {
            GameObject s = _segments[1].gameObject;
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
        FoodCount = 0;
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
        if (this.PV.IsMine)
        {
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
        KOIndicator.SetActive(true);
    }

    #endregion

    #region FUNCTIONS

    private void ColourReset(int Check)
    {
        switch (Check)
        {
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
        this.IsLost = true;
        PVInstances = PlayerPVScript.GetPlayerViewList();
        foreach (PhotonView pv in PVInstances)
        {
            if (pv.ViewID != PV.ViewID)
            {
                Player player = pv.Controller;
                pv.RPC("RPC_PlayerDied", player, TargettedBy);
            }
        }

        SetUIActive(1);
        KOImage.SetActive(true);
        KillsText.text = Kills.ToString("00");
        PlaceText.text = Place.ToString("00");
        PlayerNumText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString("00");
    }

    private void Win()
    {
        StopAllCoroutines();
        //PV.RPC("RPC_ShowWin", RpcTarget.All);
        this.IsWon = true;
        SetUIActive(1);
        WinImage.SetActive(true);
        KillsText.text = Kills.ToString("00");
        PlaceText.text = Place.ToString("00");
        PlayerNumText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString("00");
    }

    public void SetUIActive(int index) 
    {
        foreach (GameObject ui in UIParts)
        {
            ui.SetActive(false);
        }
        UIParts[index].SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (other.tag == "Food")
        {
            StopCoroutine("Movement");
            PV.RPC("RPC_Grow", RpcTarget.All);
            StartCoroutine("Movement");
            PUCheck();
        }
        else if ((other.tag == "Walls" || other.tag == "ObstacleS") && Waiting == false)
        {
            while (!IsLost)
            {
                PV.RPC("RPC_ShowKO", RpcTarget.All);
                StopAllCoroutines();
                Die();
                PV.RPC("RPC_Die", RpcTarget.All);
            }

            for (int i = 1; i < 4; i++)
            {
                ColourReset(i);
            }
        }
        else if (other.tag == "NoWalls" && Waiting == false)
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

        if (FoodCount >= 10 && IsPP1Active == false)
        {
            StartCoroutine("PUActive", PU1);
            IsPP1Active = true;
        }
        else if(FoodCount < 10)
        {
            ColourReset(1);
        }
        if (FoodCount >= 15 && IsPP2Active == false)
        {
            StartCoroutine("PUActive", PU2);
            IsPP2Active = true;
        }
        else if (FoodCount < 15)
        {
            ColourReset(2);
        }
        if (FoodCount >= 20 && IsPP3Active == false)
        {
            StartCoroutine("PUActive", PU3);
            IsPP3Active = true;
        }
        else if (FoodCount < 20)
        {
            ColourReset(3);
        }
    }

    private void TargetIncrement()
    {
        BGListPointer = (BGListPointer + 1) % 4;
        foreach (Image i in TargetBGs)
        {
            var tempColour = i.color;
            tempColour.a = 0.3f;
            i.color = tempColour;
        }
        Image tempImage = TargetBGs[BGListPointer];
        var tempColor = tempImage.color;
        tempColor.a = 1f;
        tempImage.color = tempColor;
    }

    private void CallToPlayer(int CallID, int ReturnID, int _c)
    {
        if (_c > 0)
        {
            int rID = ReturnID;
            sentInt = 0;
            if (!this.IsLost)
            {
                if (_c == 3)
                {
                    this.PVInstances = PlayerPVScript.GetPlayerViewList();
                    this.sentInt = (this.PVInstances[PlayerID].ViewID == CallID) ? -10 : 0;
                }
                else if (_c == 2)
                {
                    this.sentInt = this.FoodCount;
                }
                else if (_c == 1)
                {
                    Debug.LogError("sentInt = " + Kills);
                    this.sentInt = this.Kills;
                }
            }
            else
            {
                sentInt = -1;
            }
        }
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
        this.IsChecked = true;
    }

    public void ScoreChange(int value)
    {
        FoodCount -= value;
        ScoreText.text = FoodCount.ToString("00");
        PUCheck();
    }

    public void LengthChange(int value)
    {
        switch (value)
        {
            case int n when (n < 0):
                for (int i = 0; i < Mathf.Abs(value); i++)
                {
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
                repeat_time = 0.1f / Mathf.Pow(1.25f, (FoodCount / 5));
                break;
            case int n when (n > 0):
                for (int i = 0; i < value; i++)
                {
                    StartCoroutine("Wait", 0.02f);
                    GameObject SnakeS = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SnakeSegment"), _segments[_segments.Count - 1].position, Quaternion.identity);
                    Transform segment = SnakeS.transform;
                    segment.position = _segments[_segments.Count - 1].position;
                    _segments.Add(segment);
                }
                break;
        }
    }

    private void PlayerDied(int ID)
    {
        Place -= 1;
        if (ID == PV.ViewID)
        {
            Kills += 1;
        }
        if(Place == 1)
        {
            Win();
        }
    }

    #endregion

    #region COROUTINES
    IEnumerator Direction()
    {
        while (PV.IsMine)
        {
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
            yield return 0;
        }
    }

    IEnumerator TargetChange()
    {
        while (PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TargetIncrement();
            }
            yield return 0;
        }
    }

    IEnumerator PlayerTarget()
    {
        while (PV.IsMine)
        {
            PVInstances = PlayerPVScript.GetPlayerViewList();
            if (PVInstances.Count > 1)
            {
                switch (BGListPointer)
                {
                    case 0: //Random Player
                        PlayerID = PVInstances[Random.Range(0, PlayerPVScript.Instances.Count)].ViewID;
                        while (PlayerID == PV.ViewID)
                        {
                            PlayerID = PVInstances[Random.Range(0, PlayerPVScript.Instances.Count)].ViewID;
                        }
                        break;
                    case 1: //Max Kills
                        int MaxValue = 0;
                        foreach (PhotonView pv in PVInstances)
                        {
                            if (!pv.IsMine)
                            {
                                Player player = pv.Controller;
                                pv.RPC("RPC_CallToPlayer", player, pv.ViewID, MyID, 1);
                                yield return new WaitUntil(() => IsChecked);
                                if (tempValue > MaxValue)
                                {
                                    MaxValue = tempValue;
                                    PlayerID = pv.ViewID;
                                }
                            }
                        }
                        break;
                    case 2: //Max Score 
                        int MaxValue2 = 0;
                        foreach (PhotonView pv in PVInstances)
                        {
                            if (!pv.IsMine)
                            {
                                Player player = pv.Controller;
                                IsChecked = false;
                                pv.RPC("RPC_CallToPlayer", player, pv.ViewID, MyID, 2);
                                yield return new WaitUntil(() => IsChecked);
                                if (this.tempValue > MaxValue2)
                                {
                                    MaxValue2 = this.tempValue;
                                    PlayerID = pv.ViewID;
                                }
                            }
                        }
                        break;
                    case 3: //Attacker
                        foreach (PhotonView pv in PVInstances)
                        {
                            if (!pv.IsMine)
                            {
                                Player player = pv.Controller;
                                IsChecked = false;
                                pv.RPC("RPC_CallToPlayer", player, pv.ViewID, MyID, 3);
                                yield return new WaitUntil(() => IsChecked);
                                if (tempValue == -10)
                                {
                                    PlayerID = pv.ViewID;
                                }
                            }
                        }
                        break;
                }

                TransformDict = PlayerPVScript.GetPlayerTransformDict();
                for(int i = 0; i < PVInstances.Count; i++)
                {
                    Debug.LogError("Player " + PVInstances[i].ViewID + " At transform " +  TransformDict[PVInstances[i].ViewID].position);
                }
                TargetPosition = TransformDict[PlayerID].position;

                if (!BorderPrefab.activeInHierarchy) { BorderPrefab.SetActive(true); }
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
            if (FoodCount > 0 && FoodCount % 5 == 0 && IsSpeedable == true)
            {
                repeat_time = repeat_time / 1.25f;
                Debug.Log("Speed Up");
                IsSpeedable = false;
            }
            yield return new WaitForSeconds(repeat_time);
        }
    }

    IEnumerator Wait(float time)
    {
        Waiting = true;
        //Debug.Log("Waiting");
        yield return new WaitForSecondsRealtime(time);
        //Debug.Log("Waited");
        Waiting = false;
    }

    IEnumerator PUActive(Image image)
    {
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
        WallsList[0].SetActive(false);
        WallsList[(int)type].SetActive(true);
        foodScript.SetWallsObj();
        foodScript.RandomPos();

        yield return new WaitForSeconds(time);

        WallsList[(int)type].SetActive(false);
        WallsList[0].SetActive(true);
        foodScript.SetWallsObj();
        foodScript.RandomPos();
    }

    public IEnumerator Boost(float time, float mult)
    {
        repeat_time = (0.1f / Mathf.Pow(1.25f, (FoodCount / 5))) / mult;
        yield return new WaitForSeconds(time);
        repeat_time = 0.1f / Mathf.Pow(1.25f, (FoodCount / 5));

    }

    #endregion
}