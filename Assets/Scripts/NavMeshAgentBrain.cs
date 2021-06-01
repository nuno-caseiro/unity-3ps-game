using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.UI;
public class NavMeshAgentBrain : MonoBehaviourPun
{
    public bool ShouldIMove;
    public bool Attacking;
    public GameObject Point;
    public Animator animator;
    public NavMeshAgent navMeshAgent;


    public bool death = false;
    public float health = 1.00f;
    private float damage = 0.00f;
    public float points = 10;


    public string lastHitPlayer;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Z_FallingBack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                GetComponent<PhotonView>().RPC("DestroyZombie", RpcTarget.MasterClient);

            }

            if (ShouldIMove && !death)
            {

                GetComponent<NavMeshAgent>().SetDestination(Point.transform.position);
            }

        }

     
    }

    public void move()
    {
        animator.SetBool("run", true);
        animator.SetBool("attack", false);
        ShouldIMove = true;
        navMeshAgent.isStopped = false;
        Attacking = false;
    }


    public void stop()
    {
        animator.SetBool("run", false);
        animator.SetBool("attack", false);
        ShouldIMove = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        Attacking = false;
    }


    [PunRPC]
    public void GetDamageZombie(float amount, int lastHitPlayer)
    {
        if (!death)
        {
            health -= amount;


            if (health <= 0)
            {
                GetPlayer(lastHitPlayer);
                Death();
            }
        }
       

        /*if (photonView.IsMine)
        {
            fillImage.fillAmount = playerHealth;
        }*/
    }


    public void Death()
    {
        navMeshAgent.isStopped = true;
        death = true;
        animator.SetBool("run", false);
        animator.SetBool("attack", false);
        animator.SetTrigger("dead");

    }


    [PunRPC]
    public void DestroyZombie()
    {
        transform.gameObject.SetActive(false);
        GameObject.Find("GameManager").GetComponent<GameManager>().zombieList.Add(transform.gameObject);   
    }

    GameObject GetPlayer(int idObject)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<MyPlayer>().teamPoints += this.points;
            if (player.GetInstanceID() == idObject)
            {
                player.GetComponent<MyPlayer>().points += this.points;
            }
        }
        return null;
    }


    public void DamagePlayer()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            Point.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, damage);
        }
        
    }

    public void ReachTarget()
    {
        
        
        transform.LookAt(Point.transform);
        ShouldIMove = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetBool("run", false);
        animator.SetBool("attack", true);
        Attacking = true;
    }



}
