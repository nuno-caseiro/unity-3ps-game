using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;


public class GameManager : MonoBehaviourPunCallbacks
{

    public Camera sceneCam;
    public GameObject player;
    public Transform playerSpawnPosition;
    public GameObject poolManager;
    public GameObject spawnEnemyPoint;

    [HideInInspector]
    public GameObject localPlayer;

    private float timerCountdown = 600;
    public bool running = false;

    public Text timer;
    public GameObject myScore;


    int totalPlayers = 0;

    public GameObject deathScreen;
    public GameObject spectateContainer;
    public GameObject spectateObject;

    public GameObject gameOverScreen;
    public GameObject finishContainer;
    public GameObject finishPlayerObject;

    private int nPlayers;

    int deadPlayers = 0;

    private bool clickedExit = false;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 600;
        sceneCam.enabled = false;
       
        localPlayer = PhotonNetwork.Instantiate(player.name, playerSpawnPosition.position,playerSpawnPosition.rotation);

        totalPlayers = PhotonNetwork.PlayerList.Length;
        print("TOTAL PLAYERS" + totalPlayers);
        if (PhotonNetwork.IsMasterClient)
        {
            spawnEnemyPoint.SetActive(true);
            poolManager.SetActive(true);
        }

        /*  if (PhotonNetwork.IsMasterClient)
          {
              PhotonNetwork.Instantiate(enemy.name, enemySpawnPosition1.position, enemySpawnPosition1.rotation);
          }*/

        nPlayers  = PhotonNetwork.PlayerList.Length;

        running = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        //back buton go main menu por exemplo
        //print(PhotonNetwork.PlayerList.Length);
       /* if (nPlayers != PhotonNetwork.PlayerList.Length)
        {
            //roomCreator = null;
            //PhotonNetwork.Disconnect();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom(true);
                PhotonNetwork.LoadLevel(0);
            }
            
            return;
        }
       */
        calculateTime();
        
    }

    private void FixedUpdate()
    {
        
    }

    public void Spectate()
    { 
        FindAllPlayer();
    }

    void FindAllPlayer()
    {

        totalPlayers = PhotonNetwork.PlayerList.Length;
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");

        if (localPlayer.GetComponent<MyPlayer>().isDead)
        {
            sceneCam.enabled = true;
            deathScreen.SetActive(true);
            deathScreen.transform.Find("PlayerPoints").GetComponent<Text>().text = player.GetComponent<MyPlayer>().points.ToString();
            hidePlayerCanvasElements();
        }

        foreach (GameObject player in players)
        {
            player.GetPhotonView().RPC("callUpdateSpectate", RpcTarget.All);
        }



        /* foreach (GameObject player in players)
         {

             if (!player.GetComponent<MyPlayer>().isDead)
             {
                 GameObject so = Instantiate(spectateObject, spectateContainer.transform);
                 so.transform.Find("PlayerName").GetComponent<Text>().text = player.GetPhotonView().Owner.NickName;
                 so.transform.Find("SpectateButton").GetComponent<SpectateButtonClick>().target = player;
             }
             else
             {
                 deadPlayers++;
             }
         }
       */

        print(deadPlayers);
        if (deadPlayers == totalPlayers)
        {
            foreach (GameObject player in players)
            {
               
                    player.GetPhotonView().RPC("GameOver", RpcTarget.All);
                

            }
        }


    }

    

    public void updateSpectate()
    {
        print("AQUI");
        deadPlayers = 0;
        if (localPlayer.GetPhotonView().IsMine && localPlayer.GetComponent<MyPlayer>().isDead && deathScreen.activeSelf)
        {

            GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
            foreach (GameObject player in players)
            {
                string playerName = player.GetPhotonView().Owner.NickName;
                if (!player.GetComponent<MyPlayer>().isDead)
                {
                    if (!playerNameExists(playerName, false))
                    {
                        GameObject so = Instantiate(spectateObject, spectateContainer.transform);
                        so.transform.Find("PlayerName").GetComponent<Text>().text = playerName;
                        so.transform.Find("SpectateButton").GetComponent<SpectateButtonClick>().target = player;
                    }

                }
                else
                {
                    deadPlayers++;
                }


                if(playerNameExists(playerName,false) && player.GetComponent<MyPlayer>().isDead)
                {
                    playerNameExists(playerName, true);
                }
            }
        }
      
    }


    private bool playerNameExists(string playerName, bool toRemove)
    {
        Transform ch = spectateContainer.transform;
        print(ch.transform.name);
        print(spectateContainer.transform.childCount);
        for(int i = 0; i< spectateContainer.transform.childCount; i++)
        {
            print(ch.GetChild(i).gameObject.name);
            if (ch.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text == playerName && !toRemove)
            {
                return true;
            }

            if (ch.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text == playerName && toRemove)
            {
                Destroy(ch.GetChild(i).gameObject);
            }
        }
        


    
       
     return false;
    }


    void FinishGame()
    { 
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().Owner.IsMasterClient)
            {
                player.GetPhotonView().RPC("GameOver", RpcTarget.All);
            }
        }
    }



    void calculateTime()
    {
        if (running)
        {
            if (timerCountdown > 0)
            {
                timerCountdown -= Time.deltaTime;
                DisplayTime(timerCountdown);
            }
            else
            {
                Debug.Log("Time has run out!");
                timerCountdown = 0;
                running = false;
                FinishGame();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void ShowWinScreen()
    {

        running = false;
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        gameOverScreen.SetActive(true);
        foreach (GameObject player in players)
        {
            GameObject so = Instantiate(finishPlayerObject, finishContainer.transform);
            if (player.GetComponent<PhotonView>().IsMine)
            {
                so.transform.Find("PlayerName").GetComponent<Text>().text = "You";
            }
            else
            {
                so.transform.Find("PlayerName").GetComponent<Text>().text = player.GetPhotonView().Owner.NickName;    
            }

            so.transform.Find("Score").GetComponent<Text>().text = player.GetComponent<MyPlayer>().points.ToString();
            print(player.GetComponent<MyPlayer>().points);
        }

        hidePlayerCanvasElements();

    }

    public void hidePlayerCanvasElements()
    {
        localPlayer.GetComponent<MyPlayer>().healthBar.SetActive(false);
        localPlayer.GetComponent<MyPlayer>().chatSystem.SetActive(false);
        localPlayer.GetComponent<MyPlayer>().pointsParent.SetActive(false);
        timer.text = "";
    }

    public void OnClick_GoBackToLobby()
    {
        clickedExit = true;
        PhotonNetwork.LeaveRoom(true);
        PhotonNetwork.LoadLevel(0);
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
            base.OnPlayerLeftRoom(otherPlayer);
        if (running && !clickedExit)
        {
            print("REMOVED");
            Destroy(localPlayer);
            PhotonNetwork.LeaveRoom(true);
            PhotonNetwork.LoadLevel(0);
        }
      
    }
}
