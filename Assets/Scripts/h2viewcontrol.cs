// Script companion to the hypview shader which exposes the Poincare/Klein
// shader properties as public member functions.

// NOTE: Both the hypview shader (for drawing H^2 tiling) and the h2paint shader
// (for adding a spot to the H^2 tiling) need to be set to the same mode at all
// times!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewMode { Poincare, Klein };

public class h2viewcontrol : MonoBehaviour {
	public ViewMode viewMode;
	private Matrix4x4 PreTransformation = Matrix4x4.identity;
	private Material m = null;  // Target material with hypview shader
	private Material pm = null;  // Target material with h2paint shader
	public float h2speed = 3f;


	void Start () {
		
		Renderer r = gameObject.GetComponent<Renderer>(); 
		if (r != null) {
			m = r.material;
		}

		PaintData pd = gameObject.GetComponent<PaintData>();
		pm = pd.paintMaterial;
		EventHandler.StartListening("ToggleModel", ToggleModel);
		EventHandler.StartListening("MoveUp", MoveUp);
		EventHandler.StartListening("MoveDown", MoveDown);
		EventHandler.StartListening("MoveRight", MoveRight);
		EventHandler.StartListening("MoveLeft", MoveLeft);
		EventHandler.StartListening("Reset",ResetPreTransformation);
		ExportMode();
		ExportPreTransformation();	
	}

	public void ToggleModel() {
		if (viewMode == ViewMode.Poincare) {
			viewMode = ViewMode.Klein;
		} else {
			viewMode = ViewMode.Poincare;
		}
		ExportMode();
	}

	public void ExportMode() {
		// Set the shader property to the current value of viewMode.
		int p;

		if (viewMode == ViewMode.Poincare) {
			p = 1;
		} else {
			p = 0;
		}
		if (m != null) {
			m.SetInt("_Poincare",p);
		}
		if (pm != null) {
			pm.SetInt("_Poincare",p);
		}

	}

	public void ResetPreTransformation() {
		SetPreTransformation(Matrix4x4.identity);
		ExportPreTransformation();
	}

	public void SetPreTransformation(Matrix4x4 T) {
		PreTransformation = T;
	}

	public void ComposePreTransformation(Matrix4x4 T) {
		PreTransformation = PreTransformation * T;
	}

	public void ExportPreTransformation() {
		if (m != null) {
			m.SetMatrix("_PreTransformation",PreTransformation);
		}
		if (pm != null) {
			pm.SetMatrix("_PreTransformation",PreTransformation);
		}
	}

	public void MoveUp() {
			float dt = Time.deltaTime;
			ComposePreTransformation(HypUtil.BoostY(-h2speed*dt));
			ExportPreTransformation();
		}

	public void MoveDown() {
			float dt = Time.deltaTime;
			ComposePreTransformation(HypUtil.BoostY(h2speed*dt));
			ExportPreTransformation();
		}
	public void MoveLeft() {
			float dt = Time.deltaTime;	
			ComposePreTransformation(HypUtil.BoostX(h2speed*dt));
			ExportPreTransformation();
		}
	public void MoveRight() {
			float dt = Time.deltaTime;
			ComposePreTransformation(HypUtil.BoostX(-h2speed*dt));
			ExportPreTransformation();
		}
}
