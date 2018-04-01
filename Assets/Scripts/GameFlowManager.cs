using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour {

    public GameObject[] playerInGame;
    public int playerCount;
    public bool playerInGameIsFilled;
	// Use this for initialization
	void Start () {
        playerInGameIsFilled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!playerInGameIsFilled)
        {
            playerInGame = GameObject.FindGameObjectsWithTag("Player");
            playerCount = playerInGame.Length;
            playerInGameIsFilled = false;
        }
	}
}
