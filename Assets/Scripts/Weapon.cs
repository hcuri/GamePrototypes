using System.Collections;
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

    //Use this ID to see who shoot this weapon, -1 means unset
    public int m_id = -1;

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

    //[PunRPC]
    public void SetID(int shooterID)
    {
        m_id = shooterID;
    }

    [PunRPC]
    public void AutoDestroy()
    {
        //this setting set the life time of a weapon
        //Destroy(gameObject, 2.0f);
        //I set it to 10, just for testing
        Destroy(gameObject, 10.0f);
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

    public void SetSpeed()
    {
        m_Speed = m_Speed * (1.2f);
    }

    public void SetHeat()
    {
        m_heat = m_heat * (0.8f);
    }

    public void SetSize()
    {
        m_Size = m_Size * (1.1f);
    }

    public void SetDamage()
    {
        //Debug.Log("Power Up the Weapon now!!!");
        m_damage = m_damage + 5;
    }

}
