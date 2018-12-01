using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCollision : MonoBehaviour {

	 public Rigidbody rb;
	 private bool pressed;
	 Color originalColor;
    void Start()
    {
		originalColor=gameObject.GetComponent<Renderer> ().material.color;
		pressed=false;
        rb = GetComponent<Rigidbody>();
    }

    // Let the rigidbody take control and detect collisions.
    void Enable()
    {
		
        rb.detectCollisions = true;
    }

    // Let animation control the rigidbody and ignore collisions.
    void Disable()
    {
		
        rb.detectCollisions = false;
    }
	// Update is called once per frame
	void Update () {
		if(OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)){
			if(!pressed){
				Enable();
				rb.WakeUp();
				Debug.Log("pressed");
				pressed=true;
				gameObject.GetComponent<Renderer> ().material.color = Color.red;
			}
		}
		else if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger)){
			Debug.Log("unpressed");
			Disable();
			pressed=false;
			gameObject.GetComponent<Renderer> ().material.color = originalColor;
		}
		
	}
}