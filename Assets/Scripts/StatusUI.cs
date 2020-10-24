using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    Button advanceDayButton;


    private void Start()
    {
        advanceDayButton = transform.GetChild(0).GetComponent<Button>();
        GameEvents.BuildingClicked += OnBuildingClicked;
        GameEvents.BuildingUIClosing += OnBuildingUIClosing;

    }


    void OnBuildingClicked(object sender, BuildingEventArgs args) 
    {
        advanceDayButton.interactable = false;
    }

    void OnBuildingUIClosing(object sender, EventArgs args) 
    {
        advanceDayButton.interactable = true;
    }

    public void AdvanceDay() 
    {
        GameEvents.InvokeDayAdvanced();
    }
}
