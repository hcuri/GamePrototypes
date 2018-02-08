using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContainer : MonoBehaviour
{

    public float timeToRespawn = 2.0f;
    public GameObject weapon;
    private bool isAvailable = true;
    private float timer;
    private PhotonView pv;

    void Start()
    {
        timer = timeToRespawn;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (!isAvailable && timer < 0)
        {
            isAvailable = true;
            GetComponent<MeshRenderer>().enabled = true;
        }
    }

    [PunRPC]
    public void WaitForRespawn()
    {
        //Reset the weapon keeper 
        timer = timeToRespawn;
        isAvailable = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        pv = other.gameObject.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && isAvailable)
        {
            if (pv.isMine)
            {
                GameObject playerWeapon = PhotonNetwork.Instantiate(weapon.name.ToString(), other.transform.Find("PlayerHand").transform.position, Quaternion.identity, 0); 
                playerWeapon.GetComponent<PhotonView>().RPC("SetParentRPC", PhotonTargets.AllBuffered, other.gameObject.GetComponent<PhotonView>().viewID);
            }

            GetComponent<PhotonView>().RPC("WaitForRespawn", PhotonTargets.AllBuffered);
        }
    }
    
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.isWriting)
        //{
        //    stream.SendNext(timer);
        //    stream.SendNext(isAvailable);
        //}
        //else if (stream.isReading)
        //{
        //    timer = (float)stream.ReceiveNext();
        //    isAvailable = (bool)stream.ReceiveNext();
        //}
    }
}