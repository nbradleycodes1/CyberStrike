using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuTutorial : MonoBehaviour
{
    public GameObject pause, resumeGame, anim;
    bool flag = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (flag == false)
            {
                pause.gameObject.SetActive(true);
                flag = true;
                resumeGame.gameObject.SetActive(true);
            }
            else
            {
                pause.gameObject.SetActive(false);
                flag = false;
                anim.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickedResumeGame()
    {
        pause.gameObject.SetActive(false);
        anim.gameObject.SetActive(false);
    }

    
}


