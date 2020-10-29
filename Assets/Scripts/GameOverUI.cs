using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    Image backgroundPanel;
    Image mainPanel;
    Text gameOverText;
    Colonist dead;
    GameObject restartButton;
    AlertType alertType;

    void Start()
    {
        backgroundPanel = transform.parent.GetComponent<Image>();
        mainPanel = transform.GetComponent<Image>();
        gameOverText = transform.GetChild(0).GetComponent<Text>();
        restartButton = transform.GetChild(1).gameObject;
        dead = null;

        GameEvents.RoboAttackUIStarted += OnRoboAttackUIStarted;
        GameEvents.AlertStarted += OnAlertStarted;
        GameEvents.GameOver += OnGameOver;

        CloseGameOverUI();
    }

    void CloseGameOverUI() 
    {
        backgroundPanel.enabled = false;
        mainPanel.enabled = false;
        gameOverText.enabled = false;
        restartButton.SetActive(false);
    }

    void OpenGameOverUI()
    {
        backgroundPanel.enabled = true;
        mainPanel.enabled = true;
        gameOverText.enabled = true;
        restartButton.SetActive(true);
    }

    void OnGameOver(object sender, EventArgs args) 
    {
        alertType = AlertType.GameOver;

        gameOverText.text = "All of your colonists have died, whether by starvation or the robot menace. May the human legacy live on in their beeps.";
        restartButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Restart?";

        GameEvents.RoboAttackUIStarted -= OnRoboAttackUIStarted;
        GameEvents.AlertStarted -= OnAlertStarted;
        GameEvents.GameOver -= OnGameOver;

        OpenGameOverUI();
    }

    void OnRoboAttackUIStarted(object sender, ColonistEventArgs args) 
    {
        alertType = AlertType.RobotAttack;

        OpenGameOverUI();

        if (args.colonistPayload != null)
        {
            dead = args.colonistPayload;
            gameOverText.text = "ROBOT ATTACK: They rushed us bad last night. We held them off for now, but " + dead.name + " was killed.";
            restartButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Goodbye, " + dead.name;
        }
        else
        {
            gameOverText.text = "ROBOT ATTACK: They attacked last night, but we were ready. Even went out and popped the heads to stop the beeping.";
            restartButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Nice.";
        }

        
    }

    void OnAlertStarted(object sender, AlertEventArgs args)
    {
        alertType = AlertType.Misc;

        OpenGameOverUI();

        gameOverText.text = args.alertString;
        restartButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = args.buttonString;
    }

    public void RestartGame() 
    {
        switch (alertType) 
        {
            case AlertType.GameOver:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case AlertType.RobotAttack:
                CloseGameOverUI();
                GameEvents.InvokeAlertConcluded();
                if (dead != null)
                {
                    GameEvents.InvokeRemoveColonist(dead);
                    dead = null;
                }
                break;
            case AlertType.Misc:
                CloseGameOverUI();
                GameEvents.InvokeAlertConcluded();
                GameEvents.InvokeRemoveRandomColonist();
                break;
        }
    }

}
