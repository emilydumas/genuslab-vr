using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this to the left hand anchor of the OVRCameraRig for some very simple
// (but not very good) grabbing support.

// Note the left hand trigger is hard-coded as the activation button, so you
// must attach to the left hand anchor!

// This grabbing feature should really be improved to something usable...

public class WarpGrab : MonoBehaviour {
    public GameObject surface;
	
	// Update is called once per frame
	void Update () {
	    if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger)) {
            surface.transform.position = gameObject.transform.position;
        }
	    if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || OVRInput.Get(OVRInput.RawButton.LHandTrigger)) {
            surface.transform.rotation = gameObject.transform.rotation;
        }
	}
}
