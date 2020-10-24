﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CityBuilder : MonoBehaviour
{

    Tilemap cityTiles;
    [SerializeField] Tile[] buildings;
    Dictionary<Vector3Int, Building> buildingCatalog;
    Dictionary<Building, Vector3Int> tileCatalog;
    Boolean inMenu;

    // Start is called before the first frame update
    void Awake()
    {
        GameEvents.BuildingClicked += OnBuildingClicked;
        GameEvents.BuildingReclaimed += OnBuildingReclaimed;
        GameEvents.BuildingUIClosing += OnBuildingUIClosing;

        cityTiles = this.GetComponent<Tilemap>();
        cityTiles.SetTile(new Vector3Int(-2, 8, 0), buildings[0]);
        buildingCatalog = new Dictionary<Vector3Int, Building>();
        tileCatalog = new Dictionary<Building, Vector3Int>();

        RandomizeBuildings();
        ReclaimStarterBuildings();

        inMenu = false;

    }


    //This function places a random building in every building tile for the city by looping
    //over the columns and rows.
    public void RandomizeBuildings() 
    {

        int rowIndex = 0;
        int colIndex = 0;
        int buildingsInCol = 0;
        int tileCoordStartX = -2;
        int tileCoordStartY = 8;
        int tileCoordX = 0;
        int tileCoordY = 0;

        while (colIndex < 11)
        {
            buildingsInCol = GetBuildingsInCol(colIndex);

            tileCoordX = tileCoordStartX;
            tileCoordY = tileCoordStartY;

            while (rowIndex < buildingsInCol)
            {

                //I grab not only the building position but also the position of the tiles above
                //them. This is to make it easier for the player to click the building later, I'm
                //just going to associate each tile with the building created.
                Vector3Int tilePos = new Vector3Int(tileCoordX, tileCoordY, 0);
                Vector3Int tilePosUpLeft = new Vector3Int(tileCoordX, tileCoordY + 1, 0);
                Vector3Int tilePosUpRight = new Vector3Int(tileCoordX + 1, tileCoordY, 0);
                Vector3Int tilePosUp = new Vector3Int(tileCoordX + 1, tileCoordY + 1, 0);

                //This grays out tiles. I want tiles to start off not claimed by the player
                //(except a few)
                cityTiles.SetTileFlags(tilePos, TileFlags.None);
                cityTiles.SetColor(tilePos, Color.grey);

                //This makes the building with a random type and adds nearby tiles to the 
                //buildingCatalog so they can be recognized later when they're clicked.
                //
                //It also makes an inverse dictionary, because sometimes I want to take a
                //building and get its location instead. In that case I don't include the 
                //adjacent locations because I want the precise location.
                int type = GetRandomBuilding();
                cityTiles.SetTile(tilePos, buildings[type]);
                Building currentBuilding = new Building(type);
                buildingCatalog.Add(tilePos, currentBuilding);
                buildingCatalog.Add(tilePosUpLeft, currentBuilding);
                buildingCatalog.Add(tilePosUpRight, currentBuilding);
                buildingCatalog.Add(tilePosUp, currentBuilding);
                tileCatalog.Add(currentBuilding, tilePos);

                tileCoordX -= 2;
                tileCoordY -= 2;
                rowIndex += 1;
            }

            rowIndex = 0;

            if ((colIndex % 2) == 0)
                tileCoordStartX += 2;
            else
                tileCoordStartY -= 2;

            colIndex += 1;
        }
    }

    //This just keeps track of how many buildings are in each column because they're
    //not uniform. The randomizer needs to know so it doesn't place too many or too few
    //when creating.
    int GetBuildingsInCol(int colIndex)
    {
        int buildingsInCol = 0;

        switch (colIndex)
        {
            case 0:
                buildingsInCol = 4;
                break;
            case 1:
                buildingsInCol = 5;
                break;
            case 2:
                buildingsInCol = 5;
                break;
            case 3:
                buildingsInCol = 6;
                break;
            case 4:
                buildingsInCol = 5;
                break;
            case 5:
                buildingsInCol = 6;
                break;
            case 6:
                buildingsInCol = 5;
                break;
            case 7:
                buildingsInCol = 6;
                break;
            case 8:
                buildingsInCol = 5;
                break;
            case 9:
                buildingsInCol = 5;
                break;
            case 10:
                buildingsInCol = 4;
                break;
        }

        return buildingsInCol;
    }

    //This just gets a random number to represent a building type.
    int GetRandomBuilding()
    {
        return UnityEngine.Random.Range(0, buildings.Length);
    }

    private void OnMouseDown()
    {
        //This checks if you're in a menu, and if you're not it tries to see if you clicked
        //a building. If you did, it starts the BuildingUI to show you the info. It grabs the
        //clicked building by looking at the position/building dictionary.
        if (!inMenu)
        {
            Vector3 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 flatClickedPos = new Vector3(clickedPos.x, clickedPos.y, 0);

            Vector3Int clickedTilePos = cityTiles.WorldToCell(flatClickedPos);

            Vector3Int emptyRepresentation = new Vector3Int(100, 100, 100);

            if (buildingCatalog.ContainsKey(clickedTilePos))
                GameEvents.InvokeBuildingClicked(buildingCatalog[clickedTilePos]);

            else
                Debug.Log("not a building. Misclick?");
        }
    }

    //This is called when the reclaimed building event happens.
    private void OnBuildingReclaimed(object sender, BuildingEventArgs args) 
    {
        setBuildingReclaimed(args.buildingPayload);
    }

    //This is called by the last function to set the color of the tile white 
    //to signal it's reclaimed. It also tells the building it's reclaimed so 
    //it can tell other stuff that needs to know later.
    private void setBuildingReclaimed(Building building) 
    {
        cityTiles.SetColor(tileCatalog[building], Color.white);
        building.reclaimed = true;
    }

    //This function is called by the buildingClicked event and it just stops
    //the mouse from clicking on buildings more.
    private void OnBuildingClicked(object sender, BuildingEventArgs args)
    {
        inMenu = true;
    }

    //This is called when the buildingUI closes and it lets the mouse click on
    //buildings again.
    private void OnBuildingUIClosing(object sender, EventArgs args)
    {
        inMenu = false;
    }

    //This gets the adjacent building positions from a building position. It's an
    //array locked to 4 entries, and it makes the entries that are adjacent but aren't
    //buildings (100,100,100) to signify to the caller that they aren't buildings.
    //
    //This is mainly to help let the UI know what buildings can be reclaimed, but it is
    //used for a few other things for convenience.
    private Vector3Int[] GetAdjacentBuildingPositions(Vector3Int buildingPos) 
    {
        Vector3Int[] output = new Vector3Int[4];

        Vector3Int buildingPosUpRight = new Vector3Int(buildingPos.x + 2, buildingPos.y, 0);
        if (buildingCatalog.ContainsKey(buildingPosUpRight))
            output[0] = buildingPosUpRight;
        else
            output[0] = new Vector3Int(100, 100, 100);

        Vector3Int buildingPosUpLeft = new Vector3Int(buildingPos.x, buildingPos.y + 2, 0);
        if (buildingCatalog.ContainsKey(buildingPosUpLeft))
            output[1] = buildingPosUpLeft;
        else
            output[1] = new Vector3Int(100, 100, 100);

        Vector3Int buildingPosBottomLeft = new Vector3Int(buildingPos.x - 2, buildingPos.y, 0);
        if (buildingCatalog.ContainsKey(buildingPosBottomLeft))
            output[2] = buildingPosBottomLeft;
        else
            output[2] = new Vector3Int(100, 100, 100);

        Vector3Int buildingPosBottomRight = new Vector3Int(buildingPos.x, buildingPos.y - 2, 0);
        if (buildingCatalog.ContainsKey(buildingPosBottomRight))
            output[3] = buildingPosBottomRight;
        else
            output[3] = new Vector3Int(100, 100, 100);

        return output;
    }


    //This sets the central building and the adjacent buildings to it to be claimed, to 
    //start the game.
    public void ReclaimStarterBuildings() 
    {
        Vector3Int hubBuildingPos = new Vector3Int(0, 0, 0);
        setBuildingReclaimed(buildingCatalog[hubBuildingPos]);

        foreach (Vector3Int adjacentBuilding in GetAdjacentBuildingPositions(hubBuildingPos)) 
        {
            setBuildingReclaimed(buildingCatalog[adjacentBuilding]);
        }

    }

    //This tells you if a building is reclaimable (whether a building next to it is claimed
    // or not.)
    public Boolean Reclaimable(Building building) 
    {
        Boolean output = false;
        Vector3Int buildingPos = tileCatalog[building];
        Building adjacentBuilding;

        foreach (Vector3Int adjacentBuildingPos in GetAdjacentBuildingPositions(buildingPos))
        {
            if (buildingCatalog.ContainsKey(adjacentBuildingPos))
            {
                adjacentBuilding = buildingCatalog[adjacentBuildingPos];
                if (adjacentBuilding.reclaimed) output = true;

            }
        }
        return output;
    }

}