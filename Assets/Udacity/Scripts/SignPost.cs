﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SignPost : MonoBehaviour
{	
	private GameManager manager;

	public void setManager(GameManager gm) {
		manager = gm;
	}

	public void ResetScene() 
	{
		manager.RestartGame ();
	}
}