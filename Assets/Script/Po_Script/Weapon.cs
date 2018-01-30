using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The power up is inheritated from PickUp
/// </summary>
public class Weapon : PickUp
{

    /*enum weaponType
    {
        Stone = 1,
        Grenade = 2,
        Boomerang = 3
    }*/

    [SerializeField] private float weaponCDTime = 8.0f;
    //[SerializeField] private weaponType m_weaponType = weaponType.Stone;
    //[SerializeField] private float powerAmount = 1;
    public int weaponTypeNum = 0;

    // Use this for initialization
    void Start()
    {
        cooldownTime = weaponCDTime;
        //set all the pickup to be a trigger
        m_Collider = GetComponent<Collider>();
        m_Collider.isTrigger = true;
        m_MRenderer = GetComponent<MeshRenderer>();
        m_MRenderer.enabled = true;
        //Remember to set all object to the state you want in the beginning
        isCooldown = false;
        gameObject.tag = "WeaponPick";
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
