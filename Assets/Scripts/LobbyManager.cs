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
    public GameObject loadingScreen;

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
    public Button practice;

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
        roomUI.SetActive(true);
    }

    public void MaxPlayer1()
    {
        maxPlayers = 1;
        if (string.IsNullOrEmpty(userName.text))
        {
            userName.text = "User" + Random.Range(100, 999);
        }
        practice.interactable = false;
        PhotonNetwork.LocalPlayer.NickName = userName.text;
        showLoadingScreen();
        PhotonNetwork.JoinRandomRoom();
    }

    private void Awake()
    { 
        PhotonNetwork.ConnectUsingSettings();
       // PhotonNetwork.AutomaticallySyncScene = true;
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
        mainMenuUI.SetActive(true);
        userName.text = "Player" + Random.Range(100, 999);
        statusText.text = "Joined To Lobbyxw";
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        //PhotonNetwork.LoadLevel(1);
        if(maxPlayers == 1)
        {
           
            PhotonNetwork.LoadLevel(1);

        }
        else
        {
            roomUI.SetActive(false);
            lobbyUI.SetActive(true);
            int i = PhotonNetwork.PlayerList.Length; ;
            foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
               
                //AddPlayer(PhotonNetwork.LocalPlayer.NickName);
                AddPlayer(p.NickName, i);
                i--;
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
        roomOptions.EmptyRoomTtl = 0;
        PhotonNetwork.CreateRoom(roomName.ToString(), roomOptions, TypedLobby.Default);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        print(newPlayer.NickName);
        AddPlayer(newPlayer.NickName, PhotonNetwork.PlayerList.Length);
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
        roomOptions.EmptyRoomTtl = 0;
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
        roomOptions.EmptyRoomTtl = 0;
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
            SendMsg((byte)EventCodes.ready);
            startButton.interactable = false;
            startBtnText.text = "Wait...";

        }
        else
        {
            if (count == maxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                SendMsgAll((byte)EventCodes.loading);
               // SendMsg((byte)EventCodes.enter);
                
                
                //lobbyText.text = "All Set: Play the game scene";
            }
        }
    }

    public void showLoadingScreen()
    {
        mainMenuUI.SetActive(false);
        lobbyUI.SetActive(false);
        loadingScreen.SetActive(true);
    }


    enum EventCodes
    {
        ready = 1,
        loading = 2,
        enter = 3
    }
    int count = 1;

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object content = photonEvent.CustomData;
        EventCodes code = (EventCodes)eventCode;
       
        print(code);
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

        if(code == EventCodes.loading)
        {
            showLoadingScreen();
            PhotonNetwork.LoadLevel(1);
        }

        

    }

    public void SendMsg(byte eventCode)
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

        PhotonNetwork.RaiseEvent(eventCode, datas, options, sendOptions);
    }


    public void SendMsgAll(byte eventCode)
    {
        string message = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        object[] datas = new object[] { message };
        //cache options
        RaiseEventOptions options = new RaiseEventOptions
        {

            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All,
        };

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(eventCode, datas, options, sendOptions);
    }

    public void AddPlayer(string playerName, int nr)
    {
        GameObject pop = Instantiate(playerObjectPrefab, Vector3.zero, Quaternion.identity);
        pop.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("AvatarPlayers/Avatar" + nr);
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

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        lobbyUI.SetActive(false);
        foreach (Transform child in playersContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //limpar players



        //roomUI.SetActive(true);
        //mainMenuUI.SetActive(false);
    }


    public void ReturnMainMenu()
    {
        roomUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
