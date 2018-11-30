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

public class StickBehavior : MonoBehaviour
{
    // Might get rid of due to Alex's implementation
    public GameObject laserPointer;
    private LaserOnOff laser;

    // Allows for ability to check if item has been grabbed.
    public OVRGrabbable gb;
    // Removes issues when throwing causes multiple 'fall-throughs' otherwise a new vector is created every drop
    private Vector3 respawn;

    public OVRInput.Controller Controller;

    public float maxDist = Mathf.Infinity;
    private Renderer[] rends;
    private bool isDrawing = false;
    private bool isVisible = false;
    private bool drill = false; // not supported yet
    private PaintableTexture pt;
    private LayerMask layerMask = Physics.DefaultRaycastLayers;
    private LayerMask colorMask = Physics.DefaultRaycastLayers;
    private LayerMask textureColorMask = Physics.DefaultRaycastLayers;

    // For interpolated drawing
    private Vector3 lastDirection;
    private Vector3 lastPosition = new Vector3(float.NaN, float.NaN, float.NaN);

    void Start()
    {
        // Exact spot of the tool platform. Will change when we change where the tool platform goes.
        respawn = new Vector3(-0.71f, 0.25f, -7.09f);
        rends = gameObject.GetComponentsInChildren<Renderer>();
        // Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
        laser = laserPointer.GetComponent<LaserOnOff>();

        // Paintable objects must all live in a layer called "Paintable"
        layerMask = (1 << LayerMask.NameToLayer("Paintable"));
        // colorMask is to get the color from a single-colored material
        colorMask = (1 << LayerMask.NameToLayer("Color"));
        // textureColorMask is for getting a color from a texture
        textureColorMask = (1 << LayerMask.NameToLayer("TextureColor"));
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

    public void stopDrawing()                       // #DIFFERENT
    {
        setDrawing(false);
        lastPosition = new Vector3(float.NaN, float.NaN, float.NaN);
    }

    public bool drawing()
    {
        return isDrawing;                     // #DIFFERENT
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
        foreach (Renderer r in rends)
        {
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

    public void makeVisible()
    {
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
        laser.turnOff();
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
            if (isDrawing)
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
            if (isDrawing)
            {
                this.lastPosition = transform.position;
                this.lastDirection = transform.TransformDirection(Vector3.up);
            }
        }

    }

    void PaintAllHits()
    {
        Debug.Log("PAINTING ALL HITS NOT IMPLEMENTED.");
        PaintFirstHit();
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

    void PaintFirstHit()
    {
        // Local "up"
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