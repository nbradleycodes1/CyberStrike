using System;
using UnityEngine;

public class AIColorPicker : MonoBehaviour
{
    public static event Action<int> PlayerPickedEvent;
    // Start is called before the first frame update
    public void PickedWhite()
    {
        PlayerPickedEvent(1);
        GameObject.FindGameObjectWithTag("colorpickerscreen").SetActive(false);
    }

    public void PickedBlack()
    {
        PlayerPickedEvent(2);
        GameObject.FindGameObjectWithTag("colorpickerscreen").SetActive(false);
    }
}
