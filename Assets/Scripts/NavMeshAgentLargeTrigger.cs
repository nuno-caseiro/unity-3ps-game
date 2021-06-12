using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
public class NavMeshAgentLargeTrigger : MonoBehaviourPun
{
    private NavMeshAgentBrain navMeshAgentBrain;
    private void Awake()
    {
        navMeshAgentBrain = GetComponentInParent<NavMeshAgentBrain>();
    }


    private void OnTriggerExit(Collider other)
    {

        navMeshAgentBrain.stop();
            //GetComponentInParent<PhotonView>().RPC("stop", RpcTarget.All);
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
            if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
            {
                if (!other.GetComponentInParent<MyPlayer>().isDead && other.gameObject.tag == "Player")
                {

                navMeshAgentBrain.Point = getClosestTarget();
                navMeshAgentBrain.move();
                    //GetComponentInParent<PhotonView>().RPC("move", RpcTarget.All);
                }
            }
        
    }

    private void OnTriggerStay(Collider other)
    {
        
            if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
            {
                if (!other.GetComponentInParent<MyPlayer>().isDead && other.gameObject.tag == "Player")
                {
                    if (!navMeshAgentBrain.ShouldIMove && !navMeshAgentBrain.Attacking)
                    {
                    navMeshAgentBrain.Point = getClosestTarget();
                    navMeshAgentBrain.move();
                        //GetComponentInParent<PhotonView>().RPC("move", RpcTarget.All);


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
