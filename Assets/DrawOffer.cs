using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DrawOffer : MonoBehaviourPunCallbacks
{
    public GameObject drawOfferPanel;
    public GameObject drawScreen;
    private PhotonView _photonView;

    public void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void AcceptOffer()
    {
        drawOfferPanel.gameObject.SetActive(false);
        _photonView.RPC("DrawScreen", RpcTarget.All);
    }

    public void OnClickedCancel()
    {
        drawOfferPanel.gameObject.SetActive(false);
        
    }

    [PunRPC]
    private void DrawScreen()
    {
        drawScreen.gameObject.SetActive(true);
    }
    

}
