using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
    public static int SceneNumber;
    public GameObject skip;

    void Start()
    {
        
            if (SceneNumber == 0)
        {
            StartCoroutine(ToSplashTwo());
        }
        if (SceneNumber == 1)
        {
            StartCoroutine(ToMainMenu());
        }
    }

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Menu");
            Debug.Log("A key or mouse click has been detected");
        }
    }

    public void Skip()
    {
        SceneManager.LoadScene(2);
    }

    IEnumerator ToSplashTwo()
    {
        yield return new WaitForSeconds(14);
        SceneNumber = 1;
        SceneManager.LoadScene(1);
    }

    IEnumerator ToMainMenu()
    {
        yield return new WaitForSeconds(10);
        SceneNumber = 2;
        SceneManager.LoadScene(2);
    }
}
