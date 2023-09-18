using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Page5QueenIllegal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject queenBlack, arrow, grid, illegalPiece, grid2;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        illegalPiece.gameObject.SetActive(true);
        grid2.gameObject.SetActive(true);

        arrow.gameObject.SetActive(false);
        grid.gameObject.SetActive(false);
        queenBlack.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        arrow.gameObject.SetActive(true);
        grid.gameObject.SetActive(true);
        queenBlack.gameObject.SetActive(true);

        illegalPiece.gameObject.SetActive(false);
        grid2.gameObject.SetActive(false);
    }
}
