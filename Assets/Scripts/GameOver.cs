using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Photon.Pun;


public class GameOver : MonoBehaviour
{
    public GameObject transition, soundtrack, background, rematchAnim, whiteHatAnim, whiteLogoAnim, blackHatAnim, blackLogoAnim, gameOverTextAnim, winTextAnim, gameOverTextStatic, winTextStatic, glitch, staticWhiteLogo, staticWhiteHat, staticBlackLogo, staticBlackHat, restartButton, quitButton;
    public static int num;
    public static Action ResetGameEvent;
    private bool requested = false;
    private PhotonView _myPhotonView;

    void Start()
    {

        if (num == 0)
        {
            Invoke("Show", 0f);
        }

        _myPhotonView = GetComponent<PhotonView>();

    }

    // WORK HERE BVITCH
    
    
    public void OnClickedRematchOnline()
    {
        _myPhotonView.RPC("Rematch", RpcTarget.Others);
        restartButton.gameObject.SetActive(false);
        rematchAnim.gameObject.SetActive(true);
        requested = true;
        Debug.Log("Rematch?");
        
    }

    [PunRPC]
    private void Rematch()
    {
        Debug.Log("Requested");
        if (requested)
        {
            _myPhotonView.RPC("Restart", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Restart()
    {
        SceneManager.LoadScene("OnlineBoard");
    }

    public void OnClickedQuit()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnClickedRestart()
    {
        // ResetGameEvent();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // gameObject.SetActive(false);
        soundtrack.gameObject.SetActive(true);
    }

    public void Show()
    {
        transition.gameObject.SetActive(true);
        // Invoke("Hide", 2f);
        Invoke("Hide", 0);
    }

    // hide animation after X seconds
    public void Hide()
    {
        transition.gameObject.SetActive(false);
        Invoke("Show1", 1f);
    }

    public void Show1()
    {
        background.gameObject.SetActive(true);
        Invoke("Hide1", 1f);
    }

    // hide animation after X seconds
    public void Hide1()
    {
        Invoke("Show2", 0f);
    }


    public void Show2()
    {
        
        whiteLogoAnim.gameObject.SetActive(true);
        
        blackLogoAnim.gameObject.SetActive(true);
        soundtrack.gameObject.SetActive(false);
        Invoke("Hide2", 0f);
    }

    // hide animation after X seconds
    public void Hide2()
    {
        Invoke("Show3", 1f);
    }

    public void Show3()
    {
        
        whiteLogoAnim.gameObject.SetActive(false);
        
       blackLogoAnim.gameObject.SetActive(false);

        
        staticWhiteLogo.gameObject.SetActive(true);
        glitch.gameObject.SetActive(true);

        gameOverTextAnim.gameObject.SetActive(true);
        Invoke("Hide3", 1f);
    }

    // hide animation after X seconds
    public void Hide3()
    {
        Invoke("Show4", 0f);
    }

    public void Show4()
    {
        gameOverTextAnim.gameObject.SetActive(false);
        gameOverTextStatic.gameObject.SetActive(true);
        winTextAnim.gameObject.SetActive(true);
        
        Invoke("Hide4", 1f);
    }

    // hide animation after X seconds
    public void Hide4()
    {
        Invoke("Show5", 0f);
    }


    public void Show5()
    {
        winTextAnim.gameObject.SetActive(false);
        winTextStatic.gameObject.SetActive(true);
        Invoke("Hide5", 1f);
    }

    // hide animation after X seconds
    public void Hide5()
    {
        Invoke("Show6", 0f);
    }

    public void Show6()
    {
        quitButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        
    }


}
