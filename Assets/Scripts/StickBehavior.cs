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
    public float maxDist = Mathf.Infinity;
    private Renderer[] rends;
    private bool isVR;
    private bool checkColor = false;
    private bool isDrawing = false;
    private bool isVisible = false;
    private bool drill = false; // not supported yet
    private PaintableTexture pt;
    private LayerMask layerMask = Physics.DefaultRaycastLayers;
    private LayerMask colorMask = Physics.DefaultRaycastLayers;
    private LayerMask textureColorMask = Physics.DefaultRaycastLayers;
    


    private Vector3 lastDirection;
    private Vector3 lastPosition = new Vector3(float.NaN,float.NaN,float.NaN);

    void Start()
    {
        rends = gameObject.GetComponentsInChildren<Renderer>();
        // Retrieve the one and only instance of PaintableTexture (singleton pattern)
        pt = PaintableTexture.Instance;
        // Paintable objects must all live in a layer called "Paintable"
        layerMask = (1 << LayerMask.NameToLayer("Paintable"));
        colorMask = (1 << LayerMask.NameToLayer("Color"));
        textureColorMask = (1 << LayerMask.NameToLayer("TextureColor"));
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
            setVR();
        else
            setMouseAndKeyboard();
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

    public void setVR()
    {
        isVR = true;
    }

    public void setMouseAndKeyboard()
    {
        isVR = false;
    }

    public void findColor()
    {
        checkColor = true;
    }

    public void stopColorSearch()
    {
        checkColor = false;
    }

    void Update()
    {

        
        //Defaults to using mouse and keyboard if no Right VR controller is present at start()
        if (isVR)
        {
            OVRInput.Update();
            transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);
            transform.localRotation = OVRInput.GetLocalControllerRotation(Controller) * Quaternion.Euler(90, 0, 0);
        }

        
        if (checkColor)
        {
            setColor();
        }

        if (isDrawing)
            {
            if (drill)
            {
                PaintAllHits();
            }
            else
            {
//              PaintFirstHit();
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
        int numSteps = 0;

        // fixme! 0.1f is GREAT on the MCL computer, the Titan X. 
        // My laptop's GT 1030 doesn't hold up with erratic movement
        // Find a happy medium
        float maxStep = 0.4f;   

        if (HaveLastPosition())
        {
            Vector3 curDirection = transform.TransformDirection(Vector3.up);

            float spaceDist = Vector3.Distance(lastPosition, transform.position);
            float angleDist = Vector3.Angle(lastDirection, curDirection);

            numSteps =  Mathf.CeilToInt((spaceDist + angleDist) / maxStep);

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
            Debug.Log("hit!");
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

            Debug.Log("COLOR");
            GameObject g = hit.transform.gameObject;

            Color c = new Color();
            c = g.GetComponent<Renderer>().material.color;

            pt.SetDrawingColor(c);
          
        }
        if (Physics.Raycast(transform.position, raydir, out hit, maxDist, textureColorMask))
        {

            Debug.Log("TEXTURECOLOR");
            
            Texture2D tex = hit.transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
            Vector2 uvCoord = hit.textureCoord;

            // Allowing the uv Coordinates to work with the texture.
            uvCoord.x *= tex.width;
            uvCoord.y *= tex.height;
           

            Color c = new Color();

            // GetPixelBilinear SHOULD work without casting, 
            // but it creates odd color approximations.
            // Stick with GetPixel
            c = tex.GetPixel((int)uvCoord.x, (int)uvCoord.y);       

            pt.SetDrawingColor(c);

        }

    }

}