using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryName : MonoBehaviour {

    public string input_name;
    private static bool created = false;

	// Use this for initialization
	void Awake () {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
	}
	
    public void carryName(string name_i_got)
    {
        input_name = name_i_got;
    }
}
