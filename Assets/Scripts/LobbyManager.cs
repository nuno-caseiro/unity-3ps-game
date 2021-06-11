using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("---UI Screens----")]
    public GameObject roomUI;
    public GameObject connectUI;
    public GameObject lobbyUI;
    public GameObject mainMenuUI;

    [Header("---UI Text----")]
    public Text statusText;
    public Text connectingText;
    public Text startBtnText;
    public Text lobbyText;
    

    [Header("---UI Inputfields----")]
    public InputField createRoom;
    public InputField joinRoom;
    public InputField userName;
    public Button startButton;

    public Dropdown dropdown;

    public byte maxPlayers = 2;

    //MANAGER 2
    public GameObject playersContainer;
    public GameObject playerObjectPrefab;

    private void Start()
    {
        Application.targetFrameRate = 600;
        dropdown.GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });
    }

    void DropdownValueChanged(Dropdown change)
    {

        switch (change.value)
        {
            case 0:
                maxPlayers = 2;
                break;
            case 1:
                maxPlayers = 3;
                break;
            case 2:
                maxPlayers = 4;
                break;

        }
        
    }

    public void OnClickMainMenu_Start()
    {
        mainMenuUI.SetActive(false);
    }

    public void MaxPlayer1()
    {
        maxPlayers = 1;
        if (string.IsNullOrEmpty(userName.text))
        {
            userName.text = "User" + Random.Range(100, 999);
        }

        PhotonNetwork.LocalPlayer.NickName = userName.text;
        PhotonNetwork.JoinRandomRoom();
    }

    private void Awake()
    { 
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        connectingText.text = "Joining Lobby...";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        
    }

    public override void OnJoinedLobby()
    {
        //base.OnJoinedLobby();
        connectUI.SetActive(false);
        roomUI.SetActive(true);
        userName.text = "Player" + Random.Range(100, 999);
        statusText.text = "Joined To Lobbyxw";
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        //PhotonNetwork.LoadLevel(1);
        if(maxPlayers == 1)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            roomUI.SetActive(false);
            lobbyUI.SetActive(true);

            foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                AddPlayer(PhotonNetwork.LocalPlayer.NickName);
            }
            //AddPlayer(PhotonNetwork.LocalPlayer.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                startBtnText.text = "Waiting for players";
            }
            else
            {
                startBtnText.text = "Ready!";
            }
        }

     
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);
        connectUI.SetActive(true);
        connectingText.text = "Disconnected..." + cause.ToString();
        roomUI.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        int roomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(roomName.ToString(), roomOptions, TypedLobby.Default);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        AddPlayer(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        print("REMOVED");
        RemovePlayer(otherPlayer.NickName);
    }


    public void Onclick_CreateBtn()
    {
        if (string.IsNullOrEmpty(userName.text))
        {
            userName.text = "User" + Random.Range(100, 999);
        }

        PhotonNetwork.LocalPlayer.NickName = userName.text;

        RoomOptions roomOptions = new RoomOptions();
        
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(createRoom.text, roomOptions, TypedLobby.Default);
    }

    public void Onclick_JoinBtn()
    {
        if (string.IsNullOrEmpty(userName.text))
        {
            userName.text = "User" + Random.Range(100, 999);
        }

        PhotonNetwork.LocalPlayer.NickName = userName.text;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.JoinOrCreateRoom(joinRoom.text, roomOptions, TypedLobby.Default);

    }

    public void OnClick_PlayBtn()
    {
        if (string.IsNullOrEmpty(userName.text))
        {
            userName.text = "User" + Random.Range(100, 999);
        }

        PhotonNetwork.LocalPlayer.NickName = userName.text;
        PhotonNetwork.JoinRandomRoom();
        statusText.text = "Creating room... Please wait...";

    }

    public void OnClickStartButton()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            SendMsg();
            startButton.interactable = false;
            startBtnText.text = "Wait...";

        }
        else
        {
            if (count == maxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel(1);
                //lobbyText.text = "All Set: Play the game scene";
            }
        }
    }


    enum EventCodes
    {
        ready = 1
    }
    int count = 1;

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object content = photonEvent.CustomData;
        EventCodes code = (EventCodes)eventCode;

        if(code == EventCodes.ready)
        {

            object[] datas = content as object[];

            if (PhotonNetwork.IsMasterClient)
            {
                count++;
                if (count == maxPlayers)
                    startBtnText.text = "Start !";
                else
                    startBtnText.text = "Only " + count + "/ 4 players are ready";
            }
        }

    }

    public void SendMsg()
    {
        string message = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        object[] datas = new object[] { message };
        //cache options
        RaiseEventOptions options = new RaiseEventOptions
        {

            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.MasterClient,
        };

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent((byte)EventCodes.ready, datas, options, sendOptions);
    }

    public void AddPlayer(string playerName)
    {
        GameObject pop = Instantiate(playerObjectPrefab, Vector3.zero, Quaternion.identity);
        pop.transform.GetChild(0).GetComponent<Text>().text = playerName;
        pop.transform.SetParent(playersContainer.transform,false);
        pop.name = playerName;
    }

    public void RemovePlayer(string playerName)
    {
        int popCount = playersContainer.transform.childCount;
        for(int i= 0; i< popCount; i++)
        {

            Text name = (Text)playersContainer.transform.GetChild(i).GetComponentInChildren<Text>();
            if (name.text == playerName)
            {
                Destroy(playersContainer.transform.GetChild(i).gameObject);
                return;
            }
                
        }
    }

}
