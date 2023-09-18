using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpInBoard : MonoBehaviour
{
    public GameObject helpPanel, glitchAnim;
    bool flag = true;

    public void OnClickedHelp()
    {
        if (flag)
        {
            helpPanel.gameObject.SetActive(true);
            flag = false;
            Invoke("Show", 0f);
        }
    }
    
    public void OnClickedCloseHelp()
    {
        if (!flag)
        {
            helpPanel.gameObject.SetActive(false);
            flag = true;
        }
    }

    private void Show()
    {
        glitchAnim.gameObject.SetActive(true);
        Invoke("Hide", 1f);
    }

    private void Hide()
    {
        glitchAnim.gameObject.SetActive(false);
    }
}
