using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                GetComponentInParent<NavMeshAgentBrain>().ReachTarget();
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
                GetComponentInParent<NavMeshAgentBrain>().Point = other.gameObject;
                GetComponentInParent<NavMeshAgentBrain>().move();
            }
        }
        
    }
}
