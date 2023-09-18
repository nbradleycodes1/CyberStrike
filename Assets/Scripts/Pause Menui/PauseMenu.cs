using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pause, resumeGame, restartGame, toMainMenu, quitValidation, pauseMenuText;
    public static Action ResetGameEvent;
    bool flag = false;

    void Update()
    {
        if (!GameManager.isGameOver && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse2)))
        {
            if (flag == false) {
                pause.gameObject.SetActive(true);
                flag = true;
                resumeGame.gameObject.SetActive(true);
                restartGame.gameObject.SetActive(true);
                toMainMenu.gameObject.SetActive(true);
                pauseMenuText.gameObject.SetActive(true);
            }
            else
            {
                pause.gameObject.SetActive(false);
                flag = false;
                quitValidation.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickedResumeGame()
    {
        pause.gameObject.SetActive(false);
    }

    public void OnClickedRestartGame()
    {
        // ResetGameEvent();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // pause.gameObject.SetActive(false);
    }

    public void OnClickedQuit()
    {
        quitValidation.gameObject.SetActive(true);

        resumeGame.gameObject.SetActive(false);
        restartGame.gameObject.SetActive(false);
        toMainMenu.gameObject.SetActive(false);
        pauseMenuText.gameObject.SetActive(false);
    }

    public void OnCLickedYes()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnCLickedNo()
    {
        quitValidation.gameObject.SetActive(false);

        resumeGame.gameObject.SetActive(true);
        restartGame.gameObject.SetActive(true);
        toMainMenu.gameObject.SetActive(true);
        pauseMenuText.gameObject.SetActive(true);
    }
}
