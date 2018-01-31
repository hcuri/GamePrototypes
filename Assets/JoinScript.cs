using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinScript : MonoBehaviour {

	public void NextScene()
	{
		SceneManager.LoadScene("Scene1");
	}
	public void ExitGame()
	{
		Application.Quit ();
	}
}
