using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ssoitControl : MonoBehaviour {
	public GameObject[] texpreviews;
	public Material depthmat;
	public Material ssoitMaterial;
	private RenderTexture[] db;
	private Mesh m;
	private CommandBuffer cb;
	private MaterialPropertyBlock mpb;
	private int passes = 4;

	void Start () {
		m = gameObject.GetComponent<MeshFilter>().sharedMesh;
		if (m == null) {
			Debug.Log("Did not find a mesh.");
		}
		db = new RenderTexture[passes];

		// Activate display of the depth buffers on other objects
		// (Would disable this in production)
		for (int i=0; i<passes; i++) {
			db[i] = new RenderTexture( Screen.width, Screen.height, 32, RenderTextureFormat.RFloat);
  			// MUST disable interpolation otherwise depth values for a given pixel will not
			// match perfectly, due to imprecision when pixel space converted to UV and back
			// FilterMode.Point means "no interpolation".
			db[i].filterMode = FilterMode.Point;
			if ((i < texpreviews.Length) && (texpreviews[i] != null)) {
				Renderer rend = texpreviews[i].GetComponent<Renderer>();
				if (rend != null)
					rend.material.SetTexture("_MainTex",db[i]);
			}
		}

		// Tell the SSOIT shader about the depth textures we've prepared.
		for (int i=0; i<passes; i++) {
			ssoitMaterial.SetTexture("_Z" + (i+1).ToString(),db[i]);
		}

		// Activate the SSOIT material (replacing some other material that was there
		// so that the object would be visible in the editor)
		gameObject.GetComponent<Renderer>().material = ssoitMaterial;

		// Setup the pre-rendering steps needed for SSOIT
		mpb = new MaterialPropertyBlock();
		cb = new CommandBuffer();
		cb.name = "ZSort";
		Camera.main.AddCommandBuffer(CameraEvent.BeforeDepthTexture,cb);
	}

	void OnWillRenderObject() {
		cb.Clear();
		mpb.Clear();
		for (int i=0; i<passes; i++) {
			cb.SetRenderTarget(db[i]);
			cb.ClearRenderTarget(true,true,new Vector4(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity,Mathf.Infinity));
			cb.DrawMesh(m,transform.localToWorldMatrix,depthmat,0,0,mpb);
			mpb.SetTexture("_MinDepthTexture",db[i]);
		}
	}
}
