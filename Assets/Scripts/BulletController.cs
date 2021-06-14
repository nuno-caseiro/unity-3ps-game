using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviourPun
{

    private float bulletDamage = 0.001f;
    public int viewId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        StartCoroutine(LateCall());
    }



    public void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (gameManager.running)
            {
                if (other.gameObject.tag == "Player")
                {
                    if (other.gameObject.GetComponentInParent<PhotonView>().IsMine)
                    {
                        other.gameObject.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.All, bulletDamage, viewId); //photonview.viewid
                        GetComponent<PhotonView>().RPC("DisableBullet", RpcTarget.All);
                        GameObject.Find("PoolManager").GetComponent<PoolManager>().AddBullet(transform.gameObject);
                        //DIZER A tds para meter desativo e neste caso meto na lista 
                    }

                }
            }
        }
    }

    [PunRPC]
    public void DisableBullet()
    {
        transform.gameObject.SetActive(false);

    }
        
    [PunRPC]
    public void EnableBullet(int viewId)
    {
        this.viewId = viewId;
        transform.gameObject.SetActive(true);
    }

    IEnumerator LateCall()
    {
        yield return new WaitForSeconds(2);
        print("COROUTINE");
        GetComponent<PhotonView>().RPC("DisableBullet", RpcTarget.All);
        GameObject.Find("PoolManager").GetComponent<PoolManager>().AddBullet(transform.gameObject);
    }

}
