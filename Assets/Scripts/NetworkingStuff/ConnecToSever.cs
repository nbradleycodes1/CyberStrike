using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnecToSever : MonoBehaviourPunCallbacks
{

    public GameObject GlitchAnimation2, loadAnim, imageTrans, BackToMenu, Create, Join, Lobby, noConnectionPanel;

    private bool flag = true;
    // Start is called before the first frame update
    void Start()
    {
        /*if(!PhotonNetwork.IsConnected){
            Invoke("Show", 0f);
            loadAnim.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
            //StartCoroutine(Time());
        }

        else {
            Debug.Log("I am already Connected");
            PhotonNetwork.Disconnect();
            Invoke("Show", 0f);
            loadAnim.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
            //StartCoroutine(Time());

        }*/
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        Invoke("Show", 0f);
        loadAnim.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
        
    }

    public void FixedUpdate()
    {
        if (flag && !PhotonNetwork.IsConnected)
        {
            StartCoroutine(Time());
        }
    }

    public void Retry()
    {
        
        //Invoke("Show", 0f);
        loadAnim.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
        //StartCoroutine(Time());
        //Lobby.gameObject.SetActive(true);
            noConnectionPanel.gameObject.SetActive(false);

            flag = true;

        }
    
    public void Show()
    {
        imageTrans.gameObject.SetActive(true);
        Invoke("Hide", 3f);
    }

    // hide animation after X seconds
    public void Hide()
    {
        imageTrans.gameObject.SetActive(false);
    }

    public void Show2()
    {
        GlitchAnimation2.gameObject.SetActive(true);
        Invoke("Hide2", 1f);
    }

    // hide animation after X seconds
    public void Hide2()
    {
        GlitchAnimation2.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby(){
        
        Debug.Log("Lobby loaded");
        loadAnim.SetActive(false);
        Lobby.gameObject.SetActive(true);
        Invoke("Show2", 0f);

    }

    public void OnClickedBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
    IEnumerator Time()
    {
        flag = false;
        yield return new WaitForSeconds(2);
            Lobby.gameObject.SetActive(false);
            noConnectionPanel.gameObject.SetActive(true);
    }

}
