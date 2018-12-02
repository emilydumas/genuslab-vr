using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleView : MonoBehaviour {
	public List<Material> options;
	int current=1;
	private Renderer r; 

	void OnTriggerEnter(){
		NextTexture();
	}
	public void SetTexture(int idx) {
        current = idx;
        GetComponent<Renderer>().material=options[current];
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
