using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class MyPlayer : MonoBehaviourPun//, IPunObservable
{
    public float moveSpeed = 3f;
    public float smoothRotationTime = 0.12f;
    public bool enableMobileInputs = false;

    private float currentVelocity;
    private float currentSpeed;


    private float speedVelocity;
    //private float jumpForce = 5f;

    public Transform cameraTransform;

    private FixedJoystick joystick;
    //private GameObject crossHairPrefab;
    //private Vector3 crossHairVel;
    
    private bool fire = false;

    public GameObject chatSystem;

    //sounds
    public AudioSource shootSound;
    public AudioSource runSound;

    //health
    public GameObject healthBar;
    public Image fillImage;

    private float playerHealth = 1f;
    private float damage = 0.1f;

    private ParticleSystem muzzle;

    private Animator myAnimator;
    public Transform rayOrigin;

    public float currentLerpTime = 0f;

    [HideInInspector]
    public bool isDead = false;

    public GameObject pointsParent;

    public Text pointsText;
    public float points = 0;

    public Text teamPointsText;
    public float teamPoints = 0;


    Rigidbody rb;
    public float fallMultiplayer = 2.5f;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

            //crossHairPrefab = Resources.Load("CrosshairCanvas") as GameObject;
            chatSystem.SetActive(true);
        }
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            myAnimator = GetComponent<Animator>();
            cameraTransform = Camera.main.transform;

            GameObject.Find("ShootButton").GetComponent<BtnFireScript>().SetPlayer(this);
            GameObject.Find("JumpButton").GetComponent<FixedButton>().SetPlayer(this);
            //crossHairPrefab = Instantiate(crossHairPrefab);
            healthBar.SetActive(true);
            pointsParent.SetActive(true);
        }
        else
        {
            GetComponent<BetterJump>().enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (!isDead)
            {

            LocalPlayerUpdate();
            }
        }

    }
   
    void LocalPlayerUpdate()
    {
        
        float horizontalMove = joystick.Horizontal;
        float verticalMove = joystick.Vertical;

        Vector2 moveVector = new Vector2(horizontalMove, verticalMove).normalized;
        

        if (moveVector != Vector2.zero)
        {
            float rotation = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref currentVelocity, smoothRotationTime);

            if (!runSound.isPlaying)
                runSound.Play();

        }
        else
        {
            //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraTransform.eulerAngles.y, ref currentVelocity, smoothRotationTime);
            runSound.Stop();
        }

        if (fire)
        {
            float rotation = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * rotation;
        }

        float targetSpeed = moveSpeed * moveVector.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, 0.02f);

        if (moveVector.magnitude > 0)
        {
            myAnimator.SetBool("running", true);
        }
        else if (moveVector.magnitude == 0)
        {
            myAnimator.SetBool("running", false);
        }

        pointsText.text = points.ToString();
        teamPointsText.text = teamPoints.ToString();


        if (!fire)
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

    }

    public void MuzzleFlash()
    {
        if (muzzle == null)
            muzzle = GameObject.Find("SciFiRifle(Clone)/GunMuzzle").GetComponent<ParticleSystem>();
        if(!muzzle.isPlaying)
            muzzle.Play();
    }

    public void Fire()
    {
        fire = true;

        myAnimator.SetBool("fire 0", true);
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        transform.LookAt(Camera.main.transform);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {

            PhotonView pv = hit.transform.GetComponent<PhotonView>();
            print(hit.transform.gameObject.name);

          /*  if (pv != null && !hit.transform.GetComponent<PhotonView>().IsMine && hit.transform.tag == "Player" )
            {
                hit.transform.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, damage);
            }*/


            if (hit.transform.tag == "Zombie")
            { 
                hit.transform.GetComponent<PhotonView>().RPC("GetDamageZombie", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.NickName);
            }


        }

        if (!shootSound.isPlaying)
        {
            shootSound.loop = true;
            shootSound.Play();
        }
       
        MuzzleFlash();
        //Debug.DrawRay(rayOrigin.position, Camera.main.transform.forward * 25f, Color.green);
    }

    public void FireUp()
    {
        fire = false;
        if (photonView.IsMine)
            myAnimator.SetBool("fire 0", false);

        shootSound.loop = false;
        shootSound.Stop();

        if (muzzle == null)
            muzzle = GameObject.Find("SciFiRifle(Clone)/GunMuzzle").GetComponent<ParticleSystem>();
        muzzle.Stop();
      
    }

    public void Jump()
    {
        myAnimator.SetTrigger("jump");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

    }

    [PunRPC]
    public void GetDamage(float amount, int zombieId)
    {
        print("AMOUNT ZOMBIE ON PLAYER:" + amount);
        playerHealth -= amount;

        if(playerHealth <=0 && photonView.IsMine)
        {

            Death();
            GetZombie(zombieId);
        }

        if (photonView.IsMine)
        {
            fillImage.fillAmount = playerHealth;
        }
    }



    [PunRPC] 
    void HideplayerMesh()
    {
        isDead = true;
        transform.Find("Soldier").gameObject.SetActive(false);
        transform.Find("RigAss").gameObject.SetActive(false);
        transform.Find("Rig").gameObject.SetActive(false);
        transform.Find("Sounds").gameObject.SetActive(false);


    }

    void Death()
    {
        if (isDead)
            return;

        cameraTransform.gameObject.SetActive(false);

        myAnimator.SetTrigger("death");
        photonView.RPC("HideplayerMesh", RpcTarget.All);
        GameObject.Find("GameManager").GetComponent<GameManager>().Spectate();

    }

    [PunRPC]
    public void GameOver()
    {
        if (photonView.IsMine)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().ShowWinScreen();
        }
        
    }

    public void GetZombie(int idObject)
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        PhotonView.Find(idObject).RPC("StopZombieFromPlayer", RpcTarget.All);
       
    }


}
