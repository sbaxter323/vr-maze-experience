using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour 
{
    //Create a reference to the KeyPoofPrefab and Door
	public GameObject keyPoofPrefab;
	public GameObject Door; 

	private int transformDirection;

	void Awake()
	{
		transformDirection = 1;
	}
	void Update()
	{
		//Not required, but for fun why not try adding a Key Floating Animation here :)
		// Spin clockwise
		Vector3 rotationVector = new Vector3(0.0f, 0.0f, 15.0f);
		transform.Rotate(rotationVector * Time.deltaTime);

		// Move Up and Down
		if (transform.localPosition.y > .2) {
			transformDirection = -1;
		} else if (transform.localPosition.y < -.2) {
			transformDirection = 1;
		}
		Vector3 transformVector = new Vector3 (0.0f, 0.05f * transformDirection * Time.deltaTime, 0.0f);
		transform.Translate (transformVector, Space.World);
	}

	public void OnKeyClicked()
	{
        // Instatiate the KeyPoof Prefab where this key is located
        // Make sure the poof animates vertically
        // Call the Unlock() method on the Door
        // Set the Key Collected Variable to true
        // Destroy the key. Check the Unity documentation on how to use Destroy
		GameObject poof = Instantiate(keyPoofPrefab) as GameObject;
		poof.transform.localPosition = this.transform.localPosition;
		Destroy (this.gameObject);

    }

}
