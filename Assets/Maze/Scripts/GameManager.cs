using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;
	private Maze mazeInstance;
	public Player playerPrefab;
	public GameObject camera;

	private Player playerInstance;

	private void Start () {
		//StartCoroutine(BeginGame());
		BeginGame();
	}

	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RestartGame();
		}
	}

	//private IEnumerator BeginGame () {
	private void BeginGame() {
		mazeInstance = Instantiate(mazePrefab) as Maze;
		playerInstance = Instantiate(playerPrefab) as Player;
		playerInstance.setCamera (camera);
		//StartCoroutine(mazeInstance.Generate (playerInstance));
		mazeInstance.Generate (playerInstance);
	}

	private void RestartGame () {
		StopAllCoroutines();
		if (playerInstance != null) {
			Destroy(playerInstance.gameObject);
		}
		Destroy(mazeInstance.gameObject);
		//StartCoroutine(BeginGame());
		BeginGame();
	}


}
