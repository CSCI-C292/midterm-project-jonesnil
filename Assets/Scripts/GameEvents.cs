using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingEventArgs : EventArgs
{
    public Building buildingPayload;
}


public static class GameEvents
{

    public static event EventHandler<BuildingEventArgs> BuildingClicked;
    public static event EventHandler BuildingUIClosing;
    public static event EventHandler<BuildingEventArgs> BuildingReclaimed;

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

}