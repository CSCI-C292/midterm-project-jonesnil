using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    Button advanceDayButton;

    List<Task> taskHolder;

    private void Start()
    {
        advanceDayButton = transform.GetChild(0).GetComponent<Button>();
        GameEvents.BuildingClicked += OnBuildingClicked;
        GameEvents.BuildingUIClosing += OnBuildingUIClosing;

        taskHolder = new List<Task>();

        GameEvents.TaskStarted += OnTaskStarted;
        GameEvents.TaskCompleted += OnTaskCompleted;
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

    void OnTaskStarted(object sender, TaskEventArgs args) 
    {
        taskHolder.Add(args.taskPayload);
    }

    void OnTaskCompleted(object sender, TaskEventArgs args) 
    {
        taskHolder.Remove(args.taskPayload);
    }
}
