using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private MazeCell currentCell;
	private GameObject camera;

	public void setCamera (GameObject cam){
		camera = cam;
	}

	public void SetLocation (MazeCell cell) {
		transform.parent = cell.transform.parent;
		transform.localPosition = cell.transform.localPosition;
		Vector3 correction = new Vector3 (1.1f, 0f, -.5f);
		transform.localPosition = transform.localPosition + correction;
		//camera.transform.parent = transform.parent;
		camera.transform.localPosition = transform.localPosition;
	}
}
