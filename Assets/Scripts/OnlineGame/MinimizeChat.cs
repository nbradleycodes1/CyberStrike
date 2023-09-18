using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimizeChat : MonoBehaviour
{
    public GameObject minimizeButton, openButton, scrollView, inputBox, sendButton, notification;

    public void OnClickedMinimize()
    {
        openButton.gameObject.SetActive(true);

        minimizeButton.gameObject.SetActive(false);
        scrollView.gameObject.SetActive(false);
        inputBox.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
    }

    public void OnClickedOpenChat()
    {
        if (notification.activeSelf)
        {
            notification.gameObject.SetActive(false);
        }
        openButton.gameObject.SetActive(false);

        minimizeButton.gameObject.SetActive(true);
        scrollView.gameObject.SetActive(true);
        inputBox.gameObject.SetActive(true);
        sendButton.gameObject.SetActive(true);
    }
}
