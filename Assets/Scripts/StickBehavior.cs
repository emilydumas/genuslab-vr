// Make a GameObject into a "marker" or "laser" that can draw on a the
// PaintableTexture in the scene.  Marks are drawn where a ray from the
// GameObject hits something which is paintable.  The ray is in the direction of
// the object's local "up".

// User interface logic lives elsewhere; this script mostly adds public methods
// to the GameObject to perform functions related to drawing.  Hiding or showing
// the GameObject is also supported.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickBehavior : MonoBehaviour {

    public OVRInput.Controller Controller;
	public float maxDist = Mathf.Infinity;
	public Material inactiveBeamMaterial;
	public Material activeBeamMaterial;
	private Renderer[] rends;
	private Renderer beamRenderer;
	private Rigidbody beamRB;
	private bool isActive = false;  // When "active", the laser draws on things and presses buttons
	private bool isVisible = true;
	private bool drill = false; // not supported yet
	private PaintableTexture pt;
	private LayerMask layerMask = Physics.DefaultRaycastLayers;
	
	void Start () {
		rends = gameObject.GetComponentsInChildren<Renderer>();
		// Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
		// Paintable objects must all live in a layer called "Paintable"
		layerMask = (1 << LayerMask.NameToLayer("Paintable"));
		// Find the beam object.  It must have the "Beam" tag.
		foreach (Renderer r in rends) {
			if (r.gameObject.CompareTag("Beam")) {
				beamRenderer = r;
				beamRB = r.gameObject.GetComponent<Rigidbody>();
				break;
			}
		}
		if (beamRenderer == null) {
			Debug.Log("StickBehavior did not find a beam object (wrong material set?)");
		}
		makeInvisible();
		makeInactive();
	}

	// Set whether the GameObject is currently active (drawing, pressing buttons, etc)
	public void setActive(bool d)
	{
		isActive = d;
		if (beamRenderer != null) {
			if (d) {
				beamRenderer.material = activeBeamMaterial;
				if (beamRB != null) {
					beamRB.detectCollisions = true;
					beamRB.WakeUp();
				}
			} else {
				beamRenderer.material = inactiveBeamMaterial;
				if (beamRB != null) {
					beamRB.detectCollisions = false;
					beamRB.WakeUp();	
				}
			}
		}
	}

	public void makeActive()
	{
		setActive(true);
	}

	public void makeInactive()
	{
		setActive(false);
	}

	// Set whether the GameObject is currently drilling through paintable
	// objects.  NOT CURRENTLY IMPLEMENTED.
	public void setDrill(bool d)
	{
		drill = d;
	}

	// Make GameObject and its children visible or invisible.
	public void setVisibility(bool b)
	{	
		isVisible = b;
		foreach (Renderer r in rends) {
            r.enabled = b;
        }
    }

    public bool visible()
    {
        return isVisible;
    }

	public void makeInvisible()
	{
		setVisibility(false);
    }

	public void makeVisible() {
		setVisibility(true);
	}

	void Update () {

        OVRInput.Update();

        transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);

        if (isActive) {
			if (drill) {
				PaintAllHits();
			} else {
				PaintFirstHit();
			}
		}
	}

	void PaintAllHits() {
		Debug.Log("PAINTING ALL HITS NOT IMPLEMENTED.");
		PaintFirstHit();
	}

    void PaintFirstHit() {
		// Local "up"
        var raydir = transform.TransformDirection(Vector3.up);
        RaycastHit hit;

		// Cast a ray in direction "up", deteremine what paintable is first hit.
        if (Physics.Raycast (transform.position, raydir, out hit, maxDist, layerMask)) {
			GameObject g = hit.transform.gameObject;

            if (pt != null) {
				// Paint on the shared PaintableTexture at the (u,v) coordinates
				// of the point where the ray met the object.
				pt.PaintUV (g, hit.textureCoord);
			}
		}
	}
}
