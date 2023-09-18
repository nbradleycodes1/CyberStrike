using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteChange : MonoBehaviour
{

    public GameObject sprite1, sprite2;
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
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Menu");
            Debug.Log("A key or mouse click has been detected");
        }
    }

    IEnumerator ToSprite2()
    {
        yield return new WaitForSeconds(2);
        spriteNumber = 1;
        sprite2.gameObject.SetActive(true);
        sprite1.gameObject.SetActive(false);
    }

}
