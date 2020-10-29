using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColonistManager : MonoBehaviour
{
    Image background;
    GameObject exitButton;
    Text typeText;
    GameObject colonistChoiceOneButton;
    GameObject colonistChoiceTwoButton;
    GameObject colonistChoiceThreeButton;
    Text colonistNameOne;
    Text colonistNameTwo;
    Text colonistNameThree;
    GameObject nextColonistsButton;
    GameObject previousColonistsButton;
    Task currentTask;

    Text colonistStatsOne;
    Text colonistStatsTwo;
    Text colonistStatsThree;

    GameObject cancelTaskSelectionButton;
    GameObject confirmTaskSelectionButton;
    Text taskOdds;

    List<Colonist> assignableColonists;
    List<Colonist> allColonists;

    int UISlideNumber;


    // Start is called before the first frame update
    void Start()
    {
        GameEvents.TaskUIStarted += OpenUI;
        background = this.GetComponent<Image>();
        exitButton = transform.GetChild(0).gameObject;
        typeText = transform.GetChild(1).GetComponent<Text>();

        colonistChoiceOneButton = transform.GetChild(2).gameObject;
        colonistNameOne = colonistChoiceOneButton.transform.GetChild(0).GetComponent<Text>();

        colonistChoiceTwoButton = transform.GetChild(3).gameObject;
        colonistNameTwo = colonistChoiceTwoButton.transform.GetChild(0).GetComponent<Text>();

        colonistChoiceThreeButton = transform.GetChild(4).gameObject;
        colonistNameThree = colonistChoiceThreeButton.transform.GetChild(0).GetComponent<Text>();

        nextColonistsButton = transform.GetChild(5).gameObject;
        previousColonistsButton = transform.GetChild(6).gameObject;

        colonistStatsOne = transform.GetChild(7).GetComponent<Text>();
        colonistStatsTwo = transform.GetChild(8).GetComponent<Text>();
        colonistStatsThree = transform.GetChild(9).GetComponent<Text>();

        confirmTaskSelectionButton = transform.GetChild(10).gameObject;
        cancelTaskSelectionButton = transform.GetChild(11).gameObject;
        taskOdds = transform.GetChild(12).GetComponent<Text>();

        GameEvents.TaskCompleted += OnTaskCompleted;
        GameEvents.AddColonist += OnAddColonist;

        GameEvents.RemoveRandomColonist += OnRemoveRandomColonist;
        GameEvents.RemoveColonist += OnRemoveColonist;
        GameEvents.RoboAttack += OnRoboAttack;
        GameEvents.GameOver += OnGameOver;

        assignableColonists = new List<Colonist>();
        allColonists = new List<Colonist>();
        CreateStartingColonists();

        CloseUI();
        
    }

    public void CloseUI() 
    {
        colonistStatsOne.enabled = false;
        colonistStatsTwo.enabled = false;
        colonistStatsThree.enabled = false;
        background.enabled = false;
        exitButton.SetActive(false);
        typeText.enabled = false;
        colonistChoiceOneButton.SetActive(false);
        colonistChoiceTwoButton.SetActive(false);
        colonistChoiceThreeButton.SetActive(false);
        nextColonistsButton.SetActive(false);
        previousColonistsButton.SetActive(false);
        confirmTaskSelectionButton.SetActive(false);
        cancelTaskSelectionButton.SetActive(false);
        taskOdds.enabled = false;
        UISlideNumber = 1;
        try
        {
            GameEvents.InvokeTaskUIClosing();
        }
        catch { }
    }

    void OpenUI(object sender, TaskEventArgs args) 
    {
        UISlideNumber = 1;
        background.enabled = true;
        exitButton.SetActive(true);
        typeText.enabled = true;
        currentTask = args.taskPayload;

        colonistStatsOne.enabled = true;
        colonistStatsTwo.enabled = true;
        colonistStatsThree.enabled = true;

        typeText.text = currentTask.type.ToString();

        DisplayUISlide();
    }

    void DisplayUISlide() 
    {
        previousColonistsButton.SetActive(true);

        nextColonistsButton.SetActive(false);

        colonistChoiceOneButton.SetActive(true);
        colonistChoiceTwoButton.SetActive(true);
        colonistChoiceThreeButton.SetActive(true);

        colonistStatsOne.enabled = true;
        colonistStatsTwo.enabled = true;
        colonistStatsThree.enabled = true;

        if (UISlideNumber == 1)
        {
            previousColonistsButton.SetActive(false);
        }


        if (assignableColonists.Count < UISlideNumber * 3)
        {
            int difference = (UISlideNumber * 3) - assignableColonists.Count;
            if (difference == 2)
            {
                colonistNameOne.text = assignableColonists[(UISlideNumber * 3) - 3].name;
                colonistStatsOne.text = assignableColonists[(UISlideNumber * 3) - 3].GiveInfoString();
                colonistChoiceTwoButton.SetActive(false);
                colonistChoiceThreeButton.SetActive(false);

                colonistStatsTwo.enabled = false;
                colonistStatsThree.enabled = false;
            }
            if (difference == 1)
            {
                colonistNameOne.text = assignableColonists[(UISlideNumber * 3) - 3].name;
                colonistStatsOne.text = assignableColonists[(UISlideNumber * 3) - 3].GiveInfoString();
                colonistNameTwo.text = assignableColonists[(UISlideNumber * 3) - 2].name;
                colonistStatsTwo.text = assignableColonists[(UISlideNumber * 3) - 2].GiveInfoString();
                colonistChoiceThreeButton.SetActive(false);

                colonistStatsThree.enabled = false;
            }
        }
        else 
        {
            colonistNameOne.text = assignableColonists[(UISlideNumber * 3) - 3].name;
            colonistNameTwo.text = assignableColonists[(UISlideNumber * 3) - 2].name;
            colonistNameThree.text = assignableColonists[(UISlideNumber * 3) - 1].name;

            colonistStatsOne.text = assignableColonists[(UISlideNumber * 3) - 3].GiveInfoString();
            colonistStatsTwo.text = assignableColonists[(UISlideNumber * 3) - 2].GiveInfoString();
            colonistStatsThree.text = assignableColonists[(UISlideNumber * 3) - 1].GiveInfoString();
        }

        if (assignableColonists.Count > UISlideNumber * 3) 
        {
            nextColonistsButton.SetActive(true);
        }
    
    
    }

    void CreateStartingColonists() 
    {
        int startingColonistAmount = 3;
        int colonistIndex = 0;

        while (colonistIndex < startingColonistAmount) 
        {
            GameEvents.InvokeAddColonist();
            colonistIndex += 1;
        }
    }

    void OnAddColonist(object sender, EventArgs args) 
    {
        Colonist newColonist = new Colonist();
        assignableColonists.Add(newColonist);
        allColonists.Add(newColonist);
    }

    public void AdvanceSlide() 
    {
        UISlideNumber += 1;
        DisplayUISlide();
    }
     
    public void RegressSlide()
    {
        UISlideNumber -= 1;
        DisplayUISlide();
    }

    public void ChooseFirstColonist() 
    {
        currentTask.colonist = assignableColonists[(UISlideNumber * 3) - 3];
        OpenConfirmationUI();
    }

    public void ChooseSecondColonist()
    {
        currentTask.colonist = assignableColonists[(UISlideNumber * 3) - 2];
        OpenConfirmationUI();
    }

    public void ChooseThirdColonist()
    {
        currentTask.colonist = assignableColonists[(UISlideNumber * 3) - 1];
        OpenConfirmationUI();
    }

    public void OpenConfirmationUI() 
    {
        confirmTaskSelectionButton.SetActive(true);
        cancelTaskSelectionButton.SetActive(true);
        taskOdds.enabled = true;
        if(currentTask.GetTaskOdds() < 1)
            taskOdds.text = (currentTask.GetTaskOdds()*100).ToString() + "%";
        else
            taskOdds.text = "+" + currentTask.GetTaskOdds().ToString();
        colonistChoiceOneButton.GetComponent<Button>().interactable = false;
        colonistChoiceTwoButton.GetComponent<Button>().interactable = false;
        colonistChoiceThreeButton.GetComponent<Button>().interactable = false;
        nextColonistsButton.GetComponent<Button>().interactable = false;
        previousColonistsButton.GetComponent<Button>().interactable = false;
        exitButton.GetComponent<Button>().interactable = false;
    }

    public void CloseConfirmationUI() 
    {
        confirmTaskSelectionButton.SetActive(false);
        cancelTaskSelectionButton.SetActive(false);
        taskOdds.enabled = false;
        colonistChoiceOneButton.GetComponent<Button>().interactable = true;
        colonistChoiceTwoButton.GetComponent<Button>().interactable = true;
        colonistChoiceThreeButton.GetComponent<Button>().interactable = true;
        nextColonistsButton.GetComponent<Button>().interactable = true;
        previousColonistsButton.GetComponent<Button>().interactable = true;
        exitButton.GetComponent<Button>().interactable = true;
    }

    public void OnConfirmPressed() 
    {
        CloseConfirmationUI();
        assignableColonists.Remove(currentTask.colonist);
        GameEvents.InvokeTaskStarted(currentTask);
        currentTask.building.inTask = true;
        currentTask.active = true;
        CloseUI();
    }

    public void OnCancelPressed() 
    {
        CloseConfirmationUI();
    }

    public void OnExitPressed() 
    {
        currentTask.active = false;
        currentTask = null;
        CloseUI();
    }

    void OnTaskCompleted(object sender, TaskEventArgs args) 
    {
        Task task = args.taskPayload;
        assignableColonists.Add(task.colonist);
    }

    void OnRemoveRandomColonist(object sender, EventArgs args) 
    {
        GameEvents.InvokeRemoveColonist(allColonists[UnityEngine.Random.Range(0, allColonists.Count - 1)]);
    }

    void OnRemoveColonist(object sender, ColonistEventArgs args) 
    {
        Colonist colonistToRemove = args.colonistPayload;
        if (assignableColonists.Contains(colonistToRemove))
            allColonists.Remove(colonistToRemove);
        
        if(assignableColonists.Contains(colonistToRemove))
            assignableColonists.Remove(colonistToRemove);
    }

    void OnRoboAttack(object sender, BooleanEventArgs args) 
    {
        Boolean casualty = args.booleanPayload;

        if (casualty)
        {
            Colonist dead = allColonists[UnityEngine.Random.Range(0, allColonists.Count - 1)];
            GameEvents.InvokeRoboAttackUIStarted(dead);
        }
        else
        {
            GameEvents.InvokeRoboAttackUIStarted(null);
        }
    }

    void OnGameOver(object sender, EventArgs args) 
    {
        GameEvents.TaskUIStarted -= OpenUI;
        GameEvents.TaskCompleted -= OnTaskCompleted;
        GameEvents.AddColonist -= OnAddColonist;
        GameEvents.RemoveRandomColonist -= OnRemoveRandomColonist;
        GameEvents.RemoveColonist -= OnRemoveColonist;
        GameEvents.RoboAttack -= OnRoboAttack;
        GameEvents.GameOver -= OnGameOver;
    }


}
