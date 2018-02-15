using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerNetwork : MonoBehaviour {

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private MonoBehaviour[] playerControlScripts;   
    [SerializeField] private float m_health = 100;
    [SerializeField] private float m_throwforce = 1500.0f;
    [SerializeField] private float m_jumpforce = 1500.0f;

    private Text m_healthText;
	private Slider m_healthSlider;
    private PhotonView m_pv;
    private MonoBehaviour m_myPlayerControlScript;
    private GameObject weapon;
    private bool insideZone = true;
    private float m_HPReducedPerSecond = 15.0f;

    private void Start ()
    {
        m_pv = GetComponent<PhotonView>();
        m_healthText = GameObject.Find("HP").GetComponent<Text>();
		m_healthSlider = GameObject.Find ("HPSlider").GetComponent<Slider> ();
        Initialize();
	}

    private void Update()
    {
        if(m_pv.isMine)
        {
            Movement();
            m_healthText.text = "HP:" + m_health.ToString();
            m_healthSlider.value = m_health;
            if (!insideZone)
            {
                TakeDamage(Time.deltaTime * m_HPReducedPerSecond);
            }
            return;
        }
    }

    private void Initialize()
    {
        if (m_pv.isMine)
        {
            m_myPlayerControlScript = playerControlScripts[0];
        }

        //Handle things for non-local character
        else
        {
            //Disable the camera
            playerCamera.SetActive(false);

            //Disable the control scripts
            foreach (MonoBehaviour m in playerControlScripts)
            {
                m.enabled = false;
            }
        }
    }

    private void Movement()
    {
        //Attack(Throw) movement
        if (Input.GetMouseButtonDown(0))
        {
            //Searching the child weapons
            for (int i = 0; i < transform.childCount; ++i)
            {
                if (transform.GetChild(i).CompareTag("Weapon"))
                    weapon = transform.GetChild(i).gameObject;
            }

            if(weapon == null)
            {
                Debug.Log("You have no weapon");
                return;
            }

            weapon.GetComponent<PhotonView>().RPC("UnsetParentRPC", PhotonTargets.AllBuffered);
            weapon.GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward * m_throwforce);
            weapon.GetComponent<PhotonView>().RPC("AutoDestroy", PhotonTargets.AllBuffered);
            weapon = null;          
        }

        if(Input.GetMouseButtonDown(1))
        {
            
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
		m_health -= damage;

        if(m_health <= 0)
        {
            //Die
            //m_myPlayerControlScript.enabled = false;
            Debug.Log("You are died");
			PhotonNetwork.Disconnect();
			Cursor.lockState = CursorLockMode.None;
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
			SceneManager.LoadScene("EndScene");
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    public void setInsideZone(bool inside)
    {
        insideZone = inside;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*
        if(stream.isWriting)
        {
            stream.SendNext(m_health);
        }

        else if(stream.isReading)
        {
            m_health = (int)stream.ReceiveNext();
        }
        */
    }
    
	

}
