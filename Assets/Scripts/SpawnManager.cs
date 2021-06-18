using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    private float intervalToSpawnEnemy = 10;
    private float timerSpawnEnemy = 0;
    public GameObject[] enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient && transform.gameObject.activeSelf && GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            CalculateTimeForSpawnEnemy();
        }
    }

    public void CalculateTimeForSpawnEnemy()
    { 
            timerSpawnEnemy += Time.deltaTime;

            if (timerSpawnEnemy >= intervalToSpawnEnemy)
            {
                timerSpawnEnemy = 0;
            /*       float x = Random.Range(-219, 304);
                   float z = Random.Range(-349, 288);
               //Vector3 newPosition = new Vector3(x, transform.position.y, z);
               x = -115;
               z = -160;
               Vector3 newPosition = new Vector3(-115, 1, -160);*/

            spawnEnemy();
                
            }
    }


    public void spawnEnemy()
    {
        Vector3 newPosition = calculatePosition();

        GameObject zombie1 = GameObject.Find("PoolManager").GetComponent<PoolManager>().GetZombie();

        if (zombie1 != null)
        {
            print(zombie1.GetComponent<NavMeshAgentBrain>().death);
            if (zombie1.GetComponent<NavMeshAgentBrain>().death)
            {

                zombie1.GetComponent<PhotonView>().RPC("ReBirth", RpcTarget.All, newPosition.x, transform.position.y, newPosition.z);
                //zombie1.GetComponent<NavMeshAgentBrain>().ReBirth(newPosition);
                print("NEW ZOMBIE FROM LIST");
            }
        }
        else
        {
            PhotonNetwork.Instantiate(enemy[Random.Range(0, 3)].name, newPosition, transform.rotation);
            // PhotonNetwork.Instantiate(enemy[0].name, newPosition, transform.rotation);
        }
    }


    public Vector3 calculatePosition()
    {

        float x = UnityEngine.Random.Range(-173, 301);
        float z = UnityEngine.Random.Range(-320, 200);
        //Vector3 newPosition = new Vector3(x, transform.position.y, z);
        // x = -115;
        // z = -160;

        Vector3 position;
        do
        {
            position = getRandomPosition();
        }
        while (IsSamePositionAsPlayer(position) || IsSamePositionAsZombie(position));


        return position;
    }


    public bool IsSamePositionAsPlayer(Vector3 vector3)
    {
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        foreach (GameObject player in players)
        {
             if(Vector3.Distance(player.transform.position,vector3) < 3)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsSamePositionAsZombie(Vector3 vector3)
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

       
        foreach (GameObject zombie in zombies)
        {
            if (zombie.transform.position == vector3)
            {
                return true;
            }
        }

        return false;
    }




    public Vector3 getRandomPosition()
    {
        float x = Random.Range(-219, 304);
        float z = Random.Range(-349, 288);
        //Vector3 newPosition = new Vector3(x, transform.position.y, z);
        // x = -115;
        // z = -160;

        Vector3 newPosition = new Vector3(x, 1, z);

        return newPosition;
    }
}
