using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameFlowManager : MonoBehaviour {

    public GameObject[] playerInGame;
    public int playerCount;
    public bool playerInGameIsFilled;
    public GameObject pNManager;
    public GameObject endPanel;
    public GameObject winPanel;
    public bool isDebugging;
    // Use this for initialization
    void Start () {
        playerInGameIsFilled = false;

        //add by Po,
        endPanel = GameObject.Find("EndPanel");
        winPanel = GameObject.Find("WinPanel");
        endPanel.SetActive(false);
        winPanel.SetActive(false);
        pNManager = GameObject.Find("NetworkManager");
        isDebugging = pNManager.GetComponent<PhotonNetworkManager>().debugMode;
        playerCount = pNManager.GetComponent<PhotonNetworkManager>().numPeopleToStart;
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void playerDead(GameObject deadPlayer)
    {
        if (deadPlayer.GetComponent<PhotonView>().isMine)
        {
            setLost();
        }

        playerCount--;
        if(playerCount == 1 && !isDebugging)
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
        Debug.Log("Checking Winner");
        playerInGame = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("playerInGame: " + playerInGame);
        foreach(GameObject livePlayer in playerInGame)
        {
            if(livePlayer.GetComponent<PlayerNetwork>().returnHealth() > 0)
            {
                int id = livePlayer.GetComponent<PhotonView>().ownerId;

                this.GetComponent<PhotonView>().RPC("sendWin", PhotonPlayer.Find(id));
            }
        }
    }

    [PunRPC]
    public void sendWin()
    {
        winPanel.SetActive(true);
    }
}
