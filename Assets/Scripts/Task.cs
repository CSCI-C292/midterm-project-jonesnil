using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    public int durationTimer;
    public TaskType type;
    public Colonist colonist;
    public Building building;

    public Task(TaskType inputType, Building inputBuilding) 
    {
        type = inputType;
        building = inputBuilding;

        SetDurationTimer();

        GameEvents.DayAdvanced += OnDayAdvanced;
    }

    void SetDurationTimer() 
    {
        switch (type) 
        {
            case TaskType.Kill:
                durationTimer = 2;
                break;
        }
    }

    void OnDayAdvanced(object sender, EventArgs args) 
    {
        Debug.Log(durationTimer);
        durationTimer -= 1;
        if (durationTimer == 0)
            ResolveTask();
    }

    void ResolveTask() 
    {
        switch (type) 
        {
            case TaskType.Kill:
                building.robotCount = 0;
                break;
        }
    }
}
