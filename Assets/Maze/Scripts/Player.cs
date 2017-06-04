using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private MazeCell currentCell;
	private GameObject camera;

	public void setCamera (GameObject cam){
		camera = cam;
	}

	public void SetLocation (Transform mazeTransform, IntVector2 coordinates) {
		currentCell = cell;
		newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
		transform.localPosition = cell.transform.localPosition;
		camera.transform.localPosition = cell.transform.localPosition;
	}
}
