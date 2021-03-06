﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour {

    public GameObject[] playerInGame;
    public int playerCount;
    public int total_Player;
    public bool playerInGameIsFilled;
    public GameObject pNManager;
    public GameObject endPanel;
    public GameObject winPanel;
    public bool isDebugging;
    public Text playersRemain;
    //public ShrinkingZoneScript m_shrinking;

    public enum StageType
    {
        InitialStage,
        FinalStage
    }

    public StageType m_Stage;

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
        total_Player = playerCount;
        playersRemain = GameObject.Find("PlayersRemain").GetComponent<Text>();
        playersRemain.text = "";
        m_Stage = StageType.InitialStage;
        updatePlayerRemain();
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void updatePlayerRemain()
    {
        playersRemain.text = "Player Remaining: " + playerCount + "/" + total_Player;
    }

    public void playerDead(GameObject deadPlayer)
    {
        if (deadPlayer.GetComponent<PhotonView>().isMine)
        {
            setLost();
        }

        playerCount--;
        updatePlayerRemain();
        if (playerCount == 1 && !isDebugging)
        {
            checkWin();
        }
        return;
    }
    
    public void playerDisconnected()
    {
        playerCount--;
        updatePlayerRemain();
        if (playerCount == 1 && !isDebugging)
        {
            checkWin();
        }
        return;
    }

    public int requestPlayerNumber()
    {
        return playerCount;
    }

    public void setLost()
    {
        Debug.Log(endPanel.activeSelf + " " + winPanel.activeSelf);
        if (!endPanel.activeSelf && !winPanel.activeSelf)
            endPanel.SetActive(true);
        Debug.Log(endPanel.activeSelf + " " + winPanel.activeSelf);
    }

    public void checkWin()
    {
        playerInGame = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject livePlayer in playerInGame)
        {
            if(livePlayer.GetComponent<PlayerNetwork>().returnHealth() > 0)
            {
                int id = livePlayer.GetComponent<PhotonView>().ownerId;

                this.GetComponent<PhotonView>().RPC("sendWin", PhotonPlayer.Find(id));

                livePlayer.GetComponent<FirstPersonController>().showMouse();
            }
        }
    }

    [PunRPC]
    public void sendWin()
    {
        if (!endPanel.activeSelf && !winPanel.activeSelf)
            winPanel.SetActive(true);
    }

    public void processToNextStage()
    {
        m_Stage = StageType.FinalStage;
    }
}
