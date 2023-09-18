using UnityEngine;
using UnityEngine.EventSystems;

public class EnableHiveZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool enabled = true;

    void Awake()
    {
        enabled = true;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //enabled = false;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        enabled = true;
    }
}