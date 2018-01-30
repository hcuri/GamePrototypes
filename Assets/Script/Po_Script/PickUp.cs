using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all the pickup should have Collider as well as MeshRenderer
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]

public class PickUp : MonoBehaviour {

    //[SerializeField] private float respawnTime;
    [SerializeField] public float cooldownTime = 5.0f;
    public Collider m_Collider;
    public MeshRenderer m_MRenderer;
    public bool isCooldown;
    public float cooldownCounter;

    public enum pickUpType
    {
        weapon = 0,
        powerUp = 1
    }

    public pickUpType m_PUType;

    // Use this for initialization
    void Start () {
        //set all the pickup to be a trigger
        m_Collider = GetComponent<Collider>();
        m_Collider.isTrigger = true;
        m_MRenderer = GetComponent<MeshRenderer>();
        m_MRenderer.enabled = true;
        //Remember to set all object to the state you want in the beginning
        isCooldown = false;
        gameObject.tag = "PickUp";
	}
	
	// Update is called once per frame
	void Update () {
        //if it is cooldonwing, reduce the cooldown time
        /*if (isCooldown)
        {
            cooldownCounter -= Time.deltaTime;
            if(cooldownCounter <= 0)
            {
                Respawn();
            }
        }*/
	}

    private void OnTriggerEnter(Collider other)
    {
        //if it is not cooldowning, deal with its utility
        if (!isCooldown) {
            //it can only be interact with player
            if (other.CompareTag("Player")) 
            {
                BeTaken();
                //other.GetComponent<PlayerSatus>().updatePlayer()
            }
        }
    }

    public void Respawn()
    {
        isCooldown = false;
        m_MRenderer.enabled = true;
        m_Collider.enabled = true;
    }

    public void BeTaken()
    {
        m_MRenderer.enabled = false;
        m_Collider.enabled = false;
        isCooldown = true;
        StartCoroutine(waitRespawn(cooldownTime));
        //cooldownCounter = cooldownTime;
        //PlayerUpdate(int power, string type,)
        //
    }

    //It seems like working
    private IEnumerator waitRespawn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Respawn();
    }
}
