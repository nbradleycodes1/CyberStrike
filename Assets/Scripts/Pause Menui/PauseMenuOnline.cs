using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class PauseMenuOnline : MonoBehaviour
{
    public GameObject pause,
        resumeGame,
        forfeitGame,
        toMainMenu,
        quitValidation,
        pauseMenuText,
        drawButton,
        surrenderValidation,
        drawOffer,
        whiteWin,
        blackWin, borderAnim1, borderAnim2, borderAnim3, borderAnim4;
    public GameManager theGM;
    bool flag = false;
    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!GameManager.isGameOver && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse2)))
        {
            if (flag == false)
            {
                pause.gameObject.SetActive(true);
                flag = true;
                resumeGame.gameObject.SetActive(true);
                forfeitGame.gameObject.SetActive(true);
                toMainMenu.gameObject.SetActive(true);
                pauseMenuText.gameObject.SetActive(true);
                drawButton.gameObject.SetActive(true);
            }

            else
            {
                pause.gameObject.SetActive(false);
                flag = false;
                quitValidation.gameObject.SetActive(false);
                surrenderValidation.gameObject.SetActive(false);
                borderAnim1.gameObject.SetActive(false);
                borderAnim2.gameObject.SetActive(false);
                borderAnim3.gameObject.SetActive(false);
                borderAnim4.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickedResumeGame()
    {
        pause.gameObject.SetActive(false);
    }

    public void OnClickedForfeitGame()
    {
        surrenderValidation.gameObject.SetActive(true);

        resumeGame.gameObject.SetActive(false);
        forfeitGame.gameObject.SetActive(false);
        toMainMenu.gameObject.SetActive(false);
        pauseMenuText.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
    }

    public void OnClickedQuit()
    {
        quitValidation.gameObject.SetActive(true);

        resumeGame.gameObject.SetActive(false);
        forfeitGame.gameObject.SetActive(false);
        toMainMenu.gameObject.SetActive(false);
        pauseMenuText.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        borderAnim1.gameObject.SetActive(false);
        borderAnim2.gameObject.SetActive(false);
        borderAnim3.gameObject.SetActive(false);
        borderAnim4.gameObject.SetActive(false);
    }

    public void OnClickedOfferDraw()
    {
        _photonView.RPC("OfferReceived", RpcTarget.Others);
        pause.gameObject.SetActive(false);
    }
    
    [PunRPC]
    private void OfferReceived()
    {
        drawOffer.gameObject.SetActive(true);
    }

    public void OnCLickedYes()
    {
        //PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }

    public void OnCLickedNo()
    {
        quitValidation.gameObject.SetActive(false);

        resumeGame.gameObject.SetActive(true);
        forfeitGame.gameObject.SetActive(true);
        toMainMenu.gameObject.SetActive(true);
        pauseMenuText.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(true);
    }

    public void OnCLickedSurrender()
    {
        _photonView.RPC("Surrender", RpcTarget.All, this.GetComponent<ColorPicker>().GetColor());
    }

    [PunRPC]
    public void Surrender(int surrendered){
        pause.gameObject.SetActive(false);
        if (surrendered == 1)
        {
            blackWin.gameObject.SetActive(true);
        }
        else if (surrendered == 2)
        {
            whiteWin.gameObject.SetActive(true);
        }
    }
    public void OnCLickedNoSurrender()
    {
        surrenderValidation.gameObject.SetActive(false);

        resumeGame.gameObject.SetActive(true);
        forfeitGame.gameObject.SetActive(true);
        toMainMenu.gameObject.SetActive(true);
        pauseMenuText.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(true);
        borderAnim1.gameObject.SetActive(false);
        borderAnim2.gameObject.SetActive(false);
        borderAnim3.gameObject.SetActive(false);
        borderAnim4.gameObject.SetActive(false);
    }
}
