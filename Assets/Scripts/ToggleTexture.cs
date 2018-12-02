using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ToggleTexture : MonoBehaviour {
	
	public Material rt;
	private Material current;
	private Renderer r; 
	// Use this for initialization
	void Start () {
		r = gameObject.GetComponent<Renderer>();
		if (r != null) {
			current= r.material;
		}
	}
	
	// Update is called once per frame
	void OnTriggerEnter(){
		r = gameObject.GetComponent<Renderer>(); 
		if(r.material==current){
			GetComponent<Renderer>().material=rt;
		}
		else{
			GetComponent<Renderer>().material=current;
		}
	}
}
