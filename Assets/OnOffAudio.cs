using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnOffAudio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Start is called before the first frame update
    [SerializeField] GameObject slider, offIcon, lowMedIcon, highMedIcon, onIcon;


    void Awake()
    {
        byte val = 250;
        offIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        lowMedIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        highMedIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        onIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slider.gameObject.active)
            slider.GetComponent<HoverFade>().MakeFadeOut();
        else
            slider.GetComponent<HoverFade>().MakeFadeIn();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<AudioSource>().Play();
        byte val = 150;
        offIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        lowMedIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        highMedIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        onIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<AudioSource>().Play();
        byte val = 250;
        offIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        lowMedIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        highMedIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
        onIcon.GetComponent<Image>().color = new Color32 (val, val, val, 255);
    }
}
