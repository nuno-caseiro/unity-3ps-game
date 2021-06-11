using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public Queue<GameObject> zombieList = new Queue<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void AddZombie(GameObject z)
    {
        zombieList.Enqueue(z);
    }

    public GameObject GetZombie()
    {
        if (zombieList.Count > 0)
        {
            return zombieList.Dequeue();

        }
        return null;
    }

}
