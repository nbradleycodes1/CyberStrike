using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfferRematch : MonoBehaviour
{
    public GameObject rematchValidation;

    public void OnClickedCancelRematch()
    {
        rematchValidation.gameObject.SetActive(true);
    }
    

    public void OnClickedAccept()
    {
        
    }

    public void OnClickedQuit()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void OnClickedQuitBack()
    {
        rematchValidation.gameObject.SetActive(false);
    }
}
