using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusAndAction : MonoBehaviour, IPunObservable {

    [SerializeField]private bool isEquipped;
    public float m_Health;
    public float m_Power;
    public float m_Speed;
    public GameObject m_Weapon;
    public Camera m_Camera;
    public PhotonView m_pv;
	// Use this for initialization
	void Start () {
        isEquipped = false;
        m_Health = 100;
        m_Power = 1000;
        m_Speed = 1;
        //setting up the player tag
        tag = "Player";
        m_Camera = transform.Find("FirstPersonCharacter").GetComponent<Camera>();
        if (m_Camera == null)
        {
            Debug.Log("Where is the player camera?");
        }
        m_pv = GetComponent<PhotonView>();
        if(m_pv == null)
        {
            Debug.Log("The photon view is missing");
        }
	}
	
	// Update is called once per frame
	void Update () {
        MouseHandle();
	}

    public void SetWeapon(GameObject weapon)
    {
        if (isEquipped == false)
        {
            isEquipped = true;
            m_Weapon = weapon;
        }
        else
        {
            Destroy(m_Weapon);
            m_Weapon = weapon;
        }
    }

    
    public bool getEquipped()
    {
        return isEquipped;
    }

    public void ThrowObject()
    {
        if (isEquipped)
        {
            Debug.Log("I throw my weapon");
            isEquipped = false;
            m_Weapon.transform.parent = null;
            Vector3 ThrowVector = m_Camera.transform.forward;
            Rigidbody WeaponRigid = m_Weapon.GetComponent<Rigidbody>();
            WeaponRigid.isKinematic = false;
            Debug.Log("Power I use to thorw" + ThrowVector);
            WeaponRigid.AddForce(ThrowVector * m_Power);
        }
    }

    private void MouseHandle()
    {
        if (m_pv.isMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isEquipped)
                {

                    //Debug.Log("Hey I press left mouse");
                    ThrowObject();
                }
            }
        }
    }

   /* void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(IsFiring);
            stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }*/
}
