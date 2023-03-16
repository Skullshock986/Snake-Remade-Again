using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks  // Extending the class from MonoBehaviour to allow callbacks for room creation, joining lobbies, etc
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    void Awake()
    {
        // Allows this script to be called by other scripts
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Connects to the Photon Master Server as configured in the given Settings file
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    // Calls upon connecting to the Photon Master Server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby(); // Allows the User to create or join rooms
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Calls upon joining a Lobby
    public override void OnJoinedLobby()
    {
        // Opens the Title menu
        MenuManager.Instance.OpenMenu("Title");
        Debug.Log("Joined the Lobby");

        // Assign each player a random number
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }


    // Calls upon a player creating a room (button press trigger)
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            return; // Do not run this function if the room name is empty
        }
        PhotonNetwork.CreateRoom(roomNameInput.text);
        MenuManager.Instance.OpenMenu("Loading"); // Show Loading Screen while a room with the inputted name is made
    }   


    // Calls upon a player entering an existing room or a room they created
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // Show the room menu with the Title as the Room Name

        // Make a new list of all player in the room when joining.
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            // Instantiate a card for each player in the lobby
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); // Allow the Room Host to start the game when needed
    }

    // Calls when the active host leaves the lobby
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Set a random player as the new host
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); 
    }

    // Calls when there is an error in creating a room
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("Error");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Join Failed: " + message;
        MenuManager.Instance.OpenMenu("Error");
    }

    // Calls upon starting the game
    public void StartGame()
    {
        // Load the game Scene
        PhotonNetwork.LoadLevel(2);
    }

    // Calls when a player leaves a room (for the player only) (Button Trigger)
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // Notify the server that the player has left
        MenuManager.Instance.OpenMenu("Loading"); // Make the player wait while they leave
    }

    // Calls when a player has finished leaving a room (local player only)
    public override void OnLeftRoom()
    {
        // Send the player to the Title Menu
        MenuManager.Instance.OpenMenu("Title"); 
    }

    // Calls when a player joins a room (local player only)
    public void JoinRoom(RoomInfo info)
    {
        // Notify the server that the player has entered the room and make the player wait until they have fully joined
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");  
    }


    // Calls every time a room is created/destroyed (for all other players in the lobby)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            // Destroy all room cards currently in the lobby
            Destroy(trans.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++)
        {
            // add all room cards in the lobby list (if they have not been already removed)
            if (roomList[i].RemovedFromList) { continue; }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]); 
        }
    }

    // Calls when a Player enters a room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Create a new player card and add it to the room
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
