using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour 
{
	public GameObject coinPoofPrefab;
	private Maze maze;
	private int coinIndex;

    public void OnCoinClicked() {
		GameObject poof = Instantiate(coinPoofPrefab) as GameObject;
		poof.transform.localPosition = this.transform.localPosition;
		maze.coinGrabbed(coinIndex);
		Destroy (this.gameObject);

    }

	public void setMaze(Maze m) {
		maze = m;
	}

	public void setCoinIndex(int ci) {
		coinIndex = ci;
	}

	void Update()
	{
		// Spin clockwise
		Vector3 rotationVector = new Vector3 (0.0f, 15.0f, 0.0f);
		transform.Rotate (rotationVector * Time.deltaTime);
	}
}
