﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

//This is a non-Mono Behavior class that mostly just stores information about
//each building created by cityBuilder. It also has some useful functions so
//anything accessing it can pull info out of it easily.


public class Building
{
    public int typeNum;
    public BuildingType typeName;
    public int food;
    public int peopleRandomRoll;
    public int peopleCount;
    public int robotCount;
    public Boolean reclaimed;

    //These function calls create everything about the building. The
    //only thing given to the building on creation is its type, it 
    //does the heavy lifting of making its stats on its own.
    public Building(int startType)
    {
        this.typeNum = startType;
        this.typeName = this.GetTypeNameByNum(typeNum);
        this.food = this.GetFoodByType(typeName);
        this.peopleRandomRoll = this.GetPeopleByType(typeName);
        this.peopleCount = this.GetPeopleAmount();
        this.robotCount = this.GetRobotCount();
        this.reclaimed = false;
    }

    //This just grabs the enum based on a randomly rolled number this is
    //given on creation.
    private BuildingType GetTypeNameByNum(int typeNumber)
    {
        BuildingType output = BuildingType.Hospital;

        switch (typeNumber)
        {
            case 0:
                output = BuildingType.Hospital;
                break;
            case 1:
                output = BuildingType.Apartment;
                break;
            case 2:
                output = BuildingType.Grocery;
                break;
            case 3:
                output = BuildingType.PD;
                break;
            case 4:
                output = BuildingType.Farm;
                break;
        }
        return output;
    }

    //This function just randomly rolls how much food
    //the building will have based on its type. Some
    //types have a better chance of having food.
    private int GetFoodByType(BuildingType type)
    {
        int output = 0;
        switch (type) 
        {
            case BuildingType.Hospital:
                output = UnityEngine.Random.Range(1, 3);
                break;
            case BuildingType.Apartment:
                output = UnityEngine.Random.Range(1, 3);
                break;
            case BuildingType.Grocery:
                output = UnityEngine.Random.Range(5, 10);
                break;
            case BuildingType.Farm:
                output = UnityEngine.Random.Range(5, 10);
                break;
            case BuildingType.PD:
                output = UnityEngine.Random.Range(0, 2);
                break;
        }

        return output;
    }

    //Like the food one, this determines how many people live in
    //a building by the building type. The number does not represent 
    //the amount though, the number rolled is translated into the amount
    //later.
    private int GetPeopleByType(BuildingType type)
    {
        int output = 0;
        switch (type)
        {
            case BuildingType.Hospital:
                output = UnityEngine.Random.Range(0, 6);
                break;
            case BuildingType.Apartment:
                output = UnityEngine.Random.Range(2, 10);
                break;
            case BuildingType.Grocery:
                output = UnityEngine.Random.Range(0, 3);
                break;
            case BuildingType.Farm:
                output = UnityEngine.Random.Range(0, 3);
                break;
            case BuildingType.PD:
                output = UnityEngine.Random.Range(0, 6);
                break;
        }

        return output;
    }

    //This rolls the number of robots roaming in a building.
    private int GetRobotCount()
    {
        return UnityEngine.Random.Range(1,4);
    }

    //This gives the buildingUI the text for what amount of food
    //is in the building based on the food variable.
    public string GetFoodAmountString()
    {
        string output = "placeholder";
        if(this.food < 3) output = "Scraps";
        if(this.food >= 3) output = "A few meals";
        if (this.food >= 7) output = "Stockpile";
        return output;
    }

    //This takes the randomly determined people number from earlier
    //and turns it into the actual number of people hiding at the location.
    public int GetPeopleAmount()
    {
        int output = 0;
        if (this.peopleRandomRoll < 3) output = 0;
        if (this.peopleRandomRoll >= 3) output = 1;
        if (this.peopleRandomRoll >= 7) output = 2;
        return output;
    }
}
