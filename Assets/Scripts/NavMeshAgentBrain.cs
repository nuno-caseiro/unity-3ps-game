using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class NavMeshAgentBrain : MonoBehaviourPun
{
    public bool ShouldIMove;
    public bool Attacking;
    public GameObject Point;
    public Animator animator;
    public NavMeshAgent navMeshAgent;


    public bool death = false;
    public float health = 1.00f;
    public float damage = 0.01f;
    public float points = 10;
    

    public string lastHitPlayer;

    //for shooter zombie
    private float bulletSpeed = 100f;
    public GameObject shootPoint;
    public GameObject bulletPrefab;

    //for titan
    public bool playerInSmall = false;
    

      // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
            {
               /* if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Z_FallingBack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    GetComponent<PhotonView>().RPC("DestroyZombie", RpcTarget.All);

                }*/

                if (ShouldIMove && !death)
                {
                    if (Point.transform.gameObject.activeSelf)
                    {

                        GetComponent<NavMeshAgent>().SetDestination(Point.transform.position);
                    }
                    else
                    { 
                        stop();
                    }

                }

            }
        }

     
        
     
    }

   

    [PunRPC]
    public void ReBirth(float x , float y, float z)
    {
        transform.position = new Vector3(x,y,z);

        //HEALTH ON REBIRTH
        string name = transform.gameObject.name;
        if (name.Contains("Zombie"))
        {
            health = 3;
        }
        if (name.Contains("alien")){
            health = 5;
        }
        if(name.Contains("Titan"))
        {
            health = 20;
        }

        health = 1;
        death = false;
        ShouldIMove = false;
        transform.gameObject.SetActive(true);
    }

    public void move()
    {
        
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }
            animator.SetBool("run", true);
            animator.SetBool("attack", false);

     
            ShouldIMove = true;

            navMeshAgent.isStopped = false;
            Attacking = false;
        
     
        
    }

    public void stop()
    {
            
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }
            animator.SetBool("run", false);
            animator.SetBool("attack", false);

            ShouldIMove = false;
            navMeshAgent.isStopped = true;
            //navMeshAgent.velocity = Vector3.zero;
            Attacking = false;
        
    }


    [PunRPC]
    public void GetDamageZombie(float amount, string username)
    {
        if (!death)
            {
                health -= amount;
              
                if (health <= 0)
                {
                    GetPlayer(username);
                    Death();
            }
            else if(transform.gameObject.name.Contains("Titan")) {
                animator.SetTrigger("hit");
            }
        }


        /*if (photonView.IsMine)
        {
            fillImage.fillAmount = playerHealth;
        }*/
    }

    public void Death()
    {
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;
        ShouldIMove = false;
        death = true;
        animator.SetBool("run", false);
        animator.SetBool("attack", false);
        animator.SetTrigger("dead");

    }


    //[PunRPC]
    public void DestroyZombie()
    {
        ShouldIMove = false;
        GetComponent<PhotonView>().transform.gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("PoolManager").GetComponent<PoolManager>().AddZombie(transform.gameObject);
        }
           
    }

    
    void GetPlayer(string namePlayer)
    {
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerParent");
        
        foreach (GameObject player in players)
        {
                      
           // player.GetComponent<MyPlayer>().teamPoints += this.points;
            if (player.GetComponent<PhotonView>().Owner.NickName == namePlayer)
            { 
                player.GetComponent<MyPlayer>().points += this.points;
                player.GetComponent<PhotonView>().RPC("refreshPoints", RpcTarget.All);
                return;
            }
        }

        //refresh all player points
    }


    public void DamagePlayer()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().running)
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                Point.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.All, damage, photonView.ViewID);
            }
            
        }
        
    }

    public void ReachTarget()
    {
        
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }
            transform.LookAt(Point.transform);
            ShouldIMove = false;
            navMeshAgent.isStopped = true;
            //navMeshAgent.velocity = Vector3.zero;         
            animator.SetBool("run", false);
            animator.SetBool("attack", true);
            Attacking = true;
        
        
     
    }


    public void CastBullet()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (transform.gameObject.name.Contains("alien"))
            {
                
                PoolManager pool = GameObject.Find("PoolManager").GetComponent<PoolManager>();
                GameObject bullet = pool.GetBullet();
              
                Vector3 dir = (Point.transform.parent.transform.position - shootPoint.transform.position).normalized;
                //print(Point.transform.parent.transform.position - shootPoint.transform.position);

                shootPoint.transform.LookAt(dir);
                if (bullet != null)
                {
                    
                    bullet.transform.position = shootPoint.transform.position;
                    //bullet.transform.rotation = shootPoint.transform.rotation;
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.transform.rotation = Quaternion.FromToRotation(bullet.transform.up, dir);
                    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * bulletSpeed;
                    bullet.GetComponent<PhotonView>().RPC("EnableBullet", RpcTarget.All, photonView.ViewID);
                }
                else
                {
                    print("NEW BULLET");
                    bullet = PhotonNetwork.Instantiate(bulletPrefab.name, shootPoint.transform.position, Quaternion.identity);
                    bullet.transform.rotation =Quaternion.FromToRotation(bullet.transform.up, dir);
                    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * bulletSpeed;
                    bullet.GetComponent<BulletController>().viewId = photonView.ViewID;
                }
               

            }
        }
    }

    [PunRPC]
    public void StopZombieFromPlayer()
    {
        stop();
    }

    public void Look()
    {
        if (Point != null)
        {
           
           transform.LookAt(Point.transform.parent.transform);
            if (shootPoint != null)
            {
                shootPoint.transform.LookAt(Point.transform.parent.transform);
            }
           
        }
        
    }



}
