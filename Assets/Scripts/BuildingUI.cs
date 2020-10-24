using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BuildingUI : MonoBehaviour
{
    [SerializeField] CityBuilder city;
    Text typeText;
    Text foodText;
    Text peopleText;
    Text robotText;
    Image uiBox;
    GameObject exitButton;
    GameObject reclaimButton;
    Building building;

    // Start is called before the first frame update
    void Start()
    {

        GameEvents.BuildingClicked += OnBuildingClicked;
        uiBox = this.GetComponent<Image>();
        typeText = transform.GetChild(0).GetComponent<Text>();
        foodText = transform.GetChild(1).GetComponent<Text>();
        exitButton = transform.GetChild(2).gameObject;
        peopleText = transform.GetChild(3).GetComponent<Text>();
        robotText = transform.GetChild(4).GetComponent<Text>();
        reclaimButton = transform.GetChild(5).gameObject;

        this.CloseBuildingUI();
    }

    //This function sets up the UI after the player clicks on a building.
    void OnBuildingClicked(object sender, BuildingEventArgs args) 
    {
        building = args.buildingPayload;
        OpenBuildingUI();

        WipeText();
        typeText.text = building.typeName.ToString();

        if (building.reclaimed) 
            SetUpReclaimedUI();
        else 
            SetUpWildUI();
    }

    //This function disables all the UI components when the player clicks the X button on it.
    //It also activates the closing UI event to let the game know things should be clickable again
    //on the overworld.
    public void CloseBuildingUI() 
    {
        uiBox.enabled = false;
        typeText.enabled = false;
        foodText.enabled = false;
        exitButton.SetActive(false);
        peopleText.enabled = false;
        robotText.enabled = false;
        reclaimButton.SetActive(false);

        GameEvents.InvokeBuildingUIOver();
    }

    //This function just enables the UI components.
    public void OpenBuildingUI() 
    {
        uiBox.enabled = true;
        typeText.enabled = true;
        foodText.enabled = true;
        exitButton.SetActive(true); 
        peopleText.enabled = true;
        robotText.enabled = true;
    }

    //This function deletes the text still on the UI from before so certain elements
    //aren't still on there.
    public void WipeText() 
    {
        foodText.text = "";
        peopleText.text = "";
        robotText.text = "";
    }

    //This function sets up the UI text and buttons for when the player hasn't reclaimed
    //the building.
    public void SetUpWildUI() 
    {
        foodText.text = "Food available: " + building.GetFoodAmountString();
        peopleText.text = "People living here: " + building.peopleCount;

        if (building.robotCount == 0)
        {
            robotText.text = "There are no robots here, this space is reclaimable.";
            reclaimButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Reclaim";
        }
        else
        {
            robotText.text = "Killer robots roaming here: " + building.robotCount;
            reclaimButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Clear out robots";
        }

        if (city.Reclaimable(building))
            reclaimButton.SetActive(true);
    }

    //This function sets up the UI text and buttons for when the player has the building in their
    //base already.
    public void SetUpReclaimedUI()
    {
        switch (building.typeName) 
        {
            case BuildingType.Hospital:
                foodText.text = "Will help if people get hurt/sick.";
                break;
            case BuildingType.Apartment:
                foodText.text = "Houses two colonists.";
                break;
            case BuildingType.Grocery:
                foodText.text = "Not good for much. Coke machine works.";
                break;
            case BuildingType.Farm:
                foodText.text = "Station colonists here to make food.";
                break;
            case BuildingType.PD:
                foodText.text = "Station colonists here to raise your defense.";
                break;
        }
    }

    //This is just a function to be called by the reclaim button, and it uses an Event to tell
    //cityBuilder to do the work of reclaiming it.
    public void ReclaimBuilding() 
    {
        if (building.robotCount == 0)
            GameEvents.InvokeBuildingReclaimed(building);
        else 
        {
            GameEvents.InvokeTaskUIStarted(new Task(TaskType.Kill, building));
        }
    }
}
