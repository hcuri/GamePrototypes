using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonNetworkManager : Photon.PunBehaviour
{

    public int numPeopleToStart;
    [SerializeField] private Text netInfo;
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject shrinkingZone;
    [SerializeField] private GameObject lobbyCamera;

	private Text waitingText;
	//private Text playersRemain;
	private bool joinedRoom;
    public float countDownToStart = 5.0f;
    private bool countingDown = false;
    public int m_ID = -1;

    [SerializeField] private Text killText;
    [SerializeField] private Text killCountText;
    [SerializeField] private GameObject[] PlayersInGame;

    public GameObject nameCarrier;

    // todo: remove
    public bool debugMode = false;

    // Use this for initialization
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings("ver 0.1");
        PhotonNetwork.automaticallySyncScene = true;
        waitingText = GameObject.Find("WaitingText").GetComponent<Text>();
        waitingText.text = "";
        /*playersRemain = GameObject.Find ("PlayersRemain").GetComponent<Text> ();
		playersRemain.text = "";*/
        killText = GameObject.Find("KillMessage").GetComponent<Text>();
        killText.text = "";
        killCountText = GameObject.Find("KillCountMessage").GetComponent<Text>();
        killCountText.text = "Killed 0 Capsules";
        joinedRoom = false;
        //Initial the array which to keep track of players
        PlayersInGame = new GameObject[numPeopleToStart];
        nameCarrier = GameObject.Find("NameCarrier");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("We are now joined lobby");

        // todo: fix sometimes clients both try connecting at the same time, then fail and make their own rooms
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("Imma make my own room, with blackjack and hookahz!");

        PhotonNetwork.CreateRoom(null);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Creating Room for peoplez");
        PhotonNetwork.room.MaxPlayers = (debugMode ? 1 : numPeopleToStart);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("We are nao joishishi");

        doMatchMakingUpdateForPlayers();
        doMatchMaking();
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Sum1 Elsa connected");

        doMatchMakingUpdateForPlayers();
        doMatchMaking();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        // if game has started in this room
        if (countDownToStart < 0 || countingDown)
        {
            int currentNumPlayers = PhotonNetwork.room.PlayerCount;
            Debug.Log("remaining players: " + currentNumPlayers);

            // probably means that you have won
            /*if(currentNumPlayers == 1)
            {
                Debug.Log("You iZ Winna!");
				PhotonNetwork.Disconnect();
				Cursor.lockState = CursorLockMode.None;
				//Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				SceneManager.LoadScene("WinScene");
            }*/
        }
        else
        {
            // todo: it's weird how sometimes disconnecting takes a while
            doMatchMakingUpdateForPlayers();
            // we are sure that we won't start the game when we have less than what we want

        }
    }

    private void Update()
    {
        netInfo.text = PhotonNetwork.connectionStateDetailed.ToString();
        if (countingDown)
		{            
            countDownToStart -= Time.deltaTime;
            if (countDownToStart < 0)
            {
				joinedRoom = true;
                countingDown = false;

                GameObject go = PhotonNetwork.Instantiate(player.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
                
                int id = go.GetComponent<PhotonView>().ownerId;
				go.transform.Translate(spawnPoint[id % spawnPoint.Length].position.x, spawnPoint[id % spawnPoint.Length].position.y, spawnPoint[id % spawnPoint.Length].position.z);
				go.transform.Rotate (new Vector3 (0, 1, 0), (id % spawnPoint.Length) * 60 - 30);
                go.GetComponent<PhotonView>().RPC("setMyID", PhotonTargets.AllBuffered,id);
                if (go.GetComponent<PhotonView>().isMine)
                    m_ID = id;
                PlayersInGame[id-1] = go;

                lobbyCamera.SetActive(false);

                GameObject.Find("ShrinkingZone").GetComponent<ShrinkingZoneScript>().startShrinking();
            }
        }

		if (!joinedRoom && !countingDown) {
			waitingText.text = "Waiting for more players..." + PhotonNetwork.room.PlayerCount + "/" + numPeopleToStart;
			//playersRemain.text = "";
		} else if (!joinedRoom && countingDown) {
			waitingText.text = "Game is starting in... " + (int)(countDownToStart + 1);
			//playersRemain.text = "Players remaining: " + PhotonNetwork.room.PlayerCount + "/" + numPeopleToStart;
		} else {
			waitingText.text = "";
			//playersRemain.text = "Players remaining: " + PhotonNetwork.room.PlayerCount + "/" + numPeopleToStart;
            netInfo.text = "";
        }
    }


    private void doMatchMakingUpdateForPlayers()
    {
        int currentNumPlayers = PhotonNetwork.room.PlayerCount;

        Debug.Log("number of players connected: " + currentNumPlayers);
        Debug.Log("Waiting for " + (numPeopleToStart - currentNumPlayers) + " more players");
    }

    private void doMatchMaking()
    {
        int currentNumPlayers = PhotonNetwork.room.PlayerCount;

        // todo: remove debugmode
        if (currentNumPlayers == numPeopleToStart || debugMode)
        {
            PhotonNetwork.room.IsOpen = debugMode;//false;
            Debug.Log("We are going to start the game");
            countingDown = true;
        }
    }

    public bool returnDebugMode()
    {
        return (debugMode);
    }

    public void killWarn(int killer, int victim)
    {
        Debug.Log("Killer is: " + killer + " Victim is: " + victim);
        if (killer != -1)
            killText.text = "Player" + killer + " has killed Player" + victim + "\n";
        else
        {
            Debug.Log("Player " + victim + "has died in the shrinking zone");
            killText.text = "Player" + victim + " has died in the shrinking zone\n";
        }

        GameObject myPlayer = PlayersInGame[killer - 1];

        int killCount = myPlayer.GetComponent<PlayerNetwork>().killIncrement();

        if (myPlayer.GetComponent<PhotonView>().isMine)
        {
            killCountText.text = "Killed " + killCount + " Capsules";
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game!!!");
        Application.Quit();
    }

    public void Reload()
    {
        PhotonNetwork.Disconnect();
        //Scene m_Scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Scene0");
    }
}
