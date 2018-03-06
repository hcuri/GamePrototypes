﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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


    private Text m_healthText;
	private Text zoneText;
	private Slider m_healthSlider;
	private Image damageImage;
	private Color damageColor = new Color(1f, 0f, 0f, 0.5f);
	private Color m_color = new Color(0f, 1f, 0f);
	private Slider m_heatSlider;
	private Text m_heatText;
    private PhotonView m_pv;
    private MonoBehaviour m_myPlayerControlScript;
    private GameObject weapon;
    private bool insideZone = true;
    public float m_HPReducedPerSecond = 15.0f;
    private float m_maxHP = 100f;

    //added by Po
    [SerializeField] GameObject[] m_weapons;
    [SerializeField] bool[] weaponOn;
    [SerializeField] private GameObject m_Hand;
    [SerializeField] int weaponPointer;


    private void Start ()
    {
        m_pv = GetComponent<PhotonView>();
        m_healthText = GameObject.Find("HP").GetComponent<Text>();
		zoneText = GameObject.Find ("ZoneText").GetComponent<Text> ();
		m_healthSlider = GameObject.Find ("HPSlider").GetComponent<Slider> ();
		damageImage = GameObject.Find ("DamageImage").GetComponent<Image> ();
		m_heatSlider = GameObject.Find ("HeatSlider").GetComponent<Slider> ();
		m_heatText = GameObject.Find ("Heat").GetComponent<Text> ();
        m_debugMode = GameObject.Find("NetworkManager").GetComponent<PhotonNetworkManager>().returnDebugMode();
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
        weaponPointer = -1;

        //start looking green
        SetColor();
	}

    private void Update()
	{
        if(m_pv.isMine)
        {
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

            Movement();
            m_healthText.text = "HP:" + m_health.ToString();
			m_healthSlider.value = m_health;

			m_heatSlider.value = m_thermalClip;
			if (overHeated) {
				m_heatText.text = "OVERHEATED!";
			} else {
				m_heatText.text = "Heat: " + (int)m_thermalClip + "/" + (int)m_thermalClipCapacity;
			}
			if (!insideZone) {
				TakeDamage (Time.deltaTime * m_HPReducedPerSecond);
				zoneText.text = "You're taking damage inside the zone!";
			} else {
				zoneText.text = "";
			}
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, 0.5f*Time.deltaTime);
            if(weaponPointer != -1)
                weaponUpdatePosition();
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
                    Vector3 pos = camfor * 2.1f;

                    RaycastHit hit;
                    Physics.Raycast(playerCamera.transform.position, camfor, out hit);

                    if(hit.collider != null)
                    {
                        // list of stuff that we can hit
                        if(hit.distance < 2.1f && hit.collider.tag == "Untagged")
                        {
                            Debug.Log("You're too close to something to be able to throw");
                            return;
                        }
                    }

                    Vector3 position = playerCamera.transform.position + pos;
                    Vector3 velocity = playerCamera.transform.forward * m_throwforce;
                    m_pv.RPC("InstantiateWeapon", PhotonTargets.AllBuffered, weapName, position, velocity);
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

    [PunRPC]
    public void InstantiateWeapon(String weaponName, Vector3 position, Vector3 velocity)
    {
        //GameObject infiniteWeapon = PhotonNetwork.Instantiate(weaponName, position, Quaternion.identity, 0);
        GameObject infiniteWeapon = GameObject.Instantiate((GameObject)Resources.Load(weaponName), position, Quaternion.identity);

        infiniteWeapon.transform.Rotate(playerCamera.transform.right * 90);
        infiniteWeapon.GetComponent<Rigidbody>().isKinematic = false;

        float weSpeed = infiniteWeapon.GetComponent<Weapon>().ReturnSpeed();

        infiniteWeapon.GetComponent<Rigidbody>().AddForce(velocity * weSpeed);
        infiniteWeapon.GetComponent<Transform>().localScale *= infiniteWeapon.GetComponent<Weapon>().ReturnScale();
        infiniteWeapon.GetComponent<PhotonView>().RPC("AutoDestroy", PhotonTargets.AllBuffered);
        m_heat = infiniteWeapon.GetComponent<Weapon>().ReturnHeat();
        Debug.Log(infiniteWeapon.name + " is heat: " + m_heat);

        if(m_pv.isMine)
            HeatingWeapon();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
		m_health -= damage;
        if (m_health > m_maxHP) m_health = m_maxHP;
        SetColor();

        if (m_pv.isMine && damage > 0f) {
			damageImage.color = damageColor;
		}

        // hides the dead body *cue murder sound effects
        if (m_health <= 0 && !m_pv.isMine)
            transform.GetChild(1).GetComponent<Renderer>().enabled = false;

        if (m_health <= 0 && m_pv.isMine)
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

	public void SetColor(){
		if (m_health > 50) {
			m_color.r = ((100 - m_health) / 50f);
			m_color.g = 1f;
		} else {
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
        GetComponent<PhotonView>().RPC("SetHandSize", PhotonTargets.AllBuffered);
    }
    [PunRPC] void SetHandSize()
    {
        float sizeCh = 1 * (float)Math.Pow(1.1, (double)WeaponDamageEmpoweredCounter);
        m_weapons[weaponPointer].transform.localScale = new Vector3(sizeCh, sizeCh, sizeCh);
    }

    [PunRPC]
    public void GetPowerUp(int powerType)
    {
        Debug.Log("I get Power: " + powerType);
        if(powerType == 0)
        {
            HPPickedUp = true;
            //m_power = 0;
            PlayerHealthEmpoweredCounter = PlayerHealthEmpoweredCounter + 1;
        }
        else if (powerType == 3)
        {
            //m_power = 1;
            //PlayerSpeedEmpoweredCounter = PlayerSpeedEmpoweredCounter + 1;
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
            GetComponent<PhotonView>().RPC("SetHandSize", PhotonTargets.AllBuffered);
        }
        else if (powerType ==2)
        {if (WeaponSpeedEmpoweredCounter < 5)
            {
                //m_power = 3;
                WeaponSpeedEmpoweredCounter = WeaponSpeedEmpoweredCounter + 1;
            }
            else
            {
                WeaponSpeedEmpoweredCounter = 5;
            }
        }
        //Some one need to handle the number of the power type to add attribue accordingly
    }

    private void Weapon_Cool_Heat()
    {
        if(overHeated == false && m_thermalClip >= 0f)
        {
            m_thermalClip -= m_heatCooldownRate * Time.deltaTime;
        }

        //if(overHeated == true)
        //{
        //    m_thermalClip = m_thermalClipCapacity;
        //}

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
            }
        
        //}

        //if (m_power == 2)//weapon damage
        //{
            for (int i = 0; i < WeaponDamageEmpoweredCounter; i++)
            {
                weapon.GetComponent<Weapon>().SetDamage();
                weapon.GetComponent<Weapon>().SetSize();
            
            }
        //}
    }

    private void PlayerPowerSort()
    {
        //if (m_power == 0)//health
        //{
        if (HPPickedUp == true) { 
            m_pv.RPC("TakeDamage", PhotonTargets.AllBuffered, -10);
            HPPickedUp = false;
            //m_power = 4;
        }
        
        //if(m_power == 3)
        //{
            //dont know how to access
        //}
    }
}
