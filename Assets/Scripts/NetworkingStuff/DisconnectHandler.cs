using System.Collections;
using System.Collections.Generic;
using HiveCore;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public class DisconnectHandler : MonoBehaviourPunCallbacks
{
    public GameObject opponentDisconnect;
    public GameObject disconnected;
    public bool otherLeft = false;
    void Start()
    {
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Instantiate(opponentDisconnect);
        otherLeft = true;
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (!otherLeft)
        {
            Instantiate(disconnected);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
