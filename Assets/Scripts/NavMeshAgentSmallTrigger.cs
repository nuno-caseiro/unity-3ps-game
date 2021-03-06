using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class NavMeshAgentSmallTrigger : MonoBehaviour
{
    public float minDistance;


    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnTriggerEnter(Collider other)
    {

        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            if (other.gameObject.tag == "Player")
            {
                GetComponentInParent<NavMeshAgentBrain>().playerInSmall = true;
                if (!other.GetComponentInParent<MyPlayer>().isDead && !GetComponentInParent<NavMeshAgentBrain>().Attacking)
                {
                    
                    GetComponentInParent<NavMeshAgentBrain>().Point = getClosestTarget();
                  
                    GetComponentInParent<NavMeshAgentBrain>().ReachTarget();
                    //GetComponentInParent<PhotonView>().RPC("ReachTarget", RpcTarget.All);
                }
            }
        }
        
     
    }

    public void OnTriggerStay(Collider other)
    {

        /*if (other.GetComponent<MyPlayer>().isDead)
        {
            print("SMALL TRIGGER DEAD?");
            GetComponentInParent<NavMeshAgentBrain>().stop();

        }*/

        if (other.gameObject.tag == "Player")
        {

            GetComponentInParent<NavMeshAgentBrain>().Look();
            GetComponentInParent<NavMeshAgentBrain>().playerInSmall = true;
            if (!GetComponentInParent<NavMeshAgentBrain>().Attacking)
            {
                
                GetComponentInParent<NavMeshAgentBrain>().Point = getClosestTarget();
                GetComponentInParent<NavMeshAgentBrain>().ReachTarget();
            }

            if (other.GetComponentInParent<MyPlayer>().isDead)
            {
                GetComponentInParent<NavMeshAgentBrain>().stop();
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
        
            if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            print("Small Exit");

            if (other.gameObject.tag == "Player")
            {
                if (!other.GetComponentInParent<MyPlayer>().isDead )
                {
                    NavMeshAgentBrain navMeshAgentBrain = GetComponentInParent<NavMeshAgentBrain>();
                    navMeshAgentBrain.playerInSmall = false;

                    
                    if (!navMeshAgentBrain.getObjectName().Contains("Titan") )
                    {
                        navMeshAgentBrain.Attacking = false;
                    }
                    //GetComponentInParent<NavMeshAgentBrain>().Attacking = false;
                    //GetComponentInParent<NavMeshAgentBrain>().Point = getClosestTarget();
                    //GetComponentInParent<NavMeshAgentBrain>().move();

                }
                else
                {
                    GetComponentInParent<NavMeshAgentBrain>().Attacking = false;
                    GetComponentInParent<NavMeshAgentBrain>().stop();
                }
            }
        }
        
    }

    private GameObject getClosestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 1)
        {
            return players[0];
        }

        GameObject closest = players.OrderBy(go => (transform.position - go.transform.position).sqrMagnitude).First();
        return closest;
    }
}
