﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [SerializeField] private int m_damage = 10;
    [SerializeField] private float m_Size = 1.0f;
    [SerializeField] private float m_Speed = 1.0f;
    [SerializeField] private int m_type = -1;

    //2/25/2018 Switch heat setting to weapon
    [SerializeField] private float m_heat = 30.0f;
    [SerializeField] private float m_heatCooldownRate = 20.0f;
    //2/25/2018 Switch heat setting to weapon

    public int m_id = 1;

    private PhotonView m_pv;

	void Start () {
        m_pv = GetComponent<PhotonView>();
        /*if (m_type == -1)
            Debug.Log(this.name + " was set to the wrong weapon type");*/
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

    [PunRPC]
    public void SetScale()
    {
        this.gameObject.transform.localScale = this.gameObject.transform.localScale * m_Size;
    }

    [PunRPC]
    public void AutoDestroy()
    {
        Destroy(gameObject, 2.0f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player" && m_pv.isMine)
        {
            other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, m_damage);
            m_damage = 0;
        }
    }

    public float ReturnSpeed()
    {
        return m_Speed;
    }

    public float ReturnScale()
    {
        return m_Size;
    }

    //2/25/2018 Switch heat setting to weapon
    public float ReturnHeat()
    {
        return m_heat;
    }

    public float ReturnHeatCoolDownRate()
    {
        return m_heatCooldownRate;
    }
    //2/25/2018 Switch heat setting to weapon



}
