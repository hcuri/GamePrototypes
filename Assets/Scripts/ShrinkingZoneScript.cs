using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShrinkingZoneScript : Photon.MonoBehaviour
{
    public Vector2 initialPosRandomMax;
    public float timeToShrink = 20;
    public float initialRadius = 25;
    public float height = 50;
    float currentTime;
    MeshFilter reverse;

    private bool shouldShrink = false;

    // Use this for initialization; randomize the translation with respect to the level
    void Start()
    {
        float initialX = initialPosRandomMax.x;
        float initialY = initialPosRandomMax.y;
        initialX = Random.Range(-initialX, initialX);
        initialY = Random.Range(-initialY, initialY);
        transform.position = new Vector3(initialX, 0, initialY);

        Debug.Log(transform.position);

        //if (PhotonNetwork.isMasterClient)
        //{
        //    transform.localScale = new Vector3(initialRadius, height, initialRadius);

        //    Vector2 initialPos = Random.insideUnitCircle * GetComponent<Renderer>().bounds.extents.magnitude;
        //    Debug.Log(initialPos);
        //    transform.position = new Vector3(initialPos.x, initialPos.y);
        //}

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
        if (shouldShrink)
        {
            currentTime += Time.deltaTime;

            float currsize = initialRadius * (1.0f - currentTime / timeToShrink);
            if (currsize > 10)
            {
                transform.localScale = new Vector3(currsize, height, currsize);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // if it's a player
        // set the player's isInsideZone to true
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("NAAAAYSSSS");
            col.gameObject.GetComponent<PlayerNetwork>().setInsideZone(true);
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
            col.gameObject.GetComponent<PlayerNetwork>().setInsideZone(false);
        }
    }

    public void startShrinking()
    {
        shouldShrink = true;
    }
}