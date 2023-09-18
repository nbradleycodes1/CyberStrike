using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Hide", 1f);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }


}
