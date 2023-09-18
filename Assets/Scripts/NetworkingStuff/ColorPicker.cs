using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviourPunCallbacks
{

    public TMP_Text whiteName, blackName, waitMessage, lobbyName;
    private PhotonView _photonView;
    private string _displayName;
    public GameObject message, panel, panel1;
    public GameObject readyButton;
    public GameObject colorPicker;
    public static Action<int> ColorWasPickedEvent;
    public GameObject whiteButton, blackButton;
    private int color;
    //private Color white, black;
    void Start()
    {
        lobbyName.text = PhotonNetwork.CurrentRoom.Name;
        _photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            whiteButton.gameObject.SetActive(false);
            blackButton.gameObject.SetActive(false);
        }
        else
        {
            waitMessage.gameObject.SetActive(false);
        }
        panel.gameObject.SetActive(false);
        panel1.gameObject.SetActive(false);
        //white = whiteButton.GetComponent<Button>().GetComponent<Image>().color;
        //black = blackButton.GetComponent<Button>().GetComponent<Image>().color;
        
    }

    void Update()
    {
        _displayName = message.GetComponent<Message>().GetUserName();
        
        if (whiteName.text != "" && blackName.text != "")
        {
            readyButton.SetActive(true);
        }
        else
        {
            readyButton.SetActive(false);
        }
    }

    public int GetColor()
    {
        return color;
    }
    public void PickedWhite()
    {
        if (whiteName.text == "" && (blackName.text == _displayName || blackName.text == ""))
        {
            _photonView.RPC("SetWhite", RpcTarget.All, _displayName);
            //panel.gameObject.SetActive(true);
           // panel1.gameObject.SetActive(false);
            _photonView.RPC("SetBlack", RpcTarget.All, "");

        }
        else if (whiteName.text == "")
        {
            _photonView.RPC("SetWhite", RpcTarget.All, _displayName);
            //panel.gameObject.SetActive(true);
        }
        else if (whiteName.text == _displayName)
        {
            _photonView.RPC("ResetWhite", RpcTarget.All);
            panel.gameObject.SetActive(false);
        }
    }

    public void PickedBlack()
    {
        if (blackName.text == "" && (whiteName.text == _displayName || whiteName.text == ""))
        {
            _photonView.RPC("SetBlack", RpcTarget.All, _displayName);
           // panel1.gameObject.SetActive(true);
            //panel.gameObject.SetActive(false);
            _photonView.RPC("SetWhite", RpcTarget.All, "");
            
        }
        else if (blackName.text == "")
        {
            _photonView.RPC("SetBlack", RpcTarget.All, _displayName);
        }
        else if (blackName.text == _displayName)
        {
            _photonView.RPC("ResetBlack", RpcTarget.All);
            //panel1.gameObject.SetActive(false);
        }
    }

    public void HackerReady()
    {
        _photonView.RPC("LoadBoard", RpcTarget.All);
    }
    [PunRPC]
    private void SetBlack(string displayName)
    {
        blackName.text = displayName;
    }
    [PunRPC]
    private void SetWhite(string displayName)
    {
        whiteName.text = displayName;
    }

    [PunRPC]
    private void ResetWhite()
    {
        whiteName.text = "";
    }

    [PunRPC]
    private void ResetBlack()
    {
        blackName.text = "";
    }

    [PunRPC]
    private void LoadBoard()
    {
        if (whiteName.text == _displayName)
        {
           //this.GetComponent<DragDrop>().SetMyColor(1);
           //ColorWasPickedEvent(1);
           color = 1;
        }
        else if(blackName.text == _displayName)
        {
            //ColorWasPickedEvent(2);
            // GetComponent<DragDrop>()
            color = 2;
        }

        ColorWasPickedEvent(color);
       colorPicker.gameObject.SetActive(false);
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        whiteButton.gameObject.SetActive(true);
        blackButton.gameObject.SetActive(true);
        waitMessage.gameObject.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"dude left {otherPlayer.ActorNumber}");
        
    }
}
