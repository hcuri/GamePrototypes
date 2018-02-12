using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetwork : MonoBehaviour {

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private MonoBehaviour[] playerControlScripts;   
    [SerializeField] private int m_health = 100;
    [SerializeField] private float m_throwforce = 1500.0f;
    [SerializeField] private float m_jumpforce = 1500.0f;

    private Text m_healthText;
    private PhotonView m_pv;
    private MonoBehaviour m_myPlayerControlScript;
    private GameObject weapon;

    private void Start ()
    {
        m_pv = GetComponent<PhotonView>();
        m_healthText = GameObject.Find("HP").GetComponent<Text>();
        Initialize();
	}

    private void Update()
    {
        if(m_pv.isMine)
        {
            Movement();
            m_healthText.text = "HP:" + m_health.ToString();
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
            /*for (int i = 0; i < transform.childCount; ++i)
            {
                if (transform.GetChild(i).CompareTag("Weapon"))
                    weapon = transform.GetChild(i).gameObject;
            }*/
            //Because now I put player weapon under player hand so we should first get the player hand and find weapon;
            GameObject playerHand = transform.Find("FirstPersonCharacter").transform.Find("PlayerArm").transform.Find("PlayerHand").gameObject;
            foreach(Transform wea in playerHand.transform)
            {
                if (wea.gameObject.CompareTag("Weapon"))
                    weapon = wea.gameObject;
            }
            if (weapon == null)
            {
                Debug.Log("You have no weapon");
                return;
            }

            weapon.GetComponent<PhotonView>().RPC("UnsetParentRPC", PhotonTargets.AllBuffered);
            //use raycast from camera position with camera front to get
            Vector3 throwDirection;
            RaycastHit hit;
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
            {
                Debug.Log(hit.collider.gameObject.name + " the position is " + hit.point);
                throwDirection = hit.point - weapon.transform.position;
            }
            else
            {
                throwDirection = playerCamera.transform.forward;
            }
            throwDirection.Normalize();
            weapon.GetComponent<Rigidbody>().AddForce(throwDirection * m_throwforce);
            weapon = null;
        }

        if(Input.GetMouseButtonDown(1))
        {
            
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        m_health -= damage;

        if(m_health <= 0)
        {
            //Die
            m_myPlayerControlScript.enabled = false;
            Debug.Log("You are died");
        }
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
