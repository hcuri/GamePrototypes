using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonNetworkManager : Photon.PunBehaviour {

    [SerializeField] private Text netInfo;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject shrinkingZone;
    [SerializeField] private GameObject lobbyCamera;

	// Use this for initialization
	private void Start () {
        PhotonNetwork.ConnectUsingSettings("ver 0.1");
	}

    public override void OnJoinedLobby()
    {
        Debug.Log("We are now joined lobby");

        PhotonNetwork.JoinOrCreateRoom("New", null, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("We are now joined room");
        PhotonNetwork.Instantiate(player.name, spawnPoint.position, spawnPoint.rotation, 0);
        lobbyCamera.SetActive(false);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);

            PhotonNetwork.Instantiate(shrinkingZone.name, new Vector3(0f, shrinkingZone.GetComponent<CapsuleCollider>().height/2.0f, 0f), Quaternion.identity, 0);
        }
    }
	
	private void Update () {
        netInfo.text = PhotonNetwork.connectionStateDetailed.ToString();
	}
}
