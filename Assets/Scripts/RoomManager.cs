using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks // Extending the class from MonoBehaviour to allow photon Callbacks
{

    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance) // If another RoomManager exists..
        {
            Destroy(gameObject);
            return; // Destroy itsef
        }
        DontDestroyOnLoad(gameObject); // Ensure there is only one RoomManager
        Instance = this;
    }


    public override void OnEnable() // Calls when the RoomManager is enabled
    {
        base.OnEnable(); // Base OnEnable method is called
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded callback with the OnSceneLoaded method
    }

    public override void OnDisable() // Calls when the RoomManager is disabled
    {
        base.OnDisable(); // Base OnDisable method is called
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to the sceneLoaded callback with the OnSceneLoaded method
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) // Calls when a player's scene is changed
    {
        if(scene.buildIndex == 2)
        {
            // If the player switched to the game scene, instantiate the PlayerManager prefab
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
