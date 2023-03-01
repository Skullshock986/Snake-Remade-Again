using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] PlayerPVScript PVListScript;
    public List<PhotonView> PVInstances;
    public Dictionary<int, Transform> TransformDict;

    [SerializeField] int PlayerID;
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
    [SerializeField] GameObject ui;

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

    private bool IsPP1Active;
    private bool IsPP2Active;
    private bool IsPP3Active;

    [SerializeField] GameObject AreaWallsObj;
    [SerializeField] BoxCollider2D gridArea;

    PhotonView PV;

    #region UNITY

    void Awake()
    {
        instance = this;
        PV = GetComponent<PhotonView>();
        gridArea = AreaWallsObj.GetComponentInChildren<BoxCollider2D>();
        PlayerPosition = ControllerMain.transform.position;
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(ui);
            return;
        }

        PlayerID = 0;
        MyID = PV.ViewID;

        DirectionIndicator = "None";

        _segments = new List<Transform>();
        _segments.Add(this.transform);

        Waiting = false;
        IsSpeedable = false;
        FoodCount = 0;

        ColourReset();
        BGListPointer = -1;

        IncreaseAmt = 1;
        StartCoroutine("Movement");
        StartCoroutine("Direction");

        TargetIncrement();
        StartCoroutine("TargetChange");
        StartCoroutine("PlayerTarget");
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

        GameObject[] Segment = GameObject.FindGameObjectsWithTag("ObstacleS");
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


        repeat_time = 0.1f;
    }

    private void CallToPlayer(int CallID, int ReturnID, int _c)
    {
        if (_c > 0)
        {
            int rID = ReturnID;
            sentInt = 0;
            if (_c == 3)
            {
                this.PVInstances = PlayerPVScript.GetPlayerViewList();
                this.sentInt = (this.PVInstances[PlayerID].ViewID == CallID) ? -10 : 0;
            }
            else if (_c == 2)
            {
                Debug.LogError("sentInt = " + FoodCount);
                this.sentInt = this.FoodCount;
            }
            else if (_c == 1)
            {
                Debug.LogError("sentInt = " + Kills);
                this.sentInt = this.Kills;
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

    private void ReturnToPlayer(int _c)
    {
        this.tempValue = _c;
        this.IsChecked = true;
    }

    #endregion

    #region FUNCTIONS

    private void ColourReset()
    {
        var tempColour = PU1.color;
        tempColour.a = 0.3f;
        PU1.color = tempColour;

        tempColour = PU2.color;
        tempColour.a = 0.3f;
        PU2.color = tempColour;

        tempColour = PU3.color;
        tempColour.a = 0.3f;
        PU3.color = tempColour;
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
            StopCoroutine("Movement");
            PV.RPC("RPC_Die", RpcTarget.All);
            StartCoroutine("Movement");
            ColourReset();
        }
    }

    private void RandomPosSnake()
    {
        if (!PV.IsMine)
        {
            return;
        }

        Bounds bounds = gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
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
        if (FoodCount >= 15 && IsPP2Active == false)
        {
            StartCoroutine("PUActive", PU2);
            IsPP2Active = true;
        }
        if (FoodCount >= 20 && IsPP3Active == false)
        {
            StartCoroutine("PUActive", PU3);
            IsPP3Active = true;
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
                Debug.LogError(PlayerID);

                TransformDict = PlayerPVScript.GetPlayerTransformDict();
                Debug.LogError(TransformDict);
                TargetPosition = TransformDict[PlayerID].position;

                Debug.LogError(TargetPosition);
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
        while (PV.IsMine)
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

    #endregion
}