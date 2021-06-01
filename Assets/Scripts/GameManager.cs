using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;


public class GameManager : MonoBehaviour
{

    public Camera sceneCam;
    public GameObject player;
    public Transform playerSpawnPosition;

    public Transform enemySpawnPosition1;
    public GameObject enemy;


    [HideInInspector]
    public GameObject localPlayer;

    private float timerCountdown = 600;
    public bool running = false;

    private float intervalToSpawnEnemy = 10;
    private float timerSpawnEnemy = 0;

    public Text timer;
    public GameObject teamScore;

    public List<GameObject> zombieList;

    int totalPlayers = 0;

    public GameObject deathScreen;
    public GameObject spectateContainer;
    public GameObject spectateObject;
    public GameObject gameOverScreen;
    


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 600;
        sceneCam.enabled = false;
        localPlayer = PhotonNetwork.Instantiate(player.name, playerSpawnPosition.position,playerSpawnPosition.rotation);

        totalPlayers = PhotonNetwork.PlayerList.Length;

        /*  if (PhotonNetwork.IsMasterClient)
          {
              PhotonNetwork.Instantiate(enemy.name, enemySpawnPosition1.position, enemySpawnPosition1.rotation);
          }*/

        running = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        //back buton go main menu por exemplo
        calculateTime();
        spawnEnemy();
        
    }

    private void FixedUpdate()
    {
        
    }

    public void Spectate()
    {
        sceneCam.enabled = true;
        deathScreen.SetActive(true);
        FindAllPlayer();
    }

    void FindAllPlayer()
    {
        int deadPlayers = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
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
                if (deadPlayers == totalPlayers)
                {
                    player.GetPhotonView().RPC("GameOver", RpcTarget.All);
                }
            }
        }
    }

    void FinishGame()
    { 
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetPhotonView().RPC("GameOver", RpcTarget.All);
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


    void spawnEnemy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timerSpawnEnemy += Time.deltaTime;

            if(timerSpawnEnemy >= intervalToSpawnEnemy)
            {
                timerSpawnEnemy = 0;
                Transform spawnPoint = chooseRandomSpawnPoint();
                float x = UnityEngine.Random.Range(-100, 100);
                float z = UnityEngine.Random.Range(-100, 100);
                Vector3 newPosition = spawnPoint.position + new Vector3(x, spawnPoint.position.y, z);

                if (zombieList.Count > 0)
                {
                    zombieList[0].transform.position = newPosition;
                    zombieList[0].GetComponent<NavMeshAgentBrain>().health = 1;
                    zombieList[0].GetComponent<NavMeshAgentBrain>().death = false;
                    zombieList[0].SetActive(true);
                    zombieList.RemoveAt(0);
                    print("NEW ZOMBIE FROM LIST");
                }
                else
                {
                    PhotonNetwork.Instantiate(enemy.name, newPosition, enemySpawnPosition1.rotation);
                }
               
                
            }
            
        }
            
    }

    private Transform chooseRandomSpawnPoint()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
        int index = UnityEngine.Random.Range(0, spawnPoints.Length - 1);
        return spawnPoints[index].transform;
    }

    public void ShowWinScreen()
    {
        gameOverScreen.SetActive(true);
    }

    public void OnClick_GoBackToLobby()
    {
        PhotonNetwork.LeaveRoom(true);
        PhotonNetwork.LoadLevel(0);
        
    }
}
