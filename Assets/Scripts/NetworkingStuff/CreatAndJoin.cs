using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreatAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField joinInput;
    public TMP_Text errorMessage;
    private string lowerLobby;

    void Start()
    {
        joinInput.characterLimit = 15;
    }
    public void CreateRoom()
    {
        if (joinInput.text != "")
        {
            lowerLobby = joinInput.text.ToLower();
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            this.GetComponent<Message>().SetUserName();
            PhotonNetwork.CreateRoom(lowerLobby, roomOptions, null);
        }
        else
        {
            errorMessage.text = "Please enter Lobby Name!";
        }
    }

    public void JoinRoom()
    {
        if (joinInput.text != "")
        {
            lowerLobby = joinInput.text.ToLower();
            this.GetComponent<Message>().SetUserName();
            PhotonNetwork.JoinRoom(lowerLobby);
        }
        else
        {
            errorMessage.text = "Please enter Lobby Name!";
        }

    }

    public override void OnJoinedRoom(){
        PhotonNetwork.LoadLevel("OnlineBoard");

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorMessage.text = message;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorMessage.text = message;
    }

    public void OnJoinRandomFailed(){
        Debug.Log("Failed to join room");
    }
}
