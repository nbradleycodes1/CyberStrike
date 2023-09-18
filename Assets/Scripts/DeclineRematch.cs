using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeclineRematch : MonoBehaviour
{
    public void ToLobby()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
