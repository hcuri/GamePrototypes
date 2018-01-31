using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShrinkingZoneScript : MonoBehaviour
{
    public float timeToShrink = 20;
    public float initialRadius = 25;
    float currentTime;
    MeshFilter reverse;

    // Use this for initialization; randomize the translation with respect to the level
    void Start()
    {
        transform.position = new Vector3(0, 0, 0); // TODO: should be random

        Debug.Log(initialRadius);
        transform.localScale = new Vector3(initialRadius, 10, initialRadius);

        currentTime = 0;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        int[] inside = mesh.triangles.Reverse().ToArray();
        int[] outside = mesh.triangles.ToArray();

        int[] inout = new int[inside.Length + outside.Length];
        inside.CopyTo(inout, 0);
        outside.CopyTo(inout, inside.Length);

        mesh.triangles = inout;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        float currsize = initialRadius * (1.0f - currentTime / timeToShrink);
        if (currsize > 0)
        {
            transform.localScale = new Vector3(currsize, 10, currsize);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // if it's a player
        // set the player's isInsideZone to true
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("NAAAAYSSSS");
        }
    }

    //
    void OnTriggerExit(Collider col)
    {
        // if it's a player
        // set the player's isInsideZone to false
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("YOU'RE GONNA HAVE A BAD TIME");
        }
    }

}