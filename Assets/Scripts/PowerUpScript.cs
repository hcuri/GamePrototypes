using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : Photon.MonoBehaviour {

    public float timeToRespawn = 2.0f;
    public bool isRespawnable = true;
    public bool isRandomizable = true;
    private bool isAvailable = true;
    private float timer;
    private PhotonView pv;
    [SerializeField] int m_type = 0; //0 is health up, 1 is player speed up, 2 is weapon power up, 3 is weapon speed up
    //These Three are the table for Meshes, Material for
    public Mesh[] meshField;
    public Color[] colorField;
    bool isSet = false;
    int randNum;
    private GameObject childObject;

    public GameFlowManager m_GFM;
    public float[] earlyPos;
    public float[] earlyPre;
    public float[] latePos;
    public float[] latePre;
    public int temp;

    void Start()
    {
        if (meshField == null)
            Debug.Log("You need to assign differet meshes to the power up so that it can change");
        timer = timeToRespawn;
        this.tag = "PowerUp";
        /*randNum = (int)Random.Range(0, 5);
        Debug.Log("random is :" + randNum);*/
        childObject = transform.GetChild(0).gameObject;
		colorField = new Color[5];
		colorField [0] = new Color (255/255f,0/255f,164/255f,100/255f);
		colorField [1] = new Color (255/255f, 227/255f, 0/255f, 100/255f);
		colorField [2] = new Color (0/255f, 123/255f, 255/255f, 100/255f);
        colorField[4] = new Color(75 / 255f, 75 / 255f, 75 / 255f, 100 / 255f);
		colorField [3] = new Color (100/255f, 1000/255f, 100/255f, 100/255f);

        m_GFM = GameObject.Find("GameFlowManager").GetComponent<GameFlowManager>();

        earlyPos = new float[] { 5, 35, 35, 10, 10 ,5};
        earlyPre = new float[earlyPos.Length];
        earlyPre[0] = earlyPos[0];
        for(int i = 1; i < earlyPre.Length; i++)
        {
            earlyPre[i] = earlyPre[i - 1] + earlyPos[i];
        }
        latePos = new float[] { 20, 10, 10, 20, 20, 20 };
        latePre = new float[latePos.Length];
        latePre[0] = latePos[0];
        for (int i = 1; i < latePre.Length; i++)
        {
            latePre[i] = latePre[i - 1] + latePos[i];
        }

        randNum = GetMyPosRand();
    }

    [PunRPC]
    public void SetType(int t_type)
    {
        //Debug.Log("t_type: " +  t_type);

        t_type = t_type >= 5 ? 0 : t_type;

        m_type = t_type;
        childObject.GetComponent<MeshFilter>().sharedMesh = meshField[m_type];
        //childObject.GetComponent<MeshRenderer>().material = materialField[m_type];
        childObject.GetComponent<MeshRenderer>().material.color = colorField[m_type];
        GetComponent<MeshRenderer>().material.color = colorField[m_type];
        isSet = true;
    }

    void Update()
    {
        //Notice that, somehow when I tried to called RPC in start it doesn't work, so I called this at the first loop of update
        if (!isSet)
        {
            this.gameObject.GetComponent<PhotonView>().RPC("SetType", PhotonTargets.AllBuffered, isRandomizable ? randNum : m_type);
        }
        if (isRespawnable)
        {
            timer -= Time.deltaTime;
            if (!isAvailable && timer < 0)
            {
                isAvailable = true;
                GetComponent<MeshRenderer>().enabled = true;
                childObject.GetComponent<MeshRenderer>().enabled = true;
                GetComponent<Collider>().enabled = true;
                if (m_GFM.m_Stage == GameFlowManager.StageType.InitialStage)
                    randNum = GetMyPosRand();
                else
                    randNum = GetMyLateRand();
                if (PhotonNetwork.isMasterClient)
                {
                    this.gameObject.GetComponent<PhotonView>().RPC("SetType", PhotonTargets.AllBuffered, isRandomizable ? randNum : m_type);
                }
            }
        }
    }

    
     /* 
     * This function will disable the appearance of power up,
     * you can set isRespawnable to control whether it will respawn
     */

    [PunRPC]
    public void WaitForRespawn()
    {
        //Reset the weapon keeper 
        if (isRespawnable)
        {
            timer = timeToRespawn;
        }
        isAvailable = false;
        GetComponent<MeshRenderer>().enabled = false;
        childObject.GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        pv = other.gameObject.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && isAvailable)
        {
            if (pv.isMine)
            {
                //it will send the type number to player, and player should handle it.
                other.GetComponent<PhotonView>().RPC("GetPowerUp", PhotonTargets.AllBuffered, m_type);
            }

            if(!isRespawnable)
            {
                PhotonNetwork.Destroy(gameObject);
            }

            GetComponent<PhotonView>().RPC("WaitForRespawn", PhotonTargets.AllBuffered);
        }
    }

    public int GetMyPosRand()
    {
        Debug.Log("EarlyRand");
        int randNum = Random.Range(0, 100);
        int rightS = earlyPre.Length;
        int leftS = 0;
        int mid;
        while(leftS < rightS)
        {
            mid = (rightS + leftS) / 2;
            if (randNum > earlyPre[mid])
                leftS = mid + 1;
            else
                rightS = mid;
        }
        return leftS;
    }

    public int GetMyLateRand()
    {
        Debug.Log("LateRand");
        int randNum = Random.Range(0, 100);
        int rightS = latePre.Length;
        int leftS = 0;
        int mid;
        while (leftS < rightS)
        {
            mid = (rightS + leftS) / 2;
            if (randNum > latePre[mid])
                leftS = mid + 1;
            else
                rightS = mid;
        }
        return leftS;
    }
}
