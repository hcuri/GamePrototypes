using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

    //private IEnumerator RemoveCollider;

	// Use this for initialization
	void Start () {
        StartCoroutine(RemoveCollider());
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(gameObject, 5.0f);
	}

    IEnumerator RemoveCollider()
    {
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
