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

        assignableColonists = new List<Colonist>();
        allColonists = new List<Colonist>();
        CreateStartingColonists();

        CloseUI();
        
    }

    public void CloseUI() 
    {
        background.enabled = false;
        exitButton.SetActive(false);
        typeText.enabled = false;
        colonistChoiceOneButton.SetActive(false);
        colonistChoiceTwoButton.SetActive(false);
        colonistChoiceThreeButton.SetActive(false);
        nextColonistsButton.SetActive(false);
        previousColonistsButton.SetActive(false);
        UISlideNumber = 1;
    }

    void OpenUI(object sender, TaskEventArgs args) 
    {
        UISlideNumber = 1;
        background.enabled = true;
        exitButton.SetActive(true);
        typeText.enabled = true;
        currentTask = args.taskPayload;

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
                colonistChoiceTwoButton.SetActive(false);
                colonistChoiceThreeButton.SetActive(false);
            }
            if (difference == 1)
            {
                colonistNameOne.text = assignableColonists[(UISlideNumber * 3) - 3].name;
                colonistNameTwo.text = assignableColonists[(UISlideNumber * 3) - 2].name;
                colonistChoiceThreeButton.SetActive(false);
            }
        }
        else 
        {
            colonistNameOne.text = assignableColonists[(UISlideNumber * 3) - 3].name;
            colonistNameTwo.text = assignableColonists[(UISlideNumber * 3) - 2].name;
            colonistNameThree.text = assignableColonists[(UISlideNumber * 3) - 1].name;
        }

        if (assignableColonists.Count > UISlideNumber * 3) 
        {
            nextColonistsButton.SetActive(true);
        }
    
    
    }

    void CreateStartingColonists() 
    {
        int startingColonistAmount = 14;
        int colonistIndex = 0;

        while (colonistIndex < startingColonistAmount) 
        {
            Colonist newColonist = new Colonist();
            assignableColonists.Add(newColonist);
            allColonists.Add(newColonist);
            colonistIndex += 1;
        }
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
        CloseUI();
    }

    public void ChooseSecondColonist()
    {
        currentTask.colonist = assignableColonists[(UISlideNumber * 3) - 3];
        CloseUI();
    }

    public void ChooseThirdColonist()
    {
        currentTask.colonist = assignableColonists[(UISlideNumber * 3) - 3];
        CloseUI();
    }


}
