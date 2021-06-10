using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class NavMeshAgentSmallTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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
            if (!other.GetComponentInParent<MyPlayer>().isDead && other.gameObject.tag == "Player" )
            {
                GetComponentInParent<NavMeshAgentBrain>().Point = getClosestTarget();
                GetComponentInParent<NavMeshAgentBrain>().ReachTarget();
                //GetComponentInParent<PhotonView>().RPC("ReachTarget", RpcTarget.All);
            }
        }
    }

   /* public void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<MyPlayer>().isDead)
        {
            print("SMALL TRIGGER DEAD?");
            GetComponentInParent<NavMeshAgentBrain>().stop();

        }
    }*/


    private void OnTriggerExit(Collider other)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            print("Small Exit");
            if (!other.GetComponentInParent<MyPlayer>().isDead && other.gameObject.tag == "Player")
            {
                GetComponentInParent<NavMeshAgentBrain>().Point = getClosestTarget();
                //GetComponentInParent<PhotonView>().RPC("move", RpcTarget.All);
                GetComponentInParent<NavMeshAgentBrain>().move();
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
