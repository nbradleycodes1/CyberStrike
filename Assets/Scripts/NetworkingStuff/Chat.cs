using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviourPunCallbacks
{
    public InputField chatBox;
    public GameObject Message;
    public GameObject Content;
    public GameObject chatBoard, notification, scrollview;
    private string MessageWithName;


    void Start()
    {
        chatBox.characterLimit = 150;
        MessageWithName = Message.GetComponent<Message>().GetUserName();
        if (!PhotonNetwork.IsMasterClient)
        {
            GetComponent<PhotonView>().RPC("checkName",RpcTarget.Others, MessageWithName);
        }

    }

    void Update()
    {
        
        if (chatBoard.activeSelf && chatBox.text.Trim() != "" && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
        
        
    }

    public void SendMessage()
    {
        
        GetComponent<PhotonView>().RPC("GetMessageOther", RpcTarget.Others, chatBox.text, MessageWithName);
        GetMessage(chatBox.text, MessageWithName);
        GetComponent<PhotonView>().RPC("Notify", RpcTarget.Others);
        chatBox.text = "";
    }

    [PunRPC]
    public void checkName(string name)
    {
        if (name == MessageWithName)
        {
            GetComponent<PhotonView>().RPC("NamePlus1", RpcTarget.Others);
        }
    }
    [PunRPC]
    public void NamePlus1()
    {
        MessageWithName += "(1)";
        Debug.Log(MessageWithName);
        Message.GetComponent<Message>().SetUserName(MessageWithName);
    }
    public void GetMessage(string receivedMessage, string receivedUsername)
    {
        GameObject M = Instantiate(Message, Vector2.zero, Quaternion.identity, Content.transform);
        M.GetComponent<Message>().myMessage.text = receivedMessage;
        M.GetComponent<Message>().receivedUsername.text = receivedUsername;
        
        chatBoard.gameObject.SetActive(false);
        chatBoard.gameObject.SetActive(true);
        
    }
    [PunRPC]
    public void GetMessageOther(string receivedMessage, string receivedUsername)
    {
        GameObject M = Instantiate(Message, Vector2.zero, Quaternion.identity, Content.transform);
        M.GetComponent<Message>().myMessage.text = receivedMessage;
        M.GetComponent<Message>().receivedUsername.text = receivedUsername;
        
    }

    [PunRPC]
    public void Notify()
    {
        if (!chatBoard.activeSelf)
        {
            notification.gameObject.SetActive(true);
        }
    }

}
