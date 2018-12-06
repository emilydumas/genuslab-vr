using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTool : MonoBehaviour {	
	public bool allowScreenshots = false;

	void toggle() {
		allowScreenshots = !allowScreenshots;
	}

	void Update () {
		if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && ((Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))  && Input.GetKeyDown(KeyCode.S))) {
			toggle();
		}

		if (allowScreenshots) {
			if (OVRInput.GetDown(OVRInput.RawButton.Y) || ((Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))  && Input.GetKeyDown(KeyCode.S))) {
				// Take a screenshot
				DateTime dt = DateTime.Now;
				ScreenCapture.CaptureScreenshot(Application.productName + " " + dt.ToString("yyyy-MM-dd THH.mm.ss") + ".png", 4);
			}
		}
	}
}
