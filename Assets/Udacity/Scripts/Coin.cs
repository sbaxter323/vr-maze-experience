using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour 
{
	public GameObject coinPoofPrefab;
	private Maze maze;

    public void OnCoinClicked() {
        // Instantiate the CoinPoof Prefab where this coin is located
        // Make sure the poof animates vertically
        // Destroy this coin. Check the Unity documentation on how to use Destroy
		GameObject poof = Instantiate(coinPoofPrefab) as GameObject;
		poof.transform.localPosition = this.transform.localPosition;
		maze.updateSolutionCanvas();
		Destroy (this.gameObject);

    }

	public void setMaze(Maze m) {
		maze = m;
	}

	void Update()
	{
		// Spin clockwise
		Vector3 rotationVector = new Vector3 (0.0f, 15.0f, 0.0f);
		transform.Rotate (rotationVector * Time.deltaTime);
	}
}
