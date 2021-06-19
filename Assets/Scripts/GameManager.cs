using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

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
    public GameObject timerParent;
    public bool running = false;

    public Text timer;
    public GameObject myScore;

    public GameObject aim;


    int totalPlayers = 0;

    public GameObject deathScreen;
    public GameObject spectateContainer;
    public GameObject spectateObject;

    public GameObject gameOverScreen;
    public GameObject finishContainer;
    public GameObject finishPlayerObject;

    public GameObject scoreContainer;

    public GameObject pauseMenu;

    public GameObject sun;

    public GameObject settings;

    private int nPlayers;

    int deadPlayers = 0;

    private bool clickedExit = false;

    //private string masterClient;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 600;
        sceneCam.enabled = false;

        localPlayer = PhotonNetwork.Instantiate(player.name, getRandomPosition(), Quaternion.identity) ;

        PhotonNetwork.AutomaticallySyncScene = false;

        totalPlayers = PhotonNetwork.PlayerList.Length;
        print("TOTAL PLAYERS" + totalPlayers);
        if (PhotonNetwork.IsMasterClient)
        {
            spawnEnemyPoint.SetActive(true);
            poolManager.SetActive(true);
            for(int i = 0; i< 15; i++)
            {
                spawnEnemyPoint.GetComponent<SpawnManager>().spawnEnemy();
            }
        }

        /*  if (PhotonNetwork.IsMasterClient)
          {
              PhotonNetwork.Instantiate(enemy.name, enemySpawnPosition1.position, enemySpawnPosition1.rotation);
          }*/

        nPlayers  = PhotonNetwork.PlayerList.Length;

       // masterClient = MasterClientName();
       
        running = true;

        timerParent.SetActive(true);
        
    }

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       /* if(masterClient == "None") { 
        
            masterClient = MasterClientName();
        }


        print("Masterclient:" + masterClient);*/


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

        if (scoreContainer.transform.childCount != totalPlayers && !localPlayer.GetComponent<MyPlayer>().isDead)
        {
            
            GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
            if(totalPlayers == players.Length)
            {
                PopulateScoreScreen(scoreContainer);
            }
            
        }

        calculateTime();

        if(running && timerCountdown <= 180 && sun.GetComponent<Light>().intensity>0)
        {
            if (localPlayer.GetComponent<MyPlayer>().ambient.isPlaying)
            {
                localPlayer.GetComponent<MyPlayer>().ambient.Stop();
                localPlayer.GetComponent<MyPlayer>().intense.Play();
                localPlayer.GetComponent<MyPlayer>().intense.loop = true;

            }
            sun.GetComponent<Light>().intensity -= 0.08f * Time.deltaTime;
        }

        if (sun.GetComponent<Light>().intensity <= 0)
        {
            sun.SetActive(false);
        }
        
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

        deadPlayers = 0;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<MyPlayer>().isDead)
            {
                deadPlayers++;
            }
            player.GetPhotonView().RPC("callUpdateSpectate", RpcTarget.All);
        }

       
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
      
        for(int i = 0; i< spectateContainer.transform.childCount; i++)
        {
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
           // if (player.GetComponent<PhotonView>().Owner.IsMasterClient)
           // {
                player.GetPhotonView().RPC("GameOver", RpcTarget.All);
           // }
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


    public void PopulateScoreScreen(GameObject container)
    {
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        if (container.gameObject.name.Contains("ScoreContainer"))
        {
            scoreContainer.SetActive(true);
        }
        else
        {
            gameOverScreen.SetActive(true);
        }
       

        foreach (GameObject player in players)
        {
         
            GameObject so = Instantiate(finishPlayerObject, container.transform);
            so.transform.Find("PlayerName").GetComponent<Text>().text = player.GetPhotonView().Owner.NickName;
            /*  if (player.GetComponent<PhotonView>().IsMine)
              {
                  so.transform.Find("PlayerName").GetComponent<Text>().text = "You";
              }
              else
              {
                  so.transform.Find("PlayerName").GetComponent<Text>().text = player.GetPhotonView().Owner.NickName;    
              }*/

            so.transform.Find("Score").GetComponent<Text>().text = player.GetComponent<MyPlayer>().points.ToString();
           
        }


    }

    public void hidePlayerCanvasElements()
    {
        localPlayer.GetComponent<MyPlayer>().healthBar.SetActive(false);
        localPlayer.GetComponent<MyPlayer>().chatSystem.SetActive(false);
        localPlayer.GetComponent<MyPlayer>().pointsParent.SetActive(false);
        timer.text = "";
        timerParent.SetActive(false);
        pauseMenu.SetActive(false);
        aim.SetActive(false);
    }



    public void OnClick_GoBackToLobby()
    {
        clickedExit = true;
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel(0);
        //Application.LoadLevel(0);


    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel(0);
        //Application.LoadLevel(0);


    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
      
        //remover do r



       // removePlayerFromScoreContainer();
        nPlayers = PhotonNetwork.PlayerList.Length;

     /*   print(running && !clickedExit && !MasterClientAvailable());
        if (running && !clickedExit && !MasterClientAvailable())
        {
            print("REMOVED");
            PhotonNetwork.Destroy(localPlayer);
            PhotonNetwork.LeaveRoom();
           // PhotonNetwork.LoadLevel(0);
        }
      */
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        /*   if (PhotonNetwork.IsMasterClient)
           {
               poolManager.SetActive(true);
               spawnEnemyPoint.SetActive(true);
           }*/

        PhotonNetwork.LeaveRoom();
    }


    /*private bool MasterClientAvailable()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        print(players.Length);
        foreach(GameObject player in players)
        {
            print("Playername:" + player.GetPhotonView().Owner.NickName + "Master client:" + masterClient);
            if (player.GetPhotonView().Owner.NickName == masterClient)
            {
                return true;
            }
        }

        return false;
    }
    */

    [PunRPC]
    public void updateScoreContainer()
    {
        Transform sc = scoreContainer.transform;
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        foreach (GameObject player in players)
        {
            string playerName = player.GetPhotonView().Owner.NickName;

            for (int i = 0; i < players.Length; i++)
            {
                if (sc.GetChild(i).GetChild(0).GetComponent<Text>().text == playerName)
                {
                    sc.GetChild(i).GetChild(1).GetComponent<Text>().text = player.GetComponent<MyPlayer>().points.ToString();
                    break;
                }

            }
        }
    }


    public void removePlayerFromScoreContainer()
    {
        Transform sc = scoreContainer.transform;


        for (int i = 0; i < scoreContainer.transform.childCount; i++)
        {

            if (!PlayerAvailabe(sc, i))
            {
                Destroy(scoreContainer.transform.GetChild(i));
            }

        }

    }

    public bool PlayerAvailabe(Transform sc, int pos)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        foreach (GameObject player in players)
        {
            string playerName = player.GetPhotonView().Owner.NickName;
            if (sc.GetChild(pos).GetChild(0).GetComponent<Text>().text == playerName)
            {
                return true;
            }

        }

        return false;
    } 

    public Vector3 getRandomPosition()
    {
        float x = UnityEngine.Random.Range(-173, 301);
        float z = UnityEngine.Random.Range(-320, 200);
        //Vector3 newPosition = new Vector3(x, transform.position.y, z);
        // x = -115;
        // z = -160;

        Vector3 newPosition = new Vector3(x, 30, z);

        return newPosition;
    }

    public void ResumeGame() {
        pauseMenu.SetActive(false);
    }

    public void PauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);

        }
        else
        {
            pauseMenu.SetActive(true);

        }
    }

   /* public string MasterClientName()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        foreach (GameObject player in players)
        {

            if (player.GetComponent<PhotonView>().Owner.IsMasterClient)
            {
                masterClient = player.GetComponent<PhotonView>().Owner.NickName;
                return masterClient;
            }
        }
        return "None";
    }*/


    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }


}
