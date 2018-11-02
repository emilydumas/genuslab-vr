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
	private Renderer[] rends;
	private bool isDrawing = false;
	private bool isVisible = true;
	private bool drill = false; // not supported yet
	private PaintableTexture pt;
	private LayerMask layerMask = Physics.DefaultRaycastLayers;

    private Vector3 firstCapture;
    private Vector3 secondCapture;

	
	void Start () {
		rends = gameObject.GetComponentsInChildren<Renderer>();
		// Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
		// Paintable objects must all live in a layer called "Paintable"
		layerMask = (1 << LayerMask.NameToLayer("Paintable"));
        this.firstCapture = null;
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
        
        //If in the last update(tied to frame) we were drawing, we set the first Capture to be last frame's second capture
        if (isDrawing)
        {
            this.firstCapture = this.secondCapture;
        }

        OVRInput.Update();

        transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);



        if (isDrawing) {
            this.secondCapture = transform.localPosition;      //get the 2nd position as soon as we start drawing
			if (drill) {
				PaintAllHits();
			} else {
                if (this.firstCapture.x <= -999)
                {
                    PaintFirstHit();
                }
                else
                {
                    PaintBetween();
                }
			}
            
		}
        if (!isDrawing)
        {
            this.firstCapture.Set(-999, -999, -999);      //as as we are not drawing, we set the first capture to Some Riduculous Number
                                                          //to setup for next capture
        }
	}

    void PaintBetween()
    {
        //take the first and second captures and create a number of raycasts inbetween those two

        float middle = Vector3.Distance(this.secondCapture, this.firstCapture);

        //Gonna use a lerp!
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
