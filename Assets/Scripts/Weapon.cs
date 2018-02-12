using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [SerializeField] private int m_damage = 10;

    private PhotonView m_pv;
    public GameObject playerHand;

	void Start () {
        m_pv = GetComponent<PhotonView>();
	}
	
	void Update () {
		if(playerHand != null)
        {
            transform.position = playerHand.transform.position;
        }
	}

    [PunRPC]
    public void SetParentRPC(int parent)
    {
        //transform.SetParent(PhotonView.Find(parent).transform.Find("FirstPersonCharacter").transform.Find("PlayerArm").transform.Find("PlayerHand").transform);
        Debug.Log(PhotonView.Find(parent).transform);
        playerHand = PhotonView.Find(parent).transform.Find("FirstPersonCharacter").transform.Find("PlayerArm").transform.Find("PlayerHand").gameObject;
        transform.SetParent(PhotonView.Find(parent).transform);
        //transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    [PunRPC]
    public void UnsetParentRPC()
    {
        transform.parent = null;
        playerHand = null;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player" && m_pv.isMine)
        {
            other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, m_damage);
            m_damage = 0;
        }
    }
}
