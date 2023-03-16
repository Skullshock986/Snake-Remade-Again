using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public PlayerController Snake;
    public GamesItemHandler Handler;

    public float DelayBeforePickup = 1.0f;

    public int heldPU1;
    public int heldPU2;
    public int heldPU3;

    public bool canPickUp1;
    public bool canPickUp2;
    public bool canPickUp3;

    public PUItem PUUse1;
    public PUItem PUUse2;
    public PUItem PUUse3;

    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        if (!PV.IsMine)
        {
            return;
        }

        ResetPU1();
        ResetPU2();
        ResetPU3();

        Handler = GameObject.FindGameObjectWithTag("GameController").GetComponent<GamesItemHandler>();

        Snake = GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartPickup(int Choice)
    {
        StartCoroutine(Pickup(Choice));
    }

    public IEnumerator Pickup(int Choice)
    {
        switch (Choice)
        {
            case 1:
                if (heldPU1 == -1 && canPickUp1)
                {
                    canPickUp1 = false;

                    yield return new WaitForSeconds(DelayBeforePickup);

                    int PURand = Random.Range(0, Handler.ItemStack1.Length);

                    PUUse1 = Handler.ItemStack1[PURand];

                    heldPU1 = PURand;
                }
                break;
            case 2:
                if (heldPU2 == -1 && canPickUp2)
                {
                    canPickUp2 = false;

                    yield return new WaitForSeconds(DelayBeforePickup);

                    int PURand = Random.Range(0, Handler.ItemStack2.Length);

                    PUUse2 = Handler.ItemStack2[PURand];

                    heldPU2 = PURand;
                }
                break;
            case 3:
                if (heldPU3 == -1 && canPickUp3)
                {
                    canPickUp3 = false;

                    yield return new WaitForSeconds(DelayBeforePickup);

                    int PURand = Random.Range(0, Handler.ItemStack3.Length);

                    PUUse3 = Handler.ItemStack3[PURand];

                    heldPU3 = PURand;
                }
                break;
        }
    }

    public void ActivatePU1()
    {
        List<PhotonView> PVInstances = PlayerPVScript.GetPlayerViewList();
        if (PUUse1 is ScorePU)
        {
            foreach (PhotonView pv in PVInstances)
            {
                if (pv.ViewID == Snake.PlayerID)
                {
                    Player player = pv.Controller;
                    pv.RPC("RPC_ScoreChange", player, PUUse1.Value, PV.ViewID);
                }
            }
        }
        else if (PUUse1 is LengthPU)
        {
            foreach (PhotonView pv in PVInstances)
            {
                if (pv.ViewID == Snake.PlayerID)
                {
                    Player player = pv.Controller;
                    pv.RPC("RPC_LengthChange", player, PUUse1.Value, PV.ViewID);
                }
            }
        }
        else if (PUUse1 is WallsPU)
        {
            foreach (PhotonView pv in PVInstances)
            {
                if (pv.ViewID == Snake.PlayerID)
                {
                    Player player = pv.Controller;
                    pv.RPC("RPC_WallsChange", player, PUUse1.ActivateTime, PUUse1.Value, PV.ViewID);
                }
            }
        }

        ResetPU1();
    }

    public void ActivatePU2()
    {
        List<PhotonView> PVInstances = PlayerPVScript.GetPlayerViewList();
        if (PUUse2 is BoostPU)
        {
            if (PUUse2.Value == 1.5f)
            {
                foreach (PhotonView pv in PVInstances)
                {
                    if (pv.ViewID == Snake.PlayerID)
                    {
                        Player player = pv.Controller;
                        pv.RPC("RPC_Boost", player, PUUse2.ActivateTime, PUUse2.Value, PV.ViewID, PV.ViewID);
                    }
                }
            }
            else
            {
                StartCoroutine(Snake.Boost(PUUse2.ActivateTime, PUUse2.Value));
            }
        }
        else if (PUUse2 is WallsPU)
        {
            StartCoroutine(Snake.WallsChange(PUUse2.ActivateTime, PUUse2.Value));
        }

        ResetPU2();
    }

    public void ActivatePU3()
    {
        List<PhotonView> PVInstances = PlayerPVScript.GetPlayerViewList();
        if (PUUse3 is ScorePU)
        {
            foreach (PhotonView pv in PVInstances)
            {
                if (pv.ViewID == Snake.PlayerID)
                {
                    Player player = pv.Controller;
                    pv.RPC("RPC_ScoreChange", player, PUUse3.Value, PV.ViewID);
                }
            }
        }
        else if (PUUse3 is BoostPU)
        {
            foreach (PhotonView pv in PVInstances)
            {
                if (pv.ViewID == Snake.PlayerID)
                {
                    Player player = pv.Controller;
                    pv.RPC("RPC_Boost", player, PUUse3.ActivateTime, PUUse3.Value, PV.ViewID);
                }
            }
        }
        else if (PUUse3 is LengthPU)
        {
            Snake.LengthChange((int)PUUse3.Value);
        }
        else if (PUUse3 is WallsPU)
        {
            foreach (PhotonView pv in PVInstances)
            {
                if (pv.ViewID == Snake.PlayerID)
                {
                    Player player = pv.Controller;
                    pv.RPC("RPC_WallsChange", player, PUUse3.ActivateTime, PUUse3.Value, PV.ViewID);
                }
            }
        }

        ResetPU3();
    }

    public void ResetPU1()
    {
        PUUse1 = null;
        heldPU1 = -1;
        canPickUp1 = true;
    }

    public void ResetPU2()
    {
        PUUse2 = null;
        heldPU2 = -1;
        canPickUp2 = true;
    }

    public void ResetPU3()
    {
        PUUse3 = null;
        heldPU3 = -1;
        canPickUp3 = true;
    }


}
