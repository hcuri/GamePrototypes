﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerNetwork : Photon.MonoBehaviour {

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private MonoBehaviour[] playerControlScripts;   
    [SerializeField] private float m_health = 100;
    [SerializeField] private float m_throwforce = 1500.0f;
    [SerializeField] private float m_jumpforce = 1500.0f;

    //2/21/2018 ME Cool
    [SerializeField] private float m_thermalClipCapacity = 200.0f;
    [SerializeField] private float m_thermalClip = 0.0f;
     
    [SerializeField] private float m_overheatPenaltyTime = 5.0f;
    //2/21/2018 ME Cool

    //2/21/2018 ME COOL
    private bool overHeated;
    private bool requirePenalty;
    private float m_postOverheatShootingPermit;
    //2/21/2018 ME COOL

    //2/25/2018 Switch heat setting to weapon
    private float m_heat;
    private float m_heatCooldownRate;
    private bool m_debugMode;
    //2/25/2018 Switch heat setting to weapon

    //3/5/2018 Receive the powerup
    public int WeaponDamageEmpoweredCounter;
    public int WeaponSpeedEmpoweredCounter;
    private int PlayerSpeedEmpoweredCounter;
    private int PlayerHealthEmpoweredCounter;

    private bool HPPickedUp;

    //private int m_power = 4; //4 means no powerup received.
    //private bool SizeEmpowered;
    //private bool HeatEmpowered;
    //3/5/2018 Receive the powerup

    private Text m_healthText;
	private Text zoneText;
	private Slider m_healthSlider;
	private Image damageImage;
	private Color damageColor = new Color(1f, 0f, 0f, 0.5f);
	private Color m_color = new Color(0f, 1f, 0f);
	//private Slider m_heatSlider;
	private Text m_heatText;
	private Image m_heatIcon;
	private Slider m_atkSlider;
	//private Text m_atkText;
	private Slider m_spdSlider;
	//private Text m_spdText;
    private PhotonView m_pv;
    private MonoBehaviour m_myPlayerControlScript;
    private GameObject weapon;
    private bool insideZone = true;
    public float m_HPReducedPerSecond = 15.0f;
    private float m_maxHP = 100f;
    private bool largeBalls = false;
    private bool invul = false;

	private Image max_power_bar;
	private Image invuln_bar;
	private Image invis_bar;

    private IEnumerator invulCoroutine;
    private IEnumerator bigBallsCoroutine;
    private IEnumerator inviCoroutine;

    //added by Po
    [SerializeField] GameObject[] m_weapons;
    [SerializeField] bool[] weaponOn;
    [SerializeField] private GameObject m_Hand;
    [SerializeField] int weaponPointer;
    public int player_ID = -1;
    public bool isSet = false;
    [SerializeField] GameObject NetworkManager;
    public int killCount;
    private MeshRenderer m_MR;
    private MeshRenderer[] child_MR;
    public float disappearTimer = 10.0f;
	public float bigBallsTimer = 10.0f;
	public float invulTimer = 10.0f;

    public bool showCursor;
    public bool shootEnable;
    public bool isDead;

    //Bloody effect
    [SerializeField] GameObject[] m_bloodCube;
    [SerializeField] GameObject gameFlowManager;

    //4/18/2018
    public Text overheatedSign;
    //4/18/2018
    public Text nameOnHead;
    public string playerName;
    public Image arrowImage;

    private void Start ()
    {
        m_pv = GetComponent<PhotonView>();
        m_healthText = GameObject.Find("HP").GetComponent<Text>();
		zoneText = GameObject.Find ("ZoneText").GetComponent<Text> ();
		m_healthSlider = GameObject.Find ("HPSlider").GetComponent<Slider> ();
		damageImage = GameObject.Find ("DamageImage").GetComponent<Image> ();
		//m_heatSlider = GameObject.Find ("HeatSlider").GetComponent<Slider> ();
		m_heatText = GameObject.Find ("Heat").GetComponent<Text> ();
		m_heatIcon = GameObject.Find ("Heat Icon").GetComponent<Image> ();

		m_atkSlider = GameObject.Find ("ATKSlider").GetComponent<Slider> ();
		m_spdSlider = GameObject.Find ("SPDSlider").GetComponent<Slider> ();
		//m_atkText = GameObject.Find ("ATK").GetComponent<Text> ();
		//m_spdText = GameObject.Find ("SPD").GetComponent<Text> ();

		max_power_bar = GameObject.Find ("Big Balls Bar").GetComponent<Image> ();
		invuln_bar = GameObject.Find ("Invulnerable Bar").GetComponent<Image> ();
		invis_bar = GameObject.Find ("Invisible Bar").GetComponent<Image> ();

        m_debugMode = GameObject.Find("NetworkManager").GetComponent<PhotonNetworkManager>().returnDebugMode();
        NetworkManager = GameObject.Find("NetworkManager");
        m_MR = this.GetComponent<MeshRenderer>();
        child_MR = transform.GetComponentsInChildren<MeshRenderer>();
        nameOnHead = transform.Find("PlayerName").GetComponentInChildren<Text>();

        overheatedSign = transform.Find("overheatImage").GetComponentInChildren<Text>();

        arrowImage = transform.Find("Arrow").GetComponentInChildren<Image>();
        arrowImage.enabled = false;
        overheatedSign.enabled = false;

        Initialize();

        //added by Po
        weaponOn = new bool[] { false, false, false, false, false };
        if(m_weapons.Length < 5)
        {
            Debug.Log("You only attached " + m_weapons.Length + " weapons to player");
        }

        if(m_Hand == null)
        {
            Debug.Log("You forgot to attach hand");
        }

        foreach(GameObject mw in m_weapons)
        {
            mw.SetActive(false);
        }
        weaponPointer = 2;
        m_weapons[weaponPointer].SetActive(true);
        weaponOn[weaponPointer] = true;

        //Reset the kill count at the start
        killCount = 0;

        //start looking green
        SetColor();

        //don't show your name above your own head
        if (m_pv.isMine) nameOnHead.enabled = false;

        //give coroutine references
        bigBallsCoroutine = bigBallsPowerup();
        invulCoroutine = invulPowerup();
        inviCoroutine = ComeBack();

        showCursor = false;
        shootEnable = true;
        isDead = false;

        gameFlowManager = GameObject.Find("GameFlowManager");

        this.GetComponent<PhotonView>().RPC("setMyTag", PhotonTargets.AllBuffered);

        FloatingTextController.Initialize();
    }

    private void Update()
	{
        /*if(!isSet && player_ID != -1)
        {
            //called a RPC to set my ID on every client
        }*/
        
        if (m_pv.isMine)
        {
            //this.GetComponent<PhotonView>().RPC("setMyTag", PhotonTargets.AllBuffered);
            Weapon_Cool_Heat();
            
            if(overHeated)
            {
                
                if (requirePenalty)
                {
                    
                    PenaltyHeatingWeapon();
                }

            }

            if (Time.time > m_postOverheatShootingPermit)
            {
                if(overHeated == true)
                {
                    m_thermalClip = 0.0f;
                }
                overHeated = false;
            }


            if (shootEnable)
            {
                Movement();
            }
            PlayerPowerSort();
            m_healthText.text = "HP:" + m_health.ToString("F0");
			m_healthSlider.value = m_health;
			//m_heatSlider.value = m_thermalClip;
			m_heatIcon.fillAmount = m_thermalClip / m_thermalClipCapacity;
			if (overHeated) {
				m_heatText.text = "OVERHEATED!";
				m_heatIcon.color = new Color (1, 0, 0, 0.5f);
			} else {
				m_heatText.text = "";
				m_heatIcon.color = new Color (1, 0.5f, 0, 0.5f);
			}
			m_atkSlider.value = WeaponDamageEmpoweredCounter;
			m_spdSlider.value = WeaponSpeedEmpoweredCounter;
            /*if (WeaponDamageEmpoweredCounter == 5) {
				m_atkText.text = "MAXIMUM POWER!!!";
			} else {
				m_atkText.text = "Attack Damage: " + WeaponDamageEmpoweredCounter + "/5";
			}
			if (WeaponSpeedEmpoweredCounter == 5) {
				m_spdText.text = "MAXIMUM SPEED!!!";
			} else {
				m_spdText.text = "Projectile Speed: " + WeaponSpeedEmpoweredCounter + "/5";
			}*/
            if (overHeated)
            {
                this.GetComponent<PhotonView>().RPC("showOverHeated", PhotonTargets.AllBuffered);
            }
            else
            {
                this.GetComponent<PhotonView>().RPC("hideOverHeated", PhotonTargets.AllBuffered);
            }
            //this.GetComponent<PhotonView>().RPC("displayOverHeated", PhotonTargets.AllBuffered);
            max_power_bar.fillAmount -= (1.0f/bigBallsTimer) *Time.deltaTime;
            invuln_bar.fillAmount -= (1.0f/invulTimer) *Time.deltaTime;
            invis_bar.fillAmount -= (1.0f/disappearTimer) *Time.deltaTime;
			if (!insideZone) {
                //TakeDamage (Time.deltaTime * m_HPReducedPerSecond, -1);
                this.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, Time.deltaTime * m_HPReducedPerSecond, -1);
				zoneText.text = "You're taking damage inside the zone!";

                if(!arrowImage.enabled && !isDead)
                    arrowImage.enabled = true;

                if(isDead)
                    arrowImage.enabled = false;
                else
                    arrowImage.transform.eulerAngles = new Vector3(0,0,AngleToShrinkingZone());

			} else {
				zoneText.text = "";
                arrowImage.enabled = false;
            }
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, 0.5f*Time.deltaTime);
            if(weaponPointer != -1)
                weaponUpdatePosition();

            // rotate the names towards you
            foreach (PlayerNetwork pn in FindObjectsOfType<PlayerNetwork>())
            {
                pn.transform.Find("PlayerName").transform.forward = playerCamera.transform.forward;
                pn.transform.Find("overheatImage").transform.forward = playerCamera.transform.forward;
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

            if (weapon.GetComponent<Weapon>().m_id == 1)
            {
                //2/25/2018 Switch heat setting to weapon
                /*m_heat = weapon.GetComponent<Weapon>().ReturnHeat();*/

                if (!m_debugMode)
                { 
                m_heatCooldownRate = weapon.GetComponent<Weapon>().ReturnHeatCoolDownRate();
                }
                else
                {
                    m_heatCooldownRate = 1000.0f;
                }
                //2/25/2018 Switch heat setting to weapon
                int newW = weaponPointer + 1;
                string weapName = "Weapon" + newW;
                //float weaponWeight = weapon.GetComponent<Weapon>().ReturnSpeed();
                //Debug.Log("weaponWeight " +weaponWeight);

                if (!overHeated)
                {
                    // camera forward
                    Vector3 camfor = playerCamera.transform.forward;
                    //Vector3 pos = camfor * 2.1f;
                    //Modify By Po, 3/7

                    float toScale = 1.0f;
                    for (int i = 0; i < WeaponDamageEmpoweredCounter; i++) toScale *= 1.5f;

                    if (largeBalls)
                    {
                        toScale = 15.0f;
                    }

                    Vector3 pos = camfor *  2.1f +  camfor * toScale;

                    RaycastHit hit;
                    Physics.Raycast(playerCamera.transform.position, camfor, out hit);

                    if(hit.collider != null)
                    {
                        // list of stuff that we can hit
                        if(hit.distance < toScale * 2.1f && hit.collider.tag == "Untagged")
                        {
                            Debug.Log("You're too close to something to be able to throw");
                            return;
                        }
                    }

                    Vector3 position = playerCamera.transform.position + pos;
                    Vector3 velocity = playerCamera.transform.forward * m_throwforce;
                    //m_pv.RPC("InstantiateWeapon", PhotonTargets.AllBuffered, weapName, position, velocity, toScale);
                    InstantiateWeapon(weapName, position, velocity, toScale);
                }
            }

            else
            {
                weapon.GetComponent<PhotonView>().RPC("UnsetParentRPC", PhotonTargets.AllBuffered);
                weapon.GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward * m_throwforce);
                weapon.GetComponent<PhotonView>().RPC("AutoDestroy", PhotonTargets.AllBuffered);
                weapon = null;
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            
        }
    }

    //[PunRPC]
    public void InstantiateWeapon(String weaponName, Vector3 position, Vector3 velocity, float toScale)
    {
        //GameObject infiniteWeapon = PhotonNetwork.Instantiate(weaponName, position, Quaternion.identity, 0);
        //GameObject infiniteWeapon = GameObject.Instantiate((GameObject)Resources.Load(weaponName), position, Quaternion.identity);
        GameObject infiniteWeapon = PhotonNetwork.Instantiate("Weapon3_Spiky", position, Quaternion.identity, 0);
        //GameObject infiniteWeapon = GameObject.Instantiate((GameObject)Resources.Load("Weapon3_Spiky"), position, Quaternion.identity);
        infiniteWeapon.transform.Rotate(playerCamera.transform.right * 90);
        infiniteWeapon.GetComponent<Rigidbody>().isKinematic = false;

        WeaponPowerSort(infiniteWeapon);
        float weSpeed = infiniteWeapon.GetComponent<Weapon>().ReturnSpeed();
        //Debug.Log(weSpeed);
        infiniteWeapon.GetComponent<Rigidbody>().AddForce(velocity * weSpeed);

        //Debug.Log("Toscale: " + toScale);
        //infiniteWeapon.GetComponent<Transform>().localScale *= toScale;
        //increseWeaponSize(infiniteWeapon, toScale);
        infiniteWeapon.GetComponent<PhotonView>().RPC("setSize", PhotonTargets.AllBuffered, toScale);
        infiniteWeapon.GetComponent<Weapon>().SetID(player_ID);

        //infiniteWeapon.GetComponent<Transform>().localScale *= infiniteWeapon.GetComponent<Weapon>().ReturnScale();
        //infiniteWeapon.GetComponent<PhotonView>().RPC("SetScale", PhotonTargets.AllBuffered);

        infiniteWeapon.GetComponent<PhotonView>().RPC("AutoDestroy", PhotonTargets.AllBuffered);
        m_heat = infiniteWeapon.GetComponent<Weapon>().ReturnHeat();
        //Debug.Log(infiniteWeapon.name + " is heat: " + m_heat);

        if(m_pv.isMine)
            HeatingWeapon();
    }
    
    [PunRPC]
    public void setMyID(int m_ID)
    {
        player_ID = m_ID;
    }

    /*----------------------------------------------------
    Notes for TakeDamage:
    Notice that we input shooterID to TakeDamage, so that
    we can realize the player was killed by whom.
    The ID of Player should start from 0~numOfPlayer-1
    And we set -1 as the ID of shrinking zone
    ----------------------------------------------------*/


    [PunRPC]
    public void TakeDamage(float damage, int shooterID)
    {
        if (invul && damage > 0f) return;

        if (insideZone && damage > 0) {
            foreach (GameObject go in m_bloodCube)
            {
                for (int i = 0; i < 6; i++)
                {
                    GameObject g = Instantiate(go, gameObject.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 2.2f, UnityEngine.Random.Range(-0.5f, 0.5f)), Quaternion.identity);
                    g.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(0, 5f), UnityEngine.Random.Range(-5f, 5f)));
                }
            }
        }


        Debug.Log("I was shot by player" + shooterID);
        
		m_health -= damage;
        if (m_health > m_maxHP) m_health = m_maxHP;
        if(!invul) SetColor();

        if (m_pv.isMine && damage > 0f) {
			damageImage.color = damageColor;
		}

        if (m_health <= 0 && !isDead)
        {
            isDead = true;
            //Debug.Log("my Id is: " + player_ID + "my photon id is: " + m_pv.ownerId);
            if (!m_pv.isMine)
            {
                transform.GetChild(1).GetComponent<Renderer>().enabled = false;
                transform.GetChild(0).GetComponent<Renderer>().enabled = false;
                transform.GetChild(6).GetComponent<Renderer>().enabled = false;

                this.GetComponent<CharacterController>().enabled = false;
            }
            else if (m_pv.isMine)
            {
                //Die
                //Debug.Log("I'm Dying");
                transform.GetChild(1).GetComponent<Renderer>().enabled = false;
                transform.GetChild(0).GetComponent<Renderer>().enabled = false;
                transform.GetChild(6).GetComponent<Renderer>().enabled = false;

                disableMove();
            }
            gameFlowManager.GetComponent<GameFlowManager>().playerDead(gameObject);
            NetworkManager.GetComponent<PhotonNetworkManager>().killWarn(shooterID, player_ID);

            for (int i = 0; i < 5; i++)
            {
                PhotonNetwork.Instantiate("SpreePowerUp", transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 35, UnityEngine.Random.Range(-3f, 3f)), Quaternion.identity, 0);
            }
            GameObject g1 = PhotonNetwork.Instantiate("SpreePowerUp", transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 35, UnityEngine.Random.Range(-3f, 3f)), Quaternion.identity, 0);
            g1.GetComponent<PhotonView>().RPC("SetType", PhotonTargets.AllBuffered, UnityEngine.Random.Range(1, 3));         
        }
    }


    [PunRPC]
    public void TakeDamage(int damage, int shooterID)
    {
        TakeDamage((float)damage, shooterID);
    }

    [PunRPC]
    public void showOverHeated()
    {
        overheatedSign.enabled = true;
    }

    [PunRPC]
    public void hideOverHeated()
    {
        overheatedSign.enabled = false;
    }

    //sign only viewable by overheater
    /*
    [PunRPC]
    public void displayOverHeated()
    {
        if (overHeated)
        {
            overheatedSign.enabled = true;
        }
        else
        {
            overheatedSign.enabled = false;
        }
    }
    */


    public void SetColor(){
		if (m_health <= 0) {
			m_color.r = 0;
			m_color.g = 0;
		}
		else if (m_health > 50) {
			m_color.r = ((100 - m_health) / 50f);
			m_color.g = 1f;
		}
        else {
			m_color.r = 1f;
			m_color.g = (m_health/50f) ;
		}
		GetComponentInChildren<Renderer>().material.color = m_color;
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

    private void weaponUpdatePosition()
    {
        m_weapons[weaponPointer].transform.position = m_Hand.transform.position;
    }

    [PunRPC]
    public void GrabWeapon(int weaponType)
    {
        if (weaponPointer >=0 && weaponPointer <= 4)
        {
            m_weapons[weaponPointer].SetActive(false);
            weaponOn[weaponPointer] = false;
        }
        //Debug.Log("weaponType:" + weaponType);
        weaponPointer = weaponType;
        m_weapons[weaponPointer].SetActive(true);
        weaponOn[weaponPointer] = true;
    }

    [PunRPC]
    public void GetPowerUp(int powerType)
    {
        Debug.Log("I get Power: " + powerType);
        if (powerType == 0)
        {
            HPPickedUp = true;
            //m_power = 0;
            PlayerHealthEmpoweredCounter = PlayerHealthEmpoweredCounter + 1;
        }
        else if (powerType == 1)
        {
            if (WeaponDamageEmpoweredCounter < 5)
            {
                //m_power = 2;
                WeaponDamageEmpoweredCounter = WeaponDamageEmpoweredCounter + 1;
            }
            else
            {
                WeaponDamageEmpoweredCounter = 5;
            }
        }
        else if (powerType == 2)
        {
            if (WeaponSpeedEmpoweredCounter < 5)
            {
                //m_power = 3;
                WeaponSpeedEmpoweredCounter = WeaponSpeedEmpoweredCounter + 1;
            }
            else
            {
                WeaponSpeedEmpoweredCounter = 5;
            }
        }
        else if (powerType == 4)
        {
            this.GetComponent<PhotonView>().RPC("Binojyutsu", PhotonTargets.AllBuffered);
        }
        else if (powerType == 3)
        {
            StopCoroutine(bigBallsCoroutine);
            bigBallsCoroutine = bigBallsPowerup();
            StartCoroutine(bigBallsCoroutine);
        }
        else if (powerType == 5)
        {
            StopCoroutine(invulCoroutine);
            invulCoroutine = invulPowerup();
            StartCoroutine(invulCoroutine);
        }
        //Some one need to handle the number of the power type to add attribue accordingly
    }

    [PunRPC]
    public void Binojyutsu()
    {
        Debug.Log("Nin nin");
        foreach(MeshRenderer mr in child_MR)
        {
            mr.enabled = false;
        }
        nameOnHead.enabled = false;
		if (m_pv.isMine)
			invis_bar.fillAmount = 1.0f;

        StopCoroutine(inviCoroutine);
        inviCoroutine = ComeBack();
        StartCoroutine(inviCoroutine);

    }

    public void showBack()
    {
        Debug.Log("I'm back");
        foreach(MeshRenderer mr in child_MR)
        {
            mr.enabled = true;
        }
        nameOnHead.enabled = true;
    }
    
    private IEnumerator ComeBack()
    {
        yield return new WaitForSeconds(disappearTimer);
        showBack();
    }

    private void Weapon_Cool_Heat()
    {
        if(overHeated == false && m_thermalClip >= 0f)
        {
            m_thermalClip -= m_heatCooldownRate * Time.deltaTime;
        }

        else if(overHeated == true)
		{
			m_thermalClip -= 2.075f*m_heatCooldownRate * Time.deltaTime;
        }

        if(m_thermalClip <= 0)
        {
            m_thermalClip = 0;
        }

    }

    private void HeatingWeapon()
    {
        if(m_thermalClip < m_thermalClipCapacity)
        {
            m_thermalClip = m_thermalClip + m_heat;
        }

        if(m_thermalClip > m_thermalClipCapacity)
        {
            overHeated = true;
            requirePenalty = true;
        }

    }

    private void PenaltyHeatingWeapon()
    {
        Debug.Log("WTF?");
        m_postOverheatShootingPermit = Time.time + m_overheatPenaltyTime;
        requirePenalty = false;
    }

    private void WeaponPowerSort(GameObject weapon)
    {
       
        //if (m_power == 1)//weapon speed
        //{
        for (int i = 0; i < WeaponSpeedEmpoweredCounter; i++)
        {
            weapon.GetComponent<Weapon>().SetSpeed();
        //---
            weapon.GetComponent<Weapon>().SetSize();
        }
        
        //}

        //if (m_power == 2)//weapon damage
        //{
            for (int i = 0; i < (largeBalls? 5 : WeaponDamageEmpoweredCounter); i++)
            {
                weapon.GetComponent<Weapon>().SetDamage();
                //weapon.GetComponent<Weapon>().SetSize();
            }
        //}
    }

    private void PlayerPowerSort()
    {
        //if (m_power == 0)//health
        //{
        if (HPPickedUp == true) { 
            m_pv.RPC("TakeDamage", PhotonTargets.AllBuffered, -10, 10);
            HPPickedUp = false;
            //m_power = 4;
        }
        
        //if(m_power == 3)
        //{
            //dont know how to access
        //}
    }

    public int killIncrement()
    {
        return ++killCount;
        
    }

    public void disableMove()
    {
        //we have to access mouse look so that we can change the state of the mouse
        this.GetComponent<FirstPersonController>().showMouse();

        //disable the chracter Controller so that it won't be able to move anymore, also it will disable the collider
        this.GetComponent<CharacterController>().enabled = false;
        shootEnable = false;
    }

    public float returnHealth()
    {
        return m_health;
    }
    
    IEnumerator bigBallsPowerup()
    {
        Debug.Log("big ball powerup starting");
        largeBalls = true;
		if (m_pv.isMine)
			max_power_bar.fillAmount = 1f;

        yield return new WaitForSeconds(bigBallsTimer);
        largeBalls = false;
        Debug.Log("Big ball powerup ending");
    }

    IEnumerator invulPowerup()
    {
        Debug.Log("invulnerable powerup starting");
        invul = true;
		if (m_pv.isMine)
			invuln_bar.fillAmount = 1f;
        GetComponentInChildren<Renderer>().material.color = Color.white;

        yield return new WaitForSeconds(invulTimer);
        invul = false;
        SetColor();
        Debug.Log("invulnerable powerup ending");
    }

    [PunRPC]
    public void setMyName(string m_name)
    {
        //Debug.Log("Setting Name");
        //Debug.Log(m_name);
        playerName = m_name;
    }

    [PunRPC]
    public void setMyTag()
    {
        if (playerName == "")
        {
            nameOnHead.text = "empty";
        }
        else
        {
            nameOnHead.text = playerName;

        }
    }

    private float AngleToShrinkingZone()
    {
        ShrinkingZoneScript sz = FindObjectOfType<ShrinkingZoneScript>();

        Vector2 tosz = new Vector2(sz.transform.position.x - transform.position.x, sz.transform.position.z - transform.position.z);
        Vector2 facing = new Vector2(playerCamera.transform.forward.x, playerCamera.transform.forward.z);
        
        return Vector2.SignedAngle(facing, tosz);
    }

    public bool isInvulnerable()
    {
        return invul;
    }
}
