using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MenuNetwork : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Debug.Log("Menu was connected but now it's not");
        }
    }
}
