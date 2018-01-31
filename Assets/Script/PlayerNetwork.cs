using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetwork : MonoBehaviour {

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private MonoBehaviour[] playerControlScripts;

    private PhotonView pv;

    public int playerHealth = 100;

    private void Initialize()
    {
        if(pv.isMine)
        {

        }

        //Handle things for non-local character
        else
        {
            //Disable the camera
            playerCamera.SetActive(false);

            //Disable the control scripts
            foreach(MonoBehaviour m in playerControlScripts)
            {
                m.enabled = false;
            }
        }
    }

	// Use this for initialization
	private void Start ()
    {
        pv = GetComponent<PhotonView>();

        Initialize();

	}

    private void Update()
    {
        if(pv.isMine)
        {
            /*if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("I press left Mouse");
                this.GetComponent<PlayerStatusAndAction>().ThrowObject(); ;
            }*/
            return;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            
            playerHealth -= 5;
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(playerHealth);
        }

        else if(stream.isReading)
        {
            playerHealth = (int)stream.ReceiveNext();
        }
    }
	

}
