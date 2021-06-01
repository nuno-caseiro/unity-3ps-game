using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.UI;
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
            if (!other.GetComponent<MyPlayer>().isDead)
            {
                print("Small Exit");
                GetComponentInParent<NavMeshAgentBrain>().Point = other.gameObject;
                GetComponentInParent<NavMeshAgentBrain>().move();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            if (!other.GetComponent<MyPlayer>().isDead)
            {
                if (!GetComponentInParent<NavMeshAgentBrain>().ShouldIMove && !GetComponentInParent<NavMeshAgentBrain>().Attacking)
                {
                    GetComponentInParent<NavMeshAgentBrain>().Point = other.gameObject;
                    GetComponentInParent<NavMeshAgentBrain>().move();

                }
            }
        }
    }

}
