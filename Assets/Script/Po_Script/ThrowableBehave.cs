using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]


public class ThrowableBehave : MonoBehaviour {

    Collider m_Collider;
    //when player pick up the weapon on the ground, he will use this number to detect which weapon they shuold hold
    public int weaponNum;
    public float m_Damage;
    public string m_Type;
	// Use this for initialization
	void Start () {
        tag = "Weapon";
        m_Collider = GetComponent<Collider>();
        //make sure the setting is correct
        m_Collider.isTrigger = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
