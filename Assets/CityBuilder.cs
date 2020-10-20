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

    // Start is called before the first frame update
    void Start()
    {
        cityTiles = this.GetComponent<Tilemap>();
        cityTiles.SetTile(new Vector3Int(-2, 8, 0), buildings[0]);
        buildingCatalog = new Dictionary<Vector3Int, Building>();

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
                Vector3Int tilePos = new Vector3Int(tileCoordX, tileCoordY, 0);
                int type = GetRandomBuilding();
                cityTiles.SetTile(tilePos, buildings[type]);
                Building currentBuilding = new Building(type);
                buildingCatalog.Add(tilePos, currentBuilding);
                Debug.Log("X");
                Debug.Log(tileCoordX);
                Debug.Log("Y");
                Debug.Log(tileCoordY);
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

    int GetRandomBuilding() 
    {
        return Random.Range(0, buildings.Length);
    }

    private void OnMouseDown()
    {
        Vector3 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 flatClickedPos = new Vector3(clickedPos.x, clickedPos.y, 0);

        Vector3Int clickedTilePos = cityTiles.WorldToCell(flatClickedPos);
        

        if(buildingCatalog.ContainsKey(clickedTilePos))
            Debug.Log(buildingCatalog[clickedTilePos].type);

        else
            Debug.Log("not a building. Misclick?");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
