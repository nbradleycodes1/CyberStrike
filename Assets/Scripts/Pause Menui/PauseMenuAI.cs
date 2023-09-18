using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PauseMenuAI : MonoBehaviour
{
    public GameObject pause, resumeGame, restartGame, toMainMenu, quitValidation, pauseMenuText, sideValidation, switchSides;
    public static Action ResetGameEvent;
    bool flag = false;

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape))
        if (!GameObject.FindWithTag("colorpickerscreen") && !GameManager.IsAISearching)
        {
            if (!GameManager.isGameOver && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse2)))
            {
                if (flag == false)
                {
                    pause.gameObject.SetActive(true);
                    flag = true;
                    resumeGame.gameObject.SetActive(true);
                    restartGame.gameObject.SetActive(true);
                    toMainMenu.gameObject.SetActive(true);
                    pauseMenuText.gameObject.SetActive(true);
                    switchSides.gameObject.SetActive(true);
                }
                else
                {
                    pause.gameObject.SetActive(false);
                    flag = false;
                    quitValidation.gameObject.SetActive(false);
                    sideValidation.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnClickedResumeGame()
    {
        pause.gameObject.SetActive(false);
    }

    public void OnClickedRestartGame()
    {
        ResetGameEvent();
    }

    public void OnClickedQuit()
    {
        quitValidation.gameObject.SetActive(true);

        resumeGame.gameObject.SetActive(false);
        restartGame.gameObject.SetActive(false);
        toMainMenu.gameObject.SetActive(false);
        pauseMenuText.gameObject.SetActive(false);
        switchSides.gameObject.SetActive(false);
    }

    public void OnCLickedYes()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnCLickedCancel()
    {
        quitValidation.gameObject.SetActive(false);
        sideValidation.gameObject.SetActive(false);

        resumeGame.gameObject.SetActive(true);
        restartGame.gameObject.SetActive(true);
        toMainMenu.gameObject.SetActive(true);
        pauseMenuText.gameObject.SetActive(true);
        switchSides.gameObject.SetActive(true);
    }

    public void OnClickedPickSides()
    {
        sideValidation.gameObject.SetActive(true);

        resumeGame.gameObject.SetActive(false);
        restartGame.gameObject.SetActive(false);
        toMainMenu.gameObject.SetActive(false);
        pauseMenuText.gameObject.SetActive(false);
        switchSides.gameObject.SetActive(false);
    }

    public void OnClickedWhite()
    {
        ResetGameEvent();
        pause.gameObject.SetActive(false);
    }

    public void OnClickedBlack()
    {
        ResetGameEvent();
        pause.gameObject.SetActive(false);
    }
}

