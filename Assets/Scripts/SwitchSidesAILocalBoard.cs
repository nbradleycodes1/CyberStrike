using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSidesAILocalBoard : MonoBehaviour
{
    public static event Action SwitchSides;
    public void OnClickedYesSwitchSides()
    {
        // insert the nasty switch function here
        // SwitchSides();
        // Debug.Log($"did it {GameManager.switchedSides}");
        GameManager.switchedSides = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // GameObject.Find("Pause Menu Canvas").SetActive(false);
    }
}
