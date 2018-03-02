using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : Photon.MonoBehaviour {

    public float timeToRespawn = 2.0f;
    public GameObject weapon;
    private bool isAvailable = true;
    private float timer;
    private PhotonView pv;
    [SerializeField] int m_type;
    public Mesh[] meshField;
    bool isSet = false;
    int randNum;

    void Start()
    {
        if (meshField == null)
            Debug.Log("You need to assign differet meshes to the power up so that it can change");
        timer = timeToRespawn;
        this.tag = "WeaponContainer";
        //Debug.Log("Hi I am Master");
        randNum = (int)Random.Range(0, 5);
        Debug.Log("random is :" + randNum);
        //this.gameObject.GetComponent<PhotonView>().RPC("SetType", PhotonTargets.AllBuffered, randNum);
        //Debug.Log("Am I master?");
        //Debug.Log("the type of power up is " + m_type);
        //Mesh m_mesh = GetComponent<MeshFilter>().sharedMesh;
        // GetComponent<MeshFilter>().sharedMesh = meshField[m_type];

    }
    /*public override void OnCreatedRoom()
    {
        m_type = (int)Random.Range(0, 5);
        Debug.Log("the type of power up is " + m_type);
    }*/
    [PunRPC]
    public void SetType(int t_type)
    {
        Debug.Log("t_type: " +  t_type);
        m_type = t_type;
        isSet = true;
    }

    void Update()
    {
        if (!isSet)
        {
            this.gameObject.GetComponent<PhotonView>().RPC("SetType", PhotonTargets.AllBuffered, randNum);
        }
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
                /*GameObject playerWeapon = PhotonNetwork.Instantiate(weapon.name.ToString(), other.transform.Find("PlayerHand").transform.position, Quaternion.identity, 0); 
                playerWeapon.GetComponent<PhotonView>().RPC("SetParentRPC", PhotonTargets.AllBuffered, other.gameObject.GetComponent<PhotonView>().viewID);
                playerWeapon.GetComponent<PhotonView>().RPC("SetScale", PhotonTargets.AllBuffered);*/
                other.GetComponent<PhotonView>().RPC("GrabWeapon", PhotonTargets.AllBuffered, m_type);
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
