using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour {

    public GameObject[] playerInGame;
    public int playerCount;
    public bool playerInGameIsFilled;

    public GameObject endPanel;
    public GameObject winPanel;
    // Use this for initialization
    void Start () {
        playerInGameIsFilled = false;

        //add by Po,
        endPanel = GameObject.Find("EndPanel");
        winPanel = GameObject.Find("WinPanel");
        endPanel.SetActive(false);
        winPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (!playerInGameIsFilled)
        {
            playerInGame = GameObject.FindGameObjectsWithTag("Player");
            playerCount = playerInGame.Length;
            playerInGameIsFilled = true;
        }
	}

    public void playerDead(GameObject deadPlayer)
    {
        if (deadPlayer.GetComponent<PhotonView>().isMine)
        {
            setLost();
        }

        playerCount--;
        if(playerCount == 1)
        {
            checkWin();
        }
        return;
    }

    public int requestPlayerNumber()
    {
        return playerCount;
    }

    public void setWin()
    {
        winPanel.SetActive(true);
    }

    public void setLost()
    {
        endPanel.SetActive(true);
    }

    public void checkWin()
    {
        foreach(GameObject livePlayer in playerInGame)
        {
            if(livePlayer.GetComponent<PlayerNetwork>().returnHealth() >= 0.0f)
            {

            }
        }
    }

    [PunRPC]
    public void
}
