using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyCamera : MonoBehaviour
{
    public PhotonView playerPhotonView;
    public float Yaxis;
    public float Xaxis;
    public float RotationSensitivity = 0.2f;
    private Transform target;
    public float distanceFromPlayer;

    float RotationMin = -40f;
    float RotationMax = 80;
    float smoothTime = 0.12f;
    
    Vector3 targetRotation;
    Vector3 currentVel;
    public bool enableMobileInputs = false;
    private FixedTouchField touchField;

    private void Awake()
    {
        if (playerPhotonView.IsMine)
        {
            transform.parent = null;
            touchField = GameObject.Find("TouchPanel").GetComponent<FixedTouchField>();
            target = GetLocalPlayer().transform.GetChild(3);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        

    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (!playerPhotonView.IsMine)
            {
                return;
            }

            Yaxis += touchField.TouchDist.x * RotationSensitivity;
            Xaxis -= touchField.TouchDist.y * RotationSensitivity;

            Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

            targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
            transform.eulerAngles = targetRotation;

            //transform forward - direcao do objeto

            Vector3 _offset = target.position - transform.forward * distanceFromPlayer;
            _offset.y = target.position.y + 3f;

            transform.position = _offset;
        }
     
    }

    GameObject GetLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
       
        foreach(GameObject player in players)
        {
            if (player.GetComponentInParent<PhotonView>().IsMine)
            { 
                return player.transform.parent.gameObject;
            }
        }
        return null;
    }
}
