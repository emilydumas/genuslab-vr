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
	private bool isVisible = false;
	private bool drill = false; // not supported yet
	private PaintableTexture pt;
	private LayerMask layerMask = Physics.DefaultRaycastLayers;

<<<<<<< Updated upstream

=======
    private Vector3 firstCapture;
    private Vector3 secondCapture;
>>>>>>> Stashed changes
	
	void Start () {
		rends = gameObject.GetComponentsInChildren<Renderer>();
		// Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
		// Paintable objects must all live in a layer called "Paintable"
		layerMask = (1 << LayerMask.NameToLayer("Paintable"));
        this.firstCapture.Set(-999, -999, -999);
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

<<<<<<< Updated upstream


        OVRInput.Update();

        transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);
        
=======
        //checks if we were drawing in the previous scene
        if (isDrawing)
        {
            this.firstCapture = this.secondCapture;
        }

        //OVRInput.Update();

        //transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        //transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);
>>>>>>> Stashed changes


        if (isDrawing) {
            this.secondCapture = transform.localPosition;
			if (drill) {
				PaintAllHits();
<<<<<<< Updated upstream
			} else {
                PaintFirstHit();
                
=======
>>>>>>> Stashed changes
			}
            else
            {
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
            this.firstCapture.Set(-999, -999, -999);
        }
    }

	void PaintAllHits() {
		Debug.Log("PAINTING ALL HITS NOT IMPLEMENTED.");
		PaintFirstHit();
	}

<<<<<<< Updated upstream
  
=======
    void PaintBetween()
    {
        float dist = Vector3.Distance(this.secondCapture, this.firstCapture);

        float[] interps = new float[8];

        Vector3 positionInterp;
        Vector3 directionInterp;

        interps[0] = (dist * (2 / 10));
        interps[1] = (dist * (3 / 10));
        interps[2] = (dist * (4 / 10));
        interps[3] = (dist * (5 / 10));
        interps[4] = (dist * (6 / 10));
        interps[5] = (dist * (7 / 10));
        interps[6] = (dist * (8 / 10));
        interps[7] = (dist * (9 / 10));


        for (int j = 0; j < 8; j++)
        {
            positionInterp = Vector3.Lerp(this.secondCapture, this.firstCapture, interps[j]);
            directionInterp = Vector3.Slerp(this.secondCapture, this.firstCapture, interps[j]);

            PaintFirstHit(positionInterp, directionInterp);
        }


    }

    void PaintFirstHit(Vector3 positionInterp, Vector3 directionInterp)
    {
        // Local "up"
        

        RaycastHit hit;

        // Cast a ray in direction "up", deteremine what paintable is first hit.
        if (Physics.Raycast(positionInterp, directionInterp, out hit, maxDist, layerMask))
        {
            GameObject g = hit.transform.gameObject;

            if (pt != null)
            {
                // Paint on the shared PaintableTexture at the (u,v) coordinates
                // of the point where the ray met the object.
                pt.PaintUV(g, hit.textureCoord);
            }
        }
    }
>>>>>>> Stashed changes

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
