using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusAndAction : MonoBehaviour {

    [SerializeField]private bool isEquipped;
    public float m_Health;
    public float m_Power;
    public float m_Speed;
    public GameObject m_Weapon;
    public Camera m_Camera;
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
	}
	
	// Update is called once per frame
	void Update () {
        MouseHandle();
	}

    public void SetWeapon(GameObject weapon)
    {
        isEquipped = true;
        m_Weapon = weapon;
    }

    
    public bool getEquipped()
    {
        return isEquipped;
    }

    public void ThrowObject()
    {
        Debug.Log("I throw my weapon");
        isEquipped = false;
        m_Weapon.transform.parent = null;
        Vector3 ThrowVector = m_Camera.transform.forward;
        Rigidbody WeaponRigid = m_Weapon.GetComponent<Rigidbody>();
        WeaponRigid.isKinematic = false;
        Debug.Log("Power I use to thorw" + ThrowVector);
        WeaponRigid.AddForce(ThrowVector*m_Power);
    }

    private void MouseHandle()
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
