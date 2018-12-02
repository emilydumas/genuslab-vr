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

    // Allows for ability to check if item has been grabbed.
    public OVRGrabbable gb;
    // Removes issues when throwing causes multiple 'fall-throughs' otherwise a new vector is created every drop
    private Vector3 respawn;

    // #GETRIDOF Might get rid of due to Alex's implementation
    //public GameObject laserPointer;
    //private LaserOnOff laser;

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
    private LayerMask colorMask = Physics.DefaultRaycastLayers;
    private LayerMask textureColorMask = Physics.DefaultRaycastLayers;
    private float maxForward = 10.0f;

    // For interpolated drawing
    private Vector3 lastDirection;
    private Vector3 lastPosition = new Vector3(float.NaN, float.NaN, float.NaN);

    void Start () {
        // Exact spot of the tool platform. Will change when we change where the tool platform goes.
        respawn = new Vector3(-0.71f, 0.25f, -7.09f);
        rends = gameObject.GetComponentsInChildren<Renderer>();
		// Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
		// Paintable objects must all live in a layer called "Paintable"
		layerMask = (1 << LayerMask.NameToLayer("Paintable"));
        // colorMask is to get the color from a single-colored material
        colorMask = (1 << LayerMask.NameToLayer("Color"));
        // textureColorMask is for getting a color from a texture
        textureColorMask = (1 << LayerMask.NameToLayer("TextureColor"));
        if (maxDist < maxForward)
			maxForward = maxDist;
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
        if (!d) {
            lastPosition = new Vector3(float.NaN, float.NaN, float.NaN);
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

    public bool isGrabbed()
    {
        return gb.isGrabbed;
    }

    // Respawns the controller if it falls to the floor.
    // Quick-and-dirty method that is less prone to bugs than utilizing an onTriggerEnter for the floor
    public void respawnLaser()
    {
        Rigidbody body = gameObject.GetComponent<Rigidbody>();
        body.isKinematic = true;
        transform.position = respawn;
        body.isKinematic = false;
       // laser.turnOff();
    }


    void Update()
    {
        // If the laser pointer falls down to a certain point, we respawn it at the original point on the map.
        if ((transform.position.y < -2.6f) && (!isGrabbed()))
        {
            respawnLaser();
        }

        // Necessary for getting input from the touch controllers
        OVRInput.Update();
        // Keeping as legacy from laser pointer as hand
        // transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        // transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);

        if (isGrabbed())
        {
            // DO NOT CHANGE 
            // The way the laser pointer was made doesn't allow for the usual 'snap offset'
            Vector3 handSnap = new Vector3(-0.58f, 0.96f, -7.76f);
            transform.localPosition = OVRInput.GetLocalControllerPosition(Controller) + handSnap;
            transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);
            if (isActive)
            {
                if (drill)
                {
                    PaintAllHits();
                }
                else
                {
                    PaintWithInterpolation();
                }
            }

            // checks if we were drawing in the previous scene
            if (isActive)
            {
                this.lastPosition = transform.position;
                this.lastDirection = transform.TransformDirection(Vector3.up);
            }
        }

    }

    void PaintAllHits()
    {
        if (pt == null)
            return;


        Vector3 raydir = transform.TransformDirection(Vector3.up);
        Vector3 pos = transform.position;
        RaycastHit hit;

        List<Vector3> hitList = new List<Vector3>();
        GameObject gObject = null;

        // Forward pass: record hits (ray enters surface) and paint them
        while (Physics.Raycast(pos, raydir, out hit, maxDist, layerMask))
        {
            GameObject g = hit.transform.gameObject;
            gObject = g;
            pt.PaintUV(g, hit.textureCoord);
            hitList.Add(hit.point);
            pos = hit.point + 0.001f * raydir; // move slightly forward of latest hit
        }

        // Backward pass: Detect surface exits and paint them
        if (hitList.Count != 0)
        {
            Vector3 backHitStart = hitList[hitList.Count - 1] + (maxForward * raydir);
            hitList.Add(backHitStart);
            hitList.Reverse();
            for (int i = 0; i < hitList.Count; i++)
            {
                if (Physics.Raycast(hitList[i], (-1 * raydir), out hit, maxDist, layerMask))
                {
                    pt.PaintUV(gObject, hit.textureCoord);
                }
            }
        }
    }

    bool HaveLastPosition()
    {
        return !(float.IsNaN(lastPosition.x) || float.IsNaN(lastPosition.y) || float.IsNaN(lastPosition.z));
    }

    void PaintWithInterpolation()
    {
        //threshold for where linear interpolation doesn't affect performance.
        //Still see some spotting if drawing on edges of the H2View
        float maxStep = 0.2f;

        if (HaveLastPosition())
        {
            Vector3 curDirection = transform.TransformDirection(Vector3.up);

            float spaceDist = Vector3.Distance(lastPosition, transform.position);
            float angleDist = Vector3.Angle(lastDirection, curDirection);

            float numSteps = Mathf.Ceil((spaceDist + angleDist) / maxStep);

            for (int j = 1; j <= numSteps; j++)
            {
                var pos = Vector3.Lerp(lastPosition, transform.position, (float)j / (float)numSteps);
                var dir = Vector3.Slerp(lastDirection, curDirection, (float)j / (float)numSteps);
                PaintRay(pos, dir);
            }
        }
        else
        {
            PaintFirstHit();
        }
    }

    void PaintRay(Vector3 pos, Vector3 dir)
    {
        RaycastHit hit;

        // Cast a ray in direction "up", determine what paintable is first hit.
        if (Physics.Raycast(pos, dir, out hit, maxDist, layerMask))
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



    void PaintFirstHit() {

        var raydir = transform.TransformDirection(Vector3.up);
        PaintRay(transform.position, raydir);

    }

    public void setColor()
    {
        RaycastHit hit;
        var raydir = transform.TransformDirection(Vector3.up);
        if (Physics.Raycast(transform.position, raydir, out hit, maxDist, colorMask))
        {

            GameObject g = hit.transform.gameObject;

            Color c = g.GetComponent<Renderer>().material.color;

            pt.SetDrawingColor(c);

        }
        if (Physics.Raycast(transform.position, raydir, out hit, maxDist, textureColorMask))
        {
            // HAVE to use the texture as a 2D texture for better normalized (u,v) coordinate of texture
            Texture2D tex = hit.transform.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            Vector2 uvCoord = hit.textureCoord;

            // normalizing coordinates to the texture
            uvCoord.x *= tex.width;
            uvCoord.y *= tex.height;

            Color c = tex.GetPixel((int)uvCoord.x, (int)uvCoord.y);

            pt.SetDrawingColor(c);

        }

    }

}

