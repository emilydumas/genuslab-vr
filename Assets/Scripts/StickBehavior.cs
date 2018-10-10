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
	private bool drill = false; // not supported yet
	private bool burn = true; //set true for testing
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
	public void setBurnThrough(bool b)
	{
		burn = b;
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
			} else if (burn){
				PaintBurnHits();
			}else {
				PaintFirstHit();
			}
		}
	}

	void PaintAllHits() {
		Debug.Log("PAINTING ALL HITS NOT IMPLEMENTED.");
		PaintFirstHit();
	}

	void PaintBurnHits() {
		//var raydir = transform.TransformDirection(Vector3.up);
        var raydir = transform.TransformDirection(Vector3.up);
		RaycastHit hit;
		//Debug.Log(raydir);
		var reverseRayDir = new Vector3(raydir.x, raydir.y, -1 * raydir.z);
		//Debug.Log(reverseRayDir);
		RaycastHit hitReverse;

		Debug.Log("raydir: "+ raydir + "reverse raydir: " + reverseRayDir);

		// Cast a ray in direction "up", deteremine what paintable is first hit.
        if (Physics.Raycast (transform.position, raydir, out hit, maxDist, layerMask)) {
			GameObject g = hit.transform.gameObject;

            if (pt != null) {
				pt.PaintUV(g, hit.textureCoord);
				Vector3 hitReverseOrigin = new Vector3(transform.position.x, transform.position.y, (transform.position.z + 1) * -1); 
				Debug.Log("original origin: " +  transform.position + "reverse origin: " + hitReverseOrigin);
				Physics.Raycast(hitReverseOrigin, reverseRayDir, out hitReverse, maxDist, layerMask);
				Debug.Log("original point: " + transform.position + "reversehit origin point" + hitReverseOrigin);
				pt.PaintUV(g, hitReverse.textureCoord);
				
			}
		}
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
