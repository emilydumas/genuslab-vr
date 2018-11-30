using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreRendering : MonoBehaviour {
	private RenderTexture rt;
	public GameObject q;
	//public Material mat;
	//public Renderer rend;
	//public Mesh mesh;
	// Use this for initialization
	void Start () {
		rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
		rt.Create();
		//this.GetComponent<Camera>().targetTexture = rt;
		q.GetComponent<Renderer>().material.mainTexture = rt;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

/* In the OnPreRender function you'll want to accomplish the drawing to rendertextures using Graphics.DrawMeshNow. 
You'll need to set the material with mat.SetPass on an appropriate material object so that this graphics call uses one of 
the depth sorting shaders you'll be developing.
I think this whole setup is a little complex and would be easier to develop if there were some kind of intermediate milestones. 
Here are some suggestions:

First, see if you can get an OnPreRender pass to just draw anything to a rendertexture. 
(To debug that I'd make a quad that uses the RT as its main texture. 
Then I could tell if any drawing was happening because it would appear on the quad.) */
	void OnPreRender(){
		//on first pass
		q.GetComponent<Renderer>().material.SetPass(0);
		Graphics.DrawMeshNow(q.GetComponent<MeshFilter>().mesh, q.transform.position, q.transform.rotation);
		//q.GetComponent<Renderer>().material.SetTexture("hello", rt);
	}
}
