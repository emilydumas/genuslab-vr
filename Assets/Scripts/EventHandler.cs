using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour {


	private Dictionary <string, UnityEvent> eventDictionary;

	private static  EventHandler eventHandler;
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
		if(eventDictionary == null){
			eventDictionary = new Dictionary<string,UnityEvent>();
		}
	}
	
    public static void StartListening (string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.AddListener (listener);
        } 
        else
        {
            thisEvent = new UnityEvent ();
            thisEvent.AddListener (listener);
            instance.eventDictionary.Add (eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, UnityAction listener)
    {
        if (eventHandler == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.RemoveListener (listener);
        }
    }

    public static void TriggerEvent (string eventName)
    {
        UnityEvent thisEvent = null;
        Debug.Log("Event: "+eventName);
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke ();
        }
    }
}