using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
public class NavMeshAgentLargeTrigger : MonoBehaviourPun
{


    private void OnTriggerExit(Collider other)
    {
        GetComponentInParent<NavMeshAgentBrain>().stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            if (!other.GetComponentInParent<MyPlayer>().isDead && other.gameObject.tag == "Player")
            {
                
                GetComponentInParent<NavMeshAgentBrain>().Point = getClosestTarget();
                GetComponentInParent<NavMeshAgentBrain>().move();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            if (!other.GetComponentInParent<MyPlayer>().isDead && other.gameObject.tag == "Player")
            {
                if (!GetComponentInParent<NavMeshAgentBrain>().ShouldIMove && !GetComponentInParent<NavMeshAgentBrain>().Attacking)
                {
                    GetComponentInParent<NavMeshAgentBrain>().Point = getClosestTarget();
                    GetComponentInParent<NavMeshAgentBrain>().move();

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
