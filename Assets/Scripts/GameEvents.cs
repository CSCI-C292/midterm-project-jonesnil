using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingEventArgs : EventArgs
{
    public Building buildingPayload;
}

public class TaskEventArgs : EventArgs
{
    public Task taskPayload;
}

public static class GameEvents
{

    public static event EventHandler<BuildingEventArgs> BuildingClicked;
    public static event EventHandler BuildingUIClosing;
    public static event EventHandler<BuildingEventArgs> BuildingReclaimed;
    public static event EventHandler<TaskEventArgs> TaskUIStarted;
    public static event EventHandler TaskUIClosing;
    public static event EventHandler DayAdvanced;
    public static event EventHandler<TaskEventArgs> TaskStarted;
    public static event EventHandler<TaskEventArgs> TaskCompleted;
    public static event EventHandler AddColonist;

    public static void InvokeBuildingClicked(Building building)
    {
        BuildingClicked(null, new BuildingEventArgs { buildingPayload = building });
    }

    public static void InvokeBuildingUIOver()
    {
        BuildingUIClosing(null, EventArgs.Empty);
    }

    public static void InvokeBuildingReclaimed(Building building)
    {
        BuildingReclaimed(null, new BuildingEventArgs { buildingPayload = building });
    }

    public static void InvokeTaskUIStarted(Task task)
    {
        TaskUIStarted(null, new TaskEventArgs { taskPayload = task });
    }

    public static void InvokeTaskUIClosing()
    {
        TaskUIClosing(null, EventArgs.Empty);
    }

    public static void InvokeDayAdvanced()
    {
        DayAdvanced(null, EventArgs.Empty);
    }

    public static void InvokeTaskStarted(Task task)
    {
        TaskStarted(null, new TaskEventArgs { taskPayload = task });
    }

    public static void InvokeTaskCompleted(Task task)
    {
        TaskCompleted(null, new TaskEventArgs { taskPayload = task });
    }

    public static void InvokeAddColonist()
    {
        AddColonist(null, EventArgs.Empty);
    }

}