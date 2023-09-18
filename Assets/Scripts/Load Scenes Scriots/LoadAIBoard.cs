using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadAIBoard : MonoBehaviour
{
    public GameObject LoadAnim, skipButton, imageTrans, fadeTrans, fixedTrans, Trans;
    public TMP_Text skipmessage;
    private bool ready = false;
    void Start()
    {
        fixedTrans.gameObject.SetActive(false);
        fadeTrans.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        Invoke("Show", 0f);
        Invoke("Show2", 0f);
        
    }

    private void Update()
    {
        if (ready && Input.anyKey)
        {
            skipmessage.gameObject.SetActive(false);
            Skip();
        }
    }

    public void Skip()
    {
        fadeTrans.gameObject.SetActive(true);
        StartCoroutine(ToSplashAIBoard());
    }

    public void Show()
    {
        imageTrans.gameObject.SetActive(true);
        Invoke("Hide", 3f);
    }

    public void Hide()
    {
        imageTrans.gameObject.SetActive(false);
    }


    public void Show2()
    {
        LoadAnim.gameObject.SetActive(true);
        Invoke("Hide2", 2f);
    }

    // hide animation after X seconds
    public void Hide2()
    {
        LoadAnim.gameObject.SetActive(false);
        //skipButton.gameObject.SetActive(true);
        skipmessage.gameObject.SetActive(true);
        ready = true;
    }

    IEnumerator ToSplashAIBoard()
    {
        yield return new WaitForSeconds(2);
        fixedTrans.gameObject.SetActive(true);
        
        Trans.gameObject.SetActive(true);
        SceneManager.LoadScene("AILocalBoard");
    }
}