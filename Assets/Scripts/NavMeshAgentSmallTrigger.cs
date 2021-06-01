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
            if (!other.GetComponent<MyPlayer>().isDead)
            {
                GetComponentInParent<NavMeshAgentBrain>().ReachTarget();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            print("Small Exit");
            if (!other.GetComponent<MyPlayer>().isDead)
            {
                GetComponentInParent<NavMeshAgentBrain>().Point = other.gameObject;
                GetComponentInParent<NavMeshAgentBrain>().move();
            }
        }
        
    }
}
