using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserOnOff : MonoBehaviour {

    public Material laserPink;
    public Material laserDraw;
    public GameObject laser;

    private bool isOn;

    public void turnOn()
    {
        isOn = true;
        GetComponent<Renderer>().material = laserPink;
        laser.SetActive(true);
    }

    public void turnOff()
    {
        isOn = false;
        laser.SetActive(false);
    }

    public void drawColor()
    {
        GetComponent<Renderer>().material = laserDraw;
    }

    public void OnOff()
    {
        if (isOn)
            turnOff();
        else
            turnOn();
    }

    public bool isLaserOn()
    {
        return isOn;
    }


    
}
