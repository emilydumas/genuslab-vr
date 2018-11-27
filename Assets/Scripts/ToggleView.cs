using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleView : MonoBehaviour {
	public List<Material> options;
	int current=1;
	private Renderer r; 
	// Use this for initialization
	
	
	// Update is called once per frame
	void Update(){
		GetComponent<Renderer>().material=options[current];
	}
	void OnTriggerEnter(){
		NextTexture();
	}
	public void SetTexture(int idx) {
        current = idx;
    }

    public void NextTexture() {
        SetTexture((current + 1) % options.Count);
    }
    public void PreviousTexture() {
        if (current > 0) {
            SetTexture(current - 1);
        } else {
            SetTexture(options.Count-1);
        }
    }
}
