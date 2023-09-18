using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitDisconnectPanel : MonoBehaviour
{
    public void OnClickedQuit()
    {
        SceneManager.LoadScene("Menu");
        // SceneManager.LoadScene("LoadingScene");
    }
}