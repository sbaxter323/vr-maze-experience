using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour 
{
    // Create a boolean value called "locked" that can be checked in OnDoorClicked() 
    // Create a boolean value called "opening" that can be checked in Update() 
	private bool locked = true;
	private bool opening = false;

    void Update() {
		if (opening == true) {
			if (transform.localPosition.y < 8.0f) {
				transform.Translate (0.0f, 1.0f * Time.deltaTime, 0.0f);
			} else {
				opening = false;
			}
		}
    }

    public void OnDoorClicked() {
		if (locked == false) {
			opening = true;
		} else {
			AudioSource audio = GetComponent<AudioSource> ();
			audio.Play ();
		}
    }

    public void Unlock()
    {
		locked = false;
    }
}
