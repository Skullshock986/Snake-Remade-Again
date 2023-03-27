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
    PhotonView CallPV;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>(); // Grab the Photon View of the player

        if (!PV.IsMine)
        {
            // Run this script only on the local player
            return;
        }

        // Grab the GamesItemHandler script from within the Game scene
        Handler = GameObject.FindGameObjectWithTag("GameController").GetComponent<GamesItemHandler>();

        Snake = GetComponent<PlayerController>();

        // Reset the Power-Up holders, allowing them to be set active when needed
        ResetPU1();
        ResetPU2();
        ResetPU3();
 

    }


    public void StartPickup(int Choice)
    {
        StartCoroutine(Pickup(Choice));
    }

    public IEnumerator Pickup(int Choice)
    {
        switch (Choice)
        {
            case 1: // A power-up from ItemStack 1 is chosen
                if (heldPU1 == -1 && canPickUp1) // Only continue if the player doesn't already have a powerup from this ItemStack and is able to pick up from it
                {
                    // Set canPickUp to false, so that a power-up cannot be picked again while one is already being held
                    canPickUp1 = false;

                    yield return new WaitForSeconds(DelayBeforePickup);

                    // Choose a random power-up from ItemStack 1 to be held
                    int PURand = Random.Range(0, Handler.ItemStack1.Length);
                    PUUse1 = Handler.ItemStack1[PURand];
                    heldPU1 = PURand;
                }
                break;
            case 2: // A power-up from ItemStack 2 is chosen
                if (heldPU2 == -1 && canPickUp2) // Only continue if the player doesn't already have a powerup from this ItemStack and is able to pick up from it
                {
                    // Set canPickUp to false, so that a power-up cannot be picked again while one is already being held
                    canPickUp2 = false;

                    yield return new WaitForSeconds(DelayBeforePickup);

                    // Choose a random power-up from ItemStack 2 to be held
                    int PURand = Random.Range(0, Handler.ItemStack2.Length);
                    PUUse2 = Handler.ItemStack2[PURand];
                    heldPU2 = PURand;
                }
                break;
            case 3: // A power-up from ItemStack 3 is chosen
                if (heldPU3 == -1 && canPickUp3) // Only continue if the player doesn't already have a powerup from this ItemStack and is able to pick up from it
                {
                    // Set canPickUp to false, so that a power-up cannot be picked again while one is already being held
                    canPickUp3 = false;

                    yield return new WaitForSeconds(DelayBeforePickup);

                    // Choose a random power-up from ItemStack 2 to be held
                    int PURand = Random.Range(0, Handler.ItemStack3.Length);
                    PUUse3 = Handler.ItemStack3[PURand];
                    heldPU3 = PURand;
                }
                break;
        }
    }

    public void GetController()
    {
        // Get a list of every player in the room
        List<PhotonView> PVInstances = PlayerPVScript.GetPlayerViewList();
        foreach (PhotonView pv in PVInstances)
        {
            // If the photon view of the player being checked is the same as that of the local player's target, grab their Controller and Photon View
            if (pv.ViewID == Snake.PlayerID)
            {
                player = pv.Controller;
                CallPV = pv;
            }
        }
    }

    public void ActivatePU1()
    {
        GetController();

        if (PUUse1 is ScorePU) 
        {
            CallPV.RPC("RPC_ScoreChange", player, PUUse1.Value, PV.ViewID);
        }
        else if (PUUse1 is LengthPU)
        {
            CallPV.RPC("RPC_LengthChange", player, PUUse1.Value, PV.ViewID);
        }
        else if (PUUse1 is WallsPU)
        {
            CallPV.RPC("RPC_WallsChange", player, PUUse1.ActivateTime, PUUse1.Value, PV.ViewID);
        }

        ResetPU1();
    }

    public void ActivatePU2()
    {
        // Grab the controller and photon view of the player you are targeting
        GetController();
        if (PUUse2 is BoostPU) // If the power-up is a boosting one, check the value it holds:
        {
            if (PUUse2.Value == 1.5f)
            {
                // If its value is 1.5, it is meant for the targetted player, so send RPC_Boost to the target client
                CallPV.RPC("RPC_Boost", player, PUUse2.ActivateTime, PUUse2.Value, PV.ViewID, PV.ViewID);
            }
            else
            {
                // If its value is 0.5, it is meant for the local player, so call the Boost coroutine on the local client
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
        // Grab the controller and photon view of the player you are targeting
        GetController();

        if (PUUse3 is ScorePU)
        {
            // If the power-up is a score changing one, call a ScoreChange RPC to the target
            CallPV.RPC("RPC_ScoreChange", player, PUUse3.Value, PV.ViewID);
        }
        else if (PUUse3 is BoostPU)
        {
            // If the power-up is a boosting one, call a Boost RPC to the target
            CallPV.RPC("RPC_Boost", player, PUUse3.ActivateTime, PUUse3.Value, PV.ViewID);
        }
        else if (PUUse3 is LengthPU)
        {
            // If the power-up is a length changing one, call the local player's LengthChange function
            Snake.LengthChange((int)PUUse3.Value);
        }
        else if (PUUse3 is WallsPU)
        {
            // If the power-up is a wall changing one, call a WallChange RPC to the target
            CallPV.RPC("RPC_WallsChange", player, PUUse3.ActivateTime, PUUse3.Value, PV.ViewID);
        }

        // Reset the third power-up holder
        ResetPU3();
    }

    public void ResetPU1()
    {
        // Reset Power-Up holder 1 related variables to allow it to pick up power-ups
        PUUse1 = null;
        heldPU1 = -1;
        canPickUp1 = true;
    }

    public void ResetPU2()
    {
        // Reset Power-Up holder 2 related variables to allow it to pick up power-ups
        PUUse2 = null;
        heldPU2 = -1;
        canPickUp2 = true;
    }

    public void ResetPU3()
    {
        // Reset Power-Up holder 3 related variables to allow it to pick up power-ups
        PUUse3 = null;
        heldPU3 = -1;
        canPickUp3 = true;
    }


}
