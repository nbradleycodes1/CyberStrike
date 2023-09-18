using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlitchPanel : MonoBehaviour
{

    public GameObject glitch;
    public static int spriteNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (spriteNumber == 0)
        {
            StartCoroutine(ToSprite2());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ToSprite2()
    {
        yield return new WaitForSeconds(1);
        spriteNumber = 1;
        glitch.gameObject.SetActive(false);
    }

}
