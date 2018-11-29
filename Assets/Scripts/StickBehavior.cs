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
    
    public OVRInput.Controller Controller;
    public OVRGrabbable gb;
    public GameObject laserPointer;
    public float maxDist = Mathf.Infinity;
    private Renderer[] rends;
    private bool isDrawing = false;
    private bool isVisible = false;
    private bool drill = false; // not supported yet
    private Vector3 respawn;
    private LaserOnOff laser;
    private PaintableTexture pt;
    private LayerMask layerMask = Physics.DefaultRaycastLayers;
    private LayerMask colorMask = Physics.DefaultRaycastLayers;
    private LayerMask textureColorMask = Physics.DefaultRaycastLayers;

    private Vector3 lastDirection;
    private Vector3 lastPosition = new Vector3(float.NaN,float.NaN,float.NaN);

    void Start()
    {
        respawn = new Vector3(-0.71f, 0.25f, -7.09f);
        rends = gameObject.GetComponentsInChildren<Renderer>();
        // Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
        laser = laserPointer.GetComponent<LaserOnOff>();

        // Paintable objects must all live in a layer called "Paintable"
        layerMask = (1 << LayerMask.NameToLayer("Paintable"));
        colorMask = (1 << LayerMask.NameToLayer("Color"));
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

    public void stopDrawing()
    {
        setDrawing(false);
        lastPosition = new Vector3(float.NaN, float.NaN, float.NaN);
    }

    public bool drawing()
    {
        return isDrawing;
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


    void Update()
    {

        if (transform.position.y < -2.6f)
        {
            Rigidbody body = gameObject.GetComponent<Rigidbody>();
            body.isKinematic = true;
            transform.position = respawn;
            body.isKinematic = false;
            laser.turnOff();
        }
        
        OVRInput.Update();
        //Keeping as legacy from laser pointer as hand
        //transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
        //transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);

        if (gb.isGrabbed == true)
        { 
            // DO NOT CHANGE 
            // The way the laser pointer was made turned the 'snap;
            Vector3 thing = new Vector3(-0.58f, 0.96f, -7.76f);
            transform.localPosition = OVRInput.GetLocalControllerPosition(Controller) + thing;
            transform.localRotation = OVRInput.GetLocalControllerRotation(Controller);
            transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);
            if (isDrawing)
            {
                if (drill)
                {
                    PaintAllHits();
                }
                else
                {
                    //PaintFirstHit();
                    PaintWithInterpolation();
                }
            }

            //checks if we were drawing in the previous scene
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
        } else {
            PaintFirstHit();
        }
    }

    void PaintRay(Vector3 pos, Vector3 dir)
    {
        RaycastHit hit;

        // Cast a ray in direction "up", deteremine what paintable is first hit.
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


            Texture2D tex = hit.transform.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            Vector2 uvCoord = hit.textureCoord;

            // converting world coordinates to corresponding coordinates on the texture
            uvCoord.x *= tex.width;
            uvCoord.y *= tex.height;

            // GetPixelBilinear SHOULD work without casting, 
            // but it is weirdly inconsistent?
            // Stick with GetPixel
            Color c = tex.GetPixel((int)uvCoord.x, (int)uvCoord.y);

            pt.SetDrawingColor(c);

        }

    }

}