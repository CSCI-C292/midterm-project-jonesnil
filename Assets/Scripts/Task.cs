using JetBrains.Annotations;
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
            case TaskType.Reclaim:
                durationTimer = 2;
                break;
            case TaskType.Recruit:
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
        float roll = UnityEngine.Random.Range(0.0f, 1.0f);
        Debug.Log("this really it?" + roll);
        int relevantStat;
        float odds;

        switch (type)
        {
            case TaskType.Kill:
                relevantStat = colonist.fightingSkill - building.robotCount;
                odds = GetSuccessOdds(relevantStat);
                if (odds >= roll) 
                    building.robotCount = 0;
                break;
            case TaskType.Reclaim:
                relevantStat = colonist.buildingSkill;
                odds = GetSuccessOdds(relevantStat);
                if (odds >= roll)
                    GameEvents.InvokeBuildingReclaimed(building);
                break;
            case TaskType.Recruit:
                relevantStat = colonist.leadershipSkill;
                odds = GetSuccessOdds(relevantStat);
                if (odds >= roll)
                {
                    int peopleIndex = 0;
                    while (peopleIndex < building.peopleCount)
                    {
                        GameEvents.InvokeAddColonist();
                        peopleIndex += 1;
                    }
                }
                break;
        }

        building.inTask = false;
        GameEvents.InvokeTaskCompleted(this);
    }

    public float GetTaskOdds() 
    {
        int relevantStat;

        switch (type)
        {
            case TaskType.Kill:
                relevantStat = colonist.fightingSkill - building.robotCount;
                return GetSuccessOdds(relevantStat);
            case TaskType.Reclaim:
                relevantStat = colonist.buildingSkill;
                return GetSuccessOdds(relevantStat);
            case TaskType.Recruit:
                relevantStat = colonist.leadershipSkill;
                return GetSuccessOdds(relevantStat);
        }

        return 0f;
    }

    float GetSuccessOdds(int stat) 
    {
        if (stat < 0) return .1f;
        if (stat == 0) return .2f;
        if (stat == 1) return .4f;
        if (stat == 2) return .6f;
        if (stat == 3) return .7f;
        if (stat == 4) return .8f;
        if (stat == 5) return .9f;
        return .95f;
    }
}
