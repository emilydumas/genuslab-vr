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

    private Vector2 paint1;
    private Vector2 paint2;
	
	void Start () {
		rends = gameObject.GetComponentsInChildren<Renderer>();
		// Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
		// Paintable objects must all live in a layer called "Paintable"
		layerMask = (1 << LayerMask.NameToLayer("Paintable"));
		makeInvisible();
        this.paint1.Set(0.0f, 0.0f);
        this.paint2.Set(0.0f, 0.0f);
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



        OVRInput.Update();

        transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);
        if (this.paint2.x != 0.0f || this.paint2.y != 0.0f) {
            this.paint1 = this.paint2;
        }
        


        if (isDrawing) {
			if (drill) {
				PaintAllHits();
			} else {
                GameObject g = PaintFirstHit();
                paintBetweenPoints(g);
			}
		}
	}

	void PaintAllHits() {
		Debug.Log("PAINTING ALL HITS NOT IMPLEMENTED.");
		PaintFirstHit();
	}

    void paintBetweenPoints(GameObject g)
    {

        //get the slope between x & y
        float M = (this.paint2.y - this.paint1.y) / (this.paint2.x - this.paint1.x);
        //absolute value so it doesn't matter which is greater
        Mathf.Abs(M);
        float b = 0.0f;

        if (paint1.x >= paint2.x)
        {
            b = paint2.x;
        }
        else if(paint2.x > paint1.x)
        {
            b = paint1.x;
        }

        while (Vector2.Distance(this.paint1, paint2) != 0.0f)
        {
            
            if(this.paint2.x > this.paint1.x)
            {
                paint1.Set(paint1.x + 0.001f, paint1.x * M + b);
                pt.PaintUV(g, paint1);
            }
            else if (this.paint1.x > this.paint2.x)
            {
                paint2.Set(paint2.x + 0.001f, paint2.x * M + b);
                pt.PaintUV(g, paint2);
            }
            else
            {
                if (paint2.y > paint1.y)
                {
                    paint1.Set(paint1.x, paint1.y + 0.001f);
                    pt.PaintUV(g, paint1);
                }
                else
                {
                    paint2.Set(paint2.x, paint2.y + 0.001f);
                    pt.PaintUV(g, paint2);
                }
            }

        }
    }

    GameObject PaintFirstHit() {
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
                this.paint2 = hit.textureCoord;
                return g;
			}
            return null;
		}
        return null;
	}
}
