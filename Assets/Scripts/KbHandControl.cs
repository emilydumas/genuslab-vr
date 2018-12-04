using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KbHandControl : MonoBehaviour {

    public OVRInput.Controller Controller;

    public GameObject camera;
    public GameObject stickHolder;
    public GameObject surface;
    public GameObject h2view;
    public float h2speed = 3f;
    private StickBehavior sb;
    private h2viewcontrol h2c;
    private Quaternion stickInitQ, cameraInitQ, surfaceInitQ;
    private PaintableTexture pt = null;
    private HelpScreen helpScreen;
    private Vector3 surfaceDelta;



    void Start()
    {
        pt = PaintableTexture.Instance;
        sb = stickHolder.GetComponent<StickBehavior>();
        h2c = h2view.GetComponent<h2viewcontrol>();
        stickInitQ = stickHolder.transform.localRotation;
        cameraInitQ = camera.transform.localRotation;
        surfaceInitQ = surface.transform.localRotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void doQuit()
    {
        // Exit the application (builds) or stop the player (editor)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void OnApplicationQuit()
    {
        Cursor.lockState = CursorLockMode.None;
    }



    void Update()
    {
        float dt = Time.deltaTime;

        // CTRL-Q on keyboard quits the application
        // (We expect that quit will sometimes be done after headset removal.)
        // TODO: Make a good way to quit from within VR scene.
        if  ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q)) {
			doQuit();
		}

        if (!sb.visible())
        {
            sb.makeVisible();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetKeyDown(KeyCode.Y))
        {
            pt.NextTexture();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.H))
        {
            pt.PreviousTexture();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            pt.Clear();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            // Klein-Poincare toggle
            h2c.ToggleModel();
            h2c.ExportMode();
        }

        // Laser pointer controls that only work if it is grabbed by the correct hand.
        if (sb.isGrabbed()) {
            if (sb.whichGrabbed() == "right")
            {
                if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                {
                    sb.makeActive();    // This setup gave tighter, more responsive controls.
                    sb.setColor();      // Toggling the drill and changing color were
                                        // under-responsive when executing with GetUp()
                }
                else if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
                {
                    sb.makeActive();
                }
                else
                {
                    sb.makeInactive();
                }
            }
            else if (sb.whichGrabbed() == "left")       
            {
                if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
                {
                    sb.makeActive();
                    sb.setColor();

                }
                else if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
                {
                    sb.makeActive();
                }
                else
                {
                    sb.makeInactive();
                }
            }
        }
    }
}
