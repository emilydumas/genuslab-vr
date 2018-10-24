using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour {
	public GameObject h2view;
	private h2viewcontrol h2c;
	private static EventHandler eventHandler;
	private PaintableTexture pt = null;
	public float h2speed = 3f;

	// Singleton!
	public static EventHandler instance
	{
		get
		{
			if (!eventHandler)
			{
				eventHandler = FindObjectOfType (typeof (EventHandler)) as EventHandler;

				if (!eventHandler)
				{
					Debug.LogError ("There needs to be one active EventManger script on a GameObject.");
				}
				else
				{
					eventHandler.Start(); 
				}
			}

			return eventHandler;
		}
	}

	// Use this for initialization
	void Start () {
		h2c = h2view.GetComponent<h2viewcontrol>();
		pt = PaintableTexture.Instance;
	}
	
	// Update is called once per frame
	public void handleEvent(string eventName){
		if (eventName=="Toggle"){
			instance.Toggle();
		}
		if(eventName=="Clear"){
			instance.Clear();
		}
		if(eventName=="Reset"){
			instance.Reset();
		}
		
	}

	void Toggle(){
			h2c.Toggle();
			h2c.ExportMode();
			print("the");
	}

	void Clear(){
		pt.Clear();
	}

	public void Move(string eventName){
		
		float dt = Time.deltaTime;
		if (eventName=="MoveDown") {
			h2c.ComposePreTransformation(HypUtil.BoostY(-h2speed*dt));
			h2c.ExportPreTransformation();
		}

		if (eventName=="MoveUp") {
			h2c.ComposePreTransformation(HypUtil.BoostY(h2speed*dt));
			h2c.ExportPreTransformation();
		}
		if (eventName=="MoveLeft") {
			h2c.ComposePreTransformation(HypUtil.BoostX(h2speed*dt));
			h2c.ExportPreTransformation();
		}
		if (eventName=="MoveRight") {
			h2c.ComposePreTransformation(HypUtil.BoostX(-h2speed*dt));
			h2c.ExportPreTransformation();
		}
	}

	void Reset(){
			h2c.ResetPreTransformation();
			h2c.ExportPreTransformation();
		
	}
}
