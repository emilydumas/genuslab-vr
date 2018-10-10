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
	public float maxDist = Mathf.Infinity;
	private Renderer[] rends;
	private bool isDrawing = false;
	private bool isVisible = true;
	private bool drill = true;
	private PaintableTexture pt;
	private LayerMask layerMask = Physics.DefaultRaycastLayers;
	
	void Start () {
		rends = gameObject.GetComponentsInChildren<Renderer>();
		// Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
		// Paintable objects must all live in a layer called "Paintable"
		layerMask = (1 << LayerMask.NameToLayer("Paintable"));
		makeInvisible();
	}

	// Set whether the GameObject is currently drawing.
	public void setDrawing(bool d)
	{
		isDrawing = d;
	}

	public void startDrawing()
	{
		setDrawing(true);
	}

	public void stopDrawing()
	{
		setDrawing(false);
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

	public void makeInvisible()
	{
		setVisibility(false);
    }

	public void makeVisible() {
		setVisibility(true);
	}

	void Update () {
		if (isDrawing) {
			if (drill) {
				PaintAllHits();
			} else {
				PaintFirstHit();
			}
		}
	}

	void PaintAllHits() {
		if (pt == null)
			return;

        Vector3 raydir = transform.TransformDirection(Vector3.up);
		Vector3 pos = transform.position;
        RaycastHit hit;

		while (Physics.Raycast (pos, raydir, out hit, maxDist, layerMask)) {
			GameObject g = hit.transform.gameObject;
			pt.PaintUV (g, hit.textureCoord);

			pos = hit.point + 0.001f*raydir; // move slightly forward of latest hit
		}
	}

    void PaintFirstHit() {
		// Local "up"
        var raydir = transform.TransformDirection(Vector3.up);
        RaycastHit hit;

		if (pt == null)
			return;

		// Cast a ray in direction "up", deteremine what paintable is first hit.
        if (Physics.Raycast (transform.position, raydir, out hit, maxDist, layerMask)) {
			GameObject g = hit.transform.gameObject;
			pt.PaintUV (g, hit.textureCoord);
		}
	}
}
