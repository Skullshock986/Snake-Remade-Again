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

    private bool IsPP1Active;
    private bool IsPP2Active;
    private bool IsPP3Active;

    [SerializeField] GameObject AreaWallsObj;
    [SerializeField] BoxCollider2D gridArea;

    PhotonView PV;

    void Awake()
    {
        instance = this;
        PV = GetComponent<PhotonView>();
        gridArea = AreaWallsObj.GetComponentInChildren<BoxCollider2D>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(ui);
            return;
        }

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
    }

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

    IEnumerator TargetChange()
    {
        while (PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TargetIncrement();
            }
        }
        yield return 0;
    }
}
