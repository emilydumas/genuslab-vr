﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickBehavior : MonoBehaviour {
	public float maxDist = Mathf.Infinity;
	private Renderer[] rends;
	private bool isDrawing = false;
	private bool isVisible = true;
	private bool drill = false; // not supported yet

	void Start () {
		rends = gameObject.GetComponentsInChildren<Renderer>();
		makeInvisible();
	}

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

	public void setDrill(bool d)
	{
		drill = d;
	}

	public void setVisibility(bool b)
	{	
		isVisible = b;
		foreach (Renderer r in rends) {
            r.enabled = b;
        }
    }

	public void makeInvisible()
	{
		setVisibility(false);
    }

	public void makeVisible() {
		setVisibility(true);
	}

	void Update () {
		if (isDrawing) {
			if (drill) {
				PaintAllHits();
			} else {
				PaintFirstHit();
			}
		}
	}

	void PaintAllHits() {
		Debug.Log("PAINTING ALL HITS NOT IMPLEMENTED.");
		PaintFirstHit();
	}

    void PaintFirstHit() {
        var raydir = transform.TransformDirection(Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast (transform.position, raydir, out hit, maxDist)) {
			GameObject g = hit.transform.gameObject;
			g2paintable p = g.GetComponent<g2paintable> ();
			if (p != null) {
				p.PaintUV (hit.textureCoord);
			}
		}
	}
}