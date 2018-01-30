using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The power up is inheritated from PickUp
/// </summary>
public class PowerUp : PickUp {

    enum powerType
    {
        Damage = 0,
        Power = 1,
        Speed = 2,
        Jump = 3
    }

    [SerializeField] private float powerUpCDTime = 10.0f;
    [SerializeField] private powerType m_powerType = powerType.Damage;
    [SerializeField] private float powerAmount = 1;

	// Use this for initialization
	void Start () {
        cooldownTime = powerUpCDTime;
        //set all the pickup to be a trigger
        m_Collider = GetComponent<Collider>();
        m_Collider.isTrigger = true;
        m_MRenderer = GetComponent<MeshRenderer>();
        m_MRenderer.enabled = true;
        //Remember to set all object to the state you want in the beginning
        isCooldown = false;
        gameObject.tag = "Power-Up";
        m_PUType = pickUpType.powerUp;
    }
	

    private void OnTriggerEnter(Collider other)
    {
        //if it is not cooldowning, deal with its utility
        if (!isCooldown)
        {
            //it can only be interact with player
            if (other.CompareTag("Player"))
            {
                BeTaken();
                //other.GetComponent<PlayerSatus>().updatePlayer()
            }
        }
    }
}
