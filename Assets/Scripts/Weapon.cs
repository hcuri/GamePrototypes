using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [SerializeField] private int m_damage = 10;

    private PhotonView m_pv;

	void Start () {
        m_pv = GetComponent<PhotonView>();
	}
	
	void Update () {
		
	}

    [PunRPC]
    public void SetParentRPC(int parent)
    {
        transform.SetParent(PhotonView.Find(parent).transform);
    }

    [PunRPC]
    public void UnsetParentRPC()
    {
        transform.parent = null;
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
