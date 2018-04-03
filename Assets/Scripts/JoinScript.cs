using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinScript : MonoBehaviour {

    public InputField input_text;
    public GameObject nameCarrier;

    private void Start()
    {
        input_text = GameObject.Find("NameInput").GetComponent<InputField>();
        nameCarrier = GameObject.Find("NameCarrier");

    }

    public void NextScene()
	{
        Debug.Log("Jump");
        nameCarrier.GetComponent<CarryName>().carryName(input_text.text);
		SceneManager.LoadScene("Scene1");
	}
	public void ExitGame()
	{
		Application.Quit ();
	}
}
