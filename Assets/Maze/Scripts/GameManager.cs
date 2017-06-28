using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;
	public Player playerPrefab;
	public Key keyPrefab;
	public MazeSolutionRoom solutionRoomPrefab;
	public GameObject camera;
	public Coin coinPrefab;
	public int numCoins;
	public int maxWaypointHops;

	private Maze mazeInstance;
	private Player playerInstance;
	private Key keyInstance;
	private MazeSolutionRoom solutionRoomInstance;
	private Coin coinInstance;
	private Coin[] coinInstances;
	private SignPost signpostInstance;

	private void Start () {
		//StartCoroutine(BeginGame());
		BeginGame();
	}

	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RestartGame();
		}
	}

	private void BeginGame() {
		mazeInstance = Instantiate(mazePrefab) as Maze;
		playerInstance = Instantiate(playerPrefab) as Player;
		playerInstance.setCamera (camera);
		keyInstance = Instantiate (keyPrefab) as Key;
		solutionRoomInstance = Instantiate (solutionRoomPrefab) as MazeSolutionRoom;
		solutionRoomInstance.mazeSign.setManager (this);
		signpostInstance = solutionRoomInstance.mazeSign;
		keyInstance.setDoor (solutionRoomInstance.mazeDoor);

		coinInstances = new Coin[numCoins];
		for (int i = 0; i < numCoins; i++) {
			coinInstance = Instantiate (coinPrefab) as Coin;
			coinInstance.setMaze (mazeInstance);
			coinInstances [i] = coinInstance;
		}
		mazeInstance.Generate (playerInstance, keyInstance, solutionRoomInstance, coinInstances, numCoins, maxWaypointHops, signpostInstance);

	}

	public void RestartGame () {
		if (playerInstance != null) {
			Destroy(playerInstance.gameObject);
		}
		if (keyInstance != null) {
			Destroy (keyInstance.gameObject);
		}
		if (solutionRoomInstance != null) {
			Destroy (solutionRoomInstance.gameObject);
		}

		for (int i = 0; i < numCoins; i++) {
			if (coinInstances[i] != null) {
				Destroy (coinInstances[i].gameObject);
			}
		}
		Destroy (mazeInstance.gameObject);
		BeginGame();
	}


}
