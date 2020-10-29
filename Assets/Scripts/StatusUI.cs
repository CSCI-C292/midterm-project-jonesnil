using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    Button advanceDayButton;
    Text foodDisplay;
    Text peopleDisplay;
    Text defenseDisplay;

    int currentFood;
    int currentColonists;
    int maxColonists;
    int farming;
    int defense;
    int buildingsReclaimed;

    public static Boolean canAddColonist;

    [SerializeField] GameObject iconPrefab;

    Dictionary<Vector3, GameObject> iconFinder;
        
    List<Task> taskHolder;

    private void Awake()
    {
        this.advanceDayButton = transform.GetChild(0).GetComponent<Button>();
        this.foodDisplay = transform.GetChild(1).GetComponent<Text>();
        this.peopleDisplay = transform.GetChild(2).GetComponent<Text>();
        this.defenseDisplay = transform.GetChild(3).GetComponent<Text>();

        GameEvents.BuildingClicked += OnBuildingClicked;
        GameEvents.BuildingUIClosing += OnBuildingUIClosing;

        this.iconFinder = new Dictionary<Vector3, GameObject>();

        this.taskHolder = new List<Task>();

        GameEvents.TaskStarted += OnTaskStarted;
        GameEvents.TaskCompleted += OnTaskCompleted;

        GameEvents.AddColonist += OnAddColonist;
        GameEvents.BuildingReclaimed += OnBuildingReclaimed;
        GameEvents.FoodAdded += OnFoodAdded;
        GameEvents.RemoveColonist += OnRemoveColonist;
        GameEvents.TaskCancelled += OnTaskCancelled;
        GameEvents.RoboAttackUIStarted += OnRoboAttackUIStarted;
        GameEvents.AlertConcluded += OnAlertConcluded;
        GameEvents.AlertStarted += OnAlertStarted;
        GameEvents.GameOver += OnGameOver;

        this.currentFood = 20;
        this.currentColonists = 0;

        canAddColonist = true;

        this.defense = 0;
        this.farming = 0;

        this.buildingsReclaimed = 0;

        this.UpdateDisplay();
    }

    void OnAddColonist(object sender, EventArgs args) 
    {
        this.currentColonists += 1;
        if (this.currentColonists >= this.maxColonists)
            canAddColonist = false;

        this.UpdateDisplay();
    }

    void OnBuildingReclaimed(object sender, BuildingEventArgs args) 
    {
        buildingsReclaimed += 1;

        Building sentBuilding = args.buildingPayload;
        switch (sentBuilding.typeName) 
        {
            case BuildingType.Apartment:
                this.maxColonists += 2;
                if (this.currentColonists < this.maxColonists)
                    canAddColonist = true;
                break;
        }

        this.UpdateDisplay();
    }

    void OnFoodAdded(object sender, IntEventArgs args) 
    {
        int foodAdded = args.intPayload;
        this.currentFood += foodAdded;

        this.UpdateDisplay();
    }

    void UpdateDisplay() 
    {
        this.foodDisplay.text = "Food: " + this.currentFood.ToString() + "/" + (this.farming - this.currentColonists).ToString();
        this.peopleDisplay.text = "Colonists: " + this.currentColonists.ToString() + "/" + this.maxColonists.ToString();
        this.defenseDisplay.text = "Defense: " + this.defense.ToString();
    }


    void OnBuildingClicked(object sender, BuildingEventArgs args) 
    {
        advanceDayButton.interactable = false;
    }

    void OnBuildingUIClosing(object sender, EventArgs args) 
    {
        advanceDayButton.interactable = true;
    }

    void OnAlertStarted(object sender, AlertEventArgs args)
    {
        advanceDayButton.interactable = false;
    }

    public void AdvanceDay() 
    {
        try
        {
            GameEvents.InvokeDayAdvanced();
        }
        catch { }


        currentFood += (this.farming - this.currentColonists);
        if (currentFood < 0)
        {
            GameEvents.InvokeAlertStarted("One of your colonists died of hunger! Set someone to farm or scavenge to make food.","Noted.");
        }

        
        float robotAttackRoll = UnityEngine.Random.Range(0.0f, 1.0f);
        if (robotAttackRoll <= .1f)
        {
            int robotAttackSeverity = UnityEngine.Random.Range(1, buildingsReclaimed);
            if (robotAttackSeverity > defense)
            {
                GameEvents.InvokeRoboAttack(true);
            }
            else
            {
                GameEvents.InvokeRoboAttack(false);
            }
        }
        

        UpdateDisplay();
    }

    void OnTaskStarted(object sender, TaskEventArgs args) 
    {
        Task startedTask = args.taskPayload;

        Vector3 buildingPos = startedTask.building.worldPosition;
        buildingPos.y += 1f;
        Vector3 iconPos = Camera.main.WorldToScreenPoint(buildingPos);
        Vector3 iconRotation = new Vector3(0, 0, 45);
        GameObject newIcon = Instantiate(iconPrefab, iconPos, Quaternion.Euler(iconRotation), this.transform);
        iconFinder.Add(startedTask.building.worldPosition, newIcon);


        if (startedTask.type == TaskType.Farm)
            farming += startedTask.colonist.scoutingSkill;

        if (startedTask.type == TaskType.Protect)
            defense += startedTask.colonist.fightingSkill;

        taskHolder.Add(startedTask);
        UpdateDisplay();
    }

    void OnTaskCompleted(object sender, TaskEventArgs args) 
    {
        Task finishedTask = args.taskPayload;
       
        finishedTask.building.inTask = false;

        GameObject oldIcon = iconFinder[finishedTask.building.worldPosition];
        iconFinder.Remove(finishedTask.building.worldPosition);
        Destroy(oldIcon);

        if (finishedTask.type == TaskType.Farm)
            farming -= finishedTask.colonist.scoutingSkill;

        if (finishedTask.type == TaskType.Protect)
            defense -= finishedTask.colonist.fightingSkill;

        taskHolder.Remove(finishedTask);
        UpdateDisplay();
    }

    void OnRemoveColonist(object sender, ColonistEventArgs args) 
    {
        this.currentColonists -= 1;

        if (this.currentColonists <= this.maxColonists)
            canAddColonist = true;

        if (this.currentColonists <= 0)
            GameEvents.InvokeGameOver();

        Colonist colonistToRemove = args.colonistPayload;
        int counter = 0;
        int target = taskHolder.Count;
        List<Task> dummyTaskHolder = new List<Task>();

        while (counter < target) 
        {
            if (taskHolder[counter].colonist == colonistToRemove) 
            {
                dummyTaskHolder.Add(taskHolder[counter]);
            }

            counter += 1;
        }

        foreach (Task badTask in dummyTaskHolder) 
        {
            badTask.active = false;
            taskHolder.Remove(badTask);
            GameEvents.InvokeTaskCompleted(badTask);
        }

        UpdateDisplay();
    
    }

    void OnTaskCancelled(object sender, BuildingEventArgs args) 
    {
        Building buildingToRemoveTaskFrom = args.buildingPayload;
        int counter = 0;
        int target = taskHolder.Count;
        List<Task> dummyTaskHolder = new List<Task>();

        while (counter < target)
        {
            if (taskHolder[counter].building == buildingToRemoveTaskFrom)
            {
                dummyTaskHolder.Add(taskHolder[counter]);
            }

            counter += 1;
        }

        foreach (Task badTask in dummyTaskHolder)
        {
            badTask.active = false;
            taskHolder.Remove(badTask);
            GameEvents.InvokeTaskCompleted(badTask);
        }
    }

    void OnRoboAttackUIStarted(object sender, ColonistEventArgs args)
    {
        advanceDayButton.interactable = false;
    }

    private void OnAlertConcluded(object sender, EventArgs args)
    {
        advanceDayButton.interactable = true;
    }

    void OnGameOver(object sender, EventArgs args) 
    {
        advanceDayButton.interactable = false;

        GameEvents.BuildingClicked -= OnBuildingClicked;
        GameEvents.BuildingUIClosing -= OnBuildingUIClosing;
        GameEvents.TaskStarted -= OnTaskStarted;
        GameEvents.TaskCompleted -= OnTaskCompleted;
        GameEvents.AddColonist -= OnAddColonist;
        GameEvents.BuildingReclaimed -= OnBuildingReclaimed;
        GameEvents.FoodAdded -= OnFoodAdded;
        GameEvents.RemoveColonist -= OnRemoveColonist;
        GameEvents.TaskCancelled -= OnTaskCancelled;
        GameEvents.RoboAttackUIStarted -= OnRoboAttackUIStarted;
        GameEvents.AlertConcluded -= OnAlertConcluded;
        GameEvents.AlertStarted -= OnAlertStarted;
        GameEvents.GameOver -= OnGameOver;
    }

}
