using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKeeper : MonoBehaviour {

    public GameObject[] keepWeapons;


	// Use this for initialization
	void Start () {
        tag = "WeaponKeeper";
        if (keepWeapons.Length == 0)
        {
            Debug.Log("You forget to drag weapon you need into the keeper");
        }	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject SellWeapon(int weaponNum)
    {
        Debug.Log("Selling weapon" + weaponNum);
        GameObject weaponToCreate;
        weaponToCreate = Instantiate(keepWeapons[weaponNum]);
        return weaponToCreate;
    }
}
