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
            spawnEnemy();
        }
    }

    void spawnEnemy()
    { 
            timerSpawnEnemy += Time.deltaTime;

            if (timerSpawnEnemy >= intervalToSpawnEnemy)
            {
                timerSpawnEnemy = 0;
                float x = Random.Range(-219, 304);
                float z = Random.Range(-349, 288);
            //Vector3 newPosition = new Vector3(x, transform.position.y, z);
            x = -115;
            z = -160;
            Vector3 newPosition = new Vector3(-115, 1, -160);
            GameObject zombie1 = GameObject.Find("PoolManager").GetComponent<PoolManager>().GetZombie();
            
            if (zombie1 != null)
            {
                print(zombie1.GetComponent<NavMeshAgentBrain>().death);
                if (zombie1.GetComponent<NavMeshAgentBrain>().death)
                {

                    zombie1.GetComponent<PhotonView>().RPC("ReBirth", RpcTarget.All, x, transform.position.y, z);
                    //zombie1.GetComponent<NavMeshAgentBrain>().ReBirth(newPosition);
                    print("NEW ZOMBIE FROM LIST");
                }
            }
            else
            {
                PhotonNetwork.Instantiate(enemy[Random.Range(0,2)].name, newPosition, transform.rotation);
            }

            }
    }
}
