using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputTest : MonoBehaviour {

    public TablePreferencesManager tablePreferencesManager;

    public SteamVR_Controller.Device controller;

    // Use this for initialization
    void Start () {
        controller = tablePreferencesManager.Controller;
	}
	
	// Update is called once per frame
	void Update () {
		if (controller.GetHairTrigger())
        {
            print("Trigger Pressed");
        }
	}
}
