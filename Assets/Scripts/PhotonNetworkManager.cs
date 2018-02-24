using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonNetworkManager : Photon.PunBehaviour {

    public int numPeopleToStart;
    [SerializeField] private Text netInfo;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject shrinkingZone;
    [SerializeField] private GameObject lobbyCamera;

	private Text waitingText;
	private bool joinedRoom;

	// Use this for initialization
	private void Start () {
        PhotonNetwork.ConnectUsingSettings("ver 0.1");
        PhotonNetwork.automaticallySyncScene = true;
		waitingText = GameObject.Find("WaitingText").GetComponent<Text>();
		waitingText.text = "";
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("We are now joined lobby");

        PhotonNetwork.JoinOrCreateRoom("New", null, null);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == numPeopleToStart)
        {
			joinedRoom = true;
			waitingText.text = "";
            Debug.Log("We are now joined room");
            PhotonNetwork.Instantiate(player.name, spawnPoint.position, spawnPoint.rotation, 0);
            lobbyCamera.SetActive(false);

            //if (PhotonNetwork.isMasterClient)
            //{
            GameObject.Find("ShrinkingZone").GetComponent<ShrinkingZoneScript>().startShrinking();
            //}
        }
        else
        {
			joinedRoom = false;
			waitingText.text = "Waiting for more players..." + PhotonNetwork.room.PlayerCount + "/" + numPeopleToStart;
			Debug.Log("Waiting for more players");
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log(PhotonNetwork.room.PlayerCount);
        if (PhotonNetwork.room.PlayerCount == numPeopleToStart)
		{
			joinedRoom = true;
			waitingText.text = "";
            Debug.Log("We are now joined room");
            PhotonNetwork.Instantiate(player.name, spawnPoint.position, spawnPoint.rotation, 0);
            lobbyCamera.SetActive(false);

            //if (PhotonNetwork.isMasterClient)
            //{
            GameObject.Find("ShrinkingZone").GetComponent<ShrinkingZoneScript>().startShrinking();
        }
    }

    private void Update () {
		netInfo.text = PhotonNetwork.connectionStateDetailed.ToString();
		if (!joinedRoom) {
			waitingText.text = "Waiting for more players..." + PhotonNetwork.room.PlayerCount + "/" + numPeopleToStart;
		}
	}
}
