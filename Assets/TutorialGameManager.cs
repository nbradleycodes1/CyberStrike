using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HiveCore;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static HiveCore.Utils;

public class TutorialGameManager : GameManager
{
    public static event Action <bool, bool> ZoomHiveEvent;
    public static event Action RestartHivePositionEvent;
    public static event Func<GameObject, (int, int), Task> AnimateGameObjectToEvent;
    public GameObject Glitch;
    public int stepNumber = 0, tracker = 0;
    public GameObject msgBox;
    public GameObject Hive, Hand;
    public static bool dragScrollAllowed = false;
    public GameObject msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10;

   // void Start()
   // {
   //     Invoke("Show", 0f);
   // }

    private void Show()
    {
        Glitch.gameObject.SetActive(true);
        Invoke("Hide", 1f);
    }

    private void Hide()
    {
        Glitch.gameObject.SetActive(false);
    }
    private void DisableBlackPieces()
    {
        for (int p = 16; p <= 26; ++p)
        {
            GameObject.FindGameObjectWithTag(p.ToString()).GetComponent<Image>().raycastTarget = false;
        }
    }

    private bool IsPieceDragging(int tag) => PieceGUI(tag).GetComponent<CanvasGroup>().alpha == 0.6f && !PieceGUI(tag).GetComponent<CanvasGroup>().blocksRaycasts;

    private void DisableWhitePieces()
    {
        for (int p = 32; p <= 42; ++p)
        {
            GameObject.FindGameObjectWithTag(p.ToString()).GetComponent<Image>().raycastTarget = false;
        }
    }
    private void EnableWhitePieces()
    {
        for (int p = 32; p <= 42; ++p)
        {
            GameObject.FindGameObjectWithTag(p.ToString()).GetComponent<Image>().raycastTarget = true;
        }
    }

    private bool PieceIsAt(int tag, (int x, int y) pos) => GameObject.FindGameObjectWithTag(tag.ToString()).GetComponent<RectTransform>().anchoredPosition == new Vector2(pos.x, pos.y);

    private void HighlightAt(int x, int y)
    {
        if (GameObject.FindGameObjectsWithTag("tracker").Length <= 2)
        {
            GameObject highlight = Instantiate(Resources.Load("Highlight") as GameObject, new Vector2(x, y), Quaternion.identity);
            highlight.transform.SetParent(GameObject.Find("Canvas/Hive").transform, false);
            highlight.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }

    private void EnablePieceWithTag(int tag)
    {
        GameObject.FindGameObjectWithTag(tag.ToString()).GetComponent<Image>().raycastTarget = true;
    }

    private void HighlightPieceWithTag(int tag)
    {
        Vector2 pos = GameObject.FindGameObjectWithTag(tag.ToString()).GetComponent<RectTransform>().anchoredPosition;
        HighlightAt((int)pos.x, (int)pos.y);
    }

    private void DisablePieceWithTag(string tag)
    {
        GameObject.FindGameObjectWithTag(tag).GetComponent<Image>().raycastTarget = true;
    }

    private void DisplayText(string txt)
    {
        msgBox.GetComponent<TextMeshProUGUI>().text = "";
        msgBox.GetComponent<TextMeshProUGUI>().text = txt;
    }

    private GameObject PieceGUI(int tag) => GameObject.FindGameObjectWithTag(tag.ToString());
    private bool IsOnHive(int tag) => PieceGUI(tag).gameObject.transform.parent.name == "Hive";

    private bool IsOnHand(int tag) => PieceGUI(tag).gameObject.transform.parent.name == "Hand";
    private void moveToHive(int tag) => PieceGUI(tag).GetComponent<RectTransform>().SetParent(GameObject.Find("Canvas/Hive").transform);
    private IEnumerator WaitFor()
    { 
        yield return new WaitForSeconds(5);
    }
    
    private void SelectPieceWithTag(int tag)
    {
        GameObject piece = GameObject.FindGameObjectWithTag(tag.ToString());
        piece.GetComponent<Image>().raycastTarget = true;


        // // Destroy previously displayed tracker placeholders
        // foreach (var placeholder in GameObject.FindGameObjectsWithTag("tracker"))
        // {
        //     Destroy(placeholder);
        // }

        // GameObject prevTracker = Instantiate(Resources.Load("select") as GameObject, new Vector2(prevMove.x, prevMove.y), Quaternion.identity) as GameObject;
        // // needs to get instantiated inside Board (Game Object)
        // prevTracker.transform.SetParent(IsOnHand(prevMove) ? _handGameObj.transform : _hiveGameObj.transform, false);

        // GameObject curTracker = Instantiate(Resources.Load("curmove") as GameObject, new Vector2(curMove.x, curMove.y), Quaternion.identity) as GameObject;
        // // needs to get instantiated inside Board (Game Object)
        // curTracker.transform.SetParent(_hiveGameObj.transform, false);
    }

    private void DestroyTrackers()
    {
        foreach (var placeholder in GameObject.FindGameObjectsWithTag("tracker"))
            Destroy(placeholder);
    }

    async void Update()
    {
        if (dragScrollAllowed)
        {
            // Restart zoom and position
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RestartHivePositionEvent();
                ZoomHiveEvent(false, true);
            }

            // zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (!isOnline || EnableHiveZoom.enabled)
                    ZoomHiveEvent(true, false);
            }
            // zoom out
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (!isOnline || EnableHiveZoom.enabled)
                    ZoomHiveEvent(false, false);
            }
        }

        if (Hive.transform.childCount + Hand.transform.childCount >= 22)
        {
            ShowWhoseTurnPanel();
            if (stepNumber == 0)
            {
                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wS2);
                HighlightAt(-345, -100);
                HighlightAt(0, 0);
                if (tracker == 0)
                {
                    Invoke("Show", 0f);
                    tracker = 1;
                }
                msg1.SetActive(true);
                if (IsOnHive(wS2))
                {
                    DestroyTrackers();
                    stepNumber = 1;
                }
            }
            else if (stepNumber == 1)
            {
                whoseTurn = Player.Black;
                moveToHive(bG3);
                Board.MovePiece(Board.BlackPieces[G3], (0, 56), false);
                await AnimateGameObjectToEvent(PieceGUI(bG3), (0, 56));
                if (IsOnHive(bG3))
                {
                    DestroyTrackers();
                    stepNumber = 2;
                }
            }
            else if (stepNumber == 2)
            {
                if (tracker == 1)
                {
                    Invoke("Show", 0f);
                    tracker = 2;
                }
                msg1?.GetComponent<FadeOut>().Run();
                msg2.SetActive(true);
                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wQ1);
                HighlightAt(Board.WhitePieces[Q1].DefaultPoint.x, Board.WhitePieces[Q1].DefaultPoint.y);
                HighlightAt(-48, -28);
                if (IsOnHive(wQ1))
                {
                    if (PieceGUI(wQ1).GetComponent<RectTransform>().anchoredPosition == new Vector2(-48, -28))
                    {
                        DestroyTrackers();
                        stepNumber = 3;
                    }
                    else
                    {
                        // Undo
                        _MakeValidMove((Board.WhitePieces[Q1], Board.WhitePieces[Q1].DefaultPoint));
                        Board.History[Player.White].RemoveLast();
                    }
                }
            }
            else if (stepNumber == 3)
            {
                whoseTurn = Player.Black;
                DisableBlackPieces();
                DisableWhitePieces();
                moveToHive(bQ1);
                Board.MovePiece(Board.BlackPieces[Q1], (0, 112), false);
                await AnimateGameObjectToEvent(PieceGUI(bQ1), (0, 112));
                if (IsOnHive(bQ1))
                    stepNumber = 4;
            }
            else if (stepNumber == 4)
            {
                if (tracker == 2)
                {
                    Invoke("Show", 0f);
                    tracker = 3;
                }
                msg2?.GetComponent<FadeOut>().Run();
                msg3.SetActive(true);
                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wG3);
                HighlightAt(0, -56);
                HighlightAt(Board.WhitePieces[G3].DefaultPoint.x, Board.WhitePieces[G3].DefaultPoint.y);
                if (IsOnHive(wG3))
                {
                    if (PieceGUI(wG3).GetComponent<RectTransform>().anchoredPosition == new Vector2(0, -56))
                    {
                        DestroyTrackers();
                        stepNumber = 5;
                    }
                    else
                    {
                        // Undo
                        _MakeValidMove((Board.WhitePieces[G3], Board.WhitePieces[G3].DefaultPoint));
                        Board.History[Player.White].RemoveLast();
                    }
                }
            }
            else if (stepNumber == 5)
            {
                whoseTurn = Player.Black;
                DisableBlackPieces();
                DisableWhitePieces();
                moveToHive(bG2);
                Board.MovePiece(Board.BlackPieces[G2], (0, 168), false);
                await AnimateGameObjectToEvent(PieceGUI(bG2), (0, 168));
                if (IsOnHive(bG2))
                    stepNumber = 6;
            }
            else if (stepNumber == 6)
            {
                if (tracker == 3)
                {
                    Invoke("Show", 0f);
                    tracker = 4;
                }

                msg3?.GetComponent<FadeOut>().Run();
                msg4.SetActive(true);

                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wG3);
                HighlightAt(0, 224);
                HighlightAt(0, -56);
                if (!IsPieceDragging(wG3) && IsOnHive(wG3))
                {
                    if (PieceGUI(wG3).GetComponent<RectTransform>().anchoredPosition == new Vector2(0, 224))
                    {
                        DestroyTrackers();
                        stepNumber = 7;
                    }
                    else
                    {
                        if (PieceGUI(wG3).GetComponent<RectTransform>().anchoredPosition != new Vector2(0, -56))
                        {
                            // Undo
                            _MakeValidMove((Board.WhitePieces[G3], (0, -56)));
                            Board.History[Player.White].RemoveLast();
                        }
                    }
                }
            }
            else if (stepNumber == 7)
            {
                dragScrollAllowed = true;
                if (tracker == 4)
                {
                    Invoke("Show", 0f);
                    tracker = 5;
                }


                whoseTurn = Player.Black;
                DisableBlackPieces();
                DisableWhitePieces();

                // spawn black ant 3 | 48, 140
                (int, int) ba3Pos = (48, 140);
                moveToHive(bA3);
                Board.MovePiece(Board.BlackPieces[A3], ba3Pos, false);
                await AnimateGameObjectToEvent(PieceGUI(bA3), ba3Pos);

                if (IsOnHive(bA3))
                {
                    // whoseTurn = Player.White;
                    // spawn white ant 3
                    (int, int) wa3Pos = (0, -56);
                    moveToHive(wA3);
                    Board.MovePiece(Board.WhitePieces[A3], wa3Pos, false);
                    await AnimateGameObjectToEvent(PieceGUI(wA3), wa3Pos);

                    if (IsOnHive(wA3))
                    {
                        // whoseTurn = Player.Black;
                        // spawn black spider 2
                        (int, int) bs2Pos = (96, 112);
                        moveToHive(bS2);
                        Board.MovePiece(Board.BlackPieces[S2], bs2Pos, false);
                        await AnimateGameObjectToEvent(PieceGUI(bS2), bs2Pos);
                        if (IsOnHive(bS2))
                            stepNumber = 8;
                    }
                }
            }
            else if (stepNumber == 8)
            {
                whoseTurn = Player.White;
                if (tracker == 5)
                {
                    Invoke("Show", 0f);
                    tracker = 6;
                }
                // DisplayText("drag and do ur white ant on hive to the ne of bs1");

                msg4?.GetComponent<FadeOut>().Run();
                msg5.SetActive(true);

                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wA3);
                HighlightAt(144, 140);
                HighlightAt(0, -56);

                if (IsOnHive(wA3))
                {
                    if (PieceGUI(wA3).GetComponent<RectTransform>().anchoredPosition == new Vector2(144, 140))
                    {
                        DestroyTrackers();
                        stepNumber = 9;
                    }
                    else
                    {
                        if (PieceGUI(wA3).GetComponent<RectTransform>().anchoredPosition != new Vector2(0, -56))
                        {
                            // Undo
                            _MakeValidMove((Board.WhitePieces[A3], (0, -56)));
                            Board.History[Player.White].RemoveLast();
                        }
                    }
                }
            }
            else if (stepNumber == 9)
            {
                if (tracker == 6)
                {
                    Invoke("Show", 0f);
                    tracker = 7;
                }


                whoseTurn = Player.Black;
                DisableBlackPieces();
                DisableWhitePieces();

                // spawn black spider 1 | -48, 140
                (int, int) bs1Pos = (-48, 140);
                moveToHive(bS1);
                Board.MovePiece(Board.BlackPieces[S1], bs1Pos, false);
                await AnimateGameObjectToEvent(PieceGUI(bS1), bs1Pos);

                if (IsOnHive(bS1))
                {
                    // whoseTurn = Player.White;
                    // spawn white spider 1 | -96, 0
                    (int, int) ws1Pos = (-96, 0);
                    moveToHive(wS1);
                    Board.MovePiece(Board.WhitePieces[S1], ws1Pos, false);
                    await AnimateGameObjectToEvent(PieceGUI(wS1), ws1Pos);

                    if (IsOnHive(wS1))
                    {
                        // whoseTurn = Player.Black;
                        // spawn black grasshopper 1 | 96, 56
                        (int, int) bg1Pos = (96, 56);
                        moveToHive(bG1);
                        Board.MovePiece(Board.BlackPieces[G1], bg1Pos, false);
                        await AnimateGameObjectToEvent(PieceGUI(bG1), bg1Pos);
                        if (IsOnHive(bG1))
                            stepNumber = 10;
                    }
                }
            }
            else if (stepNumber == 10)
            {
                if (tracker == 7)
                {
                    Invoke("Show", 0f);
                    tracker = 8;
                }
                // DisplayText("now do white spider by white queen");

                msg5?.GetComponent<FadeOut>().Run();
                msg6.SetActive(true);
                msg7.SetActive(true);

                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wS1);
                HighlightAt(-96, 112);
                HighlightAt(-96, 0);

                if (IsOnHive(wS1))
                {
                    if (PieceGUI(wS1).GetComponent<RectTransform>().anchoredPosition == new Vector2(-96, 112))
                    {
                        DestroyTrackers();
                        stepNumber = 11;
                    }
                    else
                    {
                        if (PieceGUI(wS1).GetComponent<RectTransform>().anchoredPosition != new Vector2(-96, 0))
                        {
                            // Undo
                            _MakeValidMove((Board.WhitePieces[S1], (-96, 0)));
                            Board.History[Player.White].RemoveLast();
                        }
                    }
                }
            }
            else if (stepNumber == 11)
            {
                whoseTurn = Player.Black;
                (int, int) bg1Pos = (96, 168);
                moveToHive(bG1);
                Board.MovePiece(Board.BlackPieces[G1], bg1Pos, false);
                await AnimateGameObjectToEvent(PieceGUI(bG1), bg1Pos);
                await Task.Delay(250);
                if (PieceIsAt(bG1, bg1Pos))
                {
                    // whoseTurn = Player.White;
                    (int, int) wb2Pos = (0, -56);
                    moveToHive(wB2);
                    Board.MovePiece(Board.WhitePieces[B2], wb2Pos, false);
                    await AnimateGameObjectToEvent(PieceGUI(wB2), wb2Pos);
                    await Task.Delay(500);
                    if (PieceIsAt(wB2, wb2Pos))
                        stepNumber = 12;
                }
            }
            else if (stepNumber == 12)
            {
                whoseTurn = Player.Black;
                (int, int) bg1Pos = (192, 112);
                moveToHive(bG1);
                Board.MovePiece(Board.BlackPieces[G1], bg1Pos, false);
                await AnimateGameObjectToEvent(PieceGUI(bG1), bg1Pos);
                await Task.Delay(500);
                if (PieceIsAt(bG1, bg1Pos))
                        stepNumber = 13;
            }
            else if (stepNumber == 13)
            {
                if (tracker == 8)
                {
                    Invoke("Show", 0f);
                    tracker = 9;
                }
                // DisplayText("try your beetle now");

                msg6?.GetComponent<FadeOut>().Run();
                msg7?.GetComponent<FadeOut>().Run();
                msg8.SetActive(true);


                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wB2);
                HighlightAt(0, 0);
                HighlightAt(0, -56);

                if (IsOnHive(wB2))
                {
                    if (PieceGUI(wB2).GetComponent<RectTransform>().anchoredPosition == new Vector2(0, 0))
                    {
                        DestroyTrackers();
                        stepNumber = 14;
                    }
                    else
                    {
                        if (PieceGUI(wB2).GetComponent<RectTransform>().anchoredPosition != new Vector2(0, -56))
                        {
                            // Undo
                            _MakeValidMove((Board.WhitePieces[B2], (0, -56)));
                            Board.History[Player.White].RemoveLast();
                        }
                    }
                }
            }
            else if (stepNumber == 14)
            {
                // move black ant to 48, 84
                whoseTurn = Player.Black;
                (int, int) bb2Pos = (48, 84);
                moveToHive(bB2);
                Board.MovePiece(Board.BlackPieces[B2], bb2Pos, false);
                await AnimateGameObjectToEvent(PieceGUI(bB2), bb2Pos);
                //await Task.Delay(500); <= This slows the beetle down and everything thereafter a bunch???
                if (PieceIsAt(bB2, bb2Pos))
                {
                        // move black ant to 48, 84
                        // whoseTurn = Player.White;
                        (int, int) wb2Pos = (0, 56);
                        moveToHive(wB2);
                        Board.MovePiece(Board.WhitePieces[B2], wb2Pos, false);
                        await AnimateGameObjectToEvent(PieceGUI(wB2), wb2Pos);
                        await Task.Delay(300);
                        if (PieceIsAt(wB2, wb2Pos))
                            stepNumber = 15;
                }
            }
            else if (stepNumber == 15)
            {
                // move black ant to 48, 84
                // whoseTurn = Player.Black;
                (int, int) bb2Pos = (0, 56);
                moveToHive(bB2);
                Board.MovePiece(Board.BlackPieces[B2], bb2Pos, false);
                await AnimateGameObjectToEvent(PieceGUI(bB2), bb2Pos);
                await Task.Delay(300);
                if (PieceIsAt(bB2, bb2Pos))
                    stepNumber = 16;
            }
            else if (stepNumber == 16)
            {
                if (tracker == 9)
                {
                    Invoke("Show", 0f);
                    tracker = 10;
                }
                // DisplayText("that darn beetle. check it out and move your queen");
                msg8?.GetComponent<FadeOut>().Run();
                msg9.SetActive(true);



                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(bB2);
                EnablePieceWithTag(wQ1);
                HighlightAt(0, -56);
                HighlightAt(-48, -28);

                if (IsOnHive(wQ1))
                {
                    if (PieceGUI(wQ1).GetComponent<RectTransform>().anchoredPosition == new Vector2(0, -56))
                    {
                        DestroyTrackers();
                        stepNumber = 17;
                    }
                    else
                    {
                        if (PieceGUI(wQ1).GetComponent<RectTransform>().anchoredPosition != new Vector2(-48, -28))
                        {
                            // Undo
                            _MakeValidMove((Board.WhitePieces[Q1], (-48, -28)));
                            Board.History[Player.White].RemoveLast();
                        }
                    }
                }
            }
            else if (stepNumber == 17)
            {
                // move black ant to 48, 84
                whoseTurn = Player.Black;
                (int, int) bb1Pos = (48, 84);
                moveToHive(bB1);
                Board.MovePiece(Board.BlackPieces[B1], bb1Pos, false);
                await AnimateGameObjectToEvent(PieceGUI(bB1), bb1Pos);
                await Task.Delay(500);
                if (PieceIsAt(bB1, bb1Pos))
                {
                        // whoseTurn = Player.White;
                        (int, int) wa2Pos = (-144, 84);
                        moveToHive(wA2);
                        Board.MovePiece(Board.WhitePieces[A2], wa2Pos, false);
                        await AnimateGameObjectToEvent(PieceGUI(wA2), wa2Pos);
                        await Task.Delay(250);
                        if (PieceIsAt(wA2, wa2Pos))
                        {
                            // whoseTurn = Player.Black;
                            (int, int) ba2Pos = (192, 56);
                            moveToHive(bA2);
                            Board.MovePiece(Board.BlackPieces[A2], ba2Pos, false);
                            await AnimateGameObjectToEvent(PieceGUI(bA2), ba2Pos);
                            await Task.Delay(250);
                            if (PieceIsAt(bA2, ba2Pos))
                                stepNumber = 18;
                        }
                }
            }
            else if (stepNumber == 18)
            {
                if (tracker == 10)
                {
                    Invoke("Show", 0f);
                    tracker = 11;
                }
                msg9?.GetComponent<FadeOut>().Run();
                msg10.SetActive(true);
                whoseTurn = Player.White;
                DisableBlackPieces();
                DisableWhitePieces();
                EnablePieceWithTag(wA2);
                EnablePieceWithTag(bB2); // to allow the hover to still work
                HighlightAt(-144, 84);

                if (IsOnHive(wA2))
                {
                    if (PieceGUI(wA2).GetComponent<RectTransform>().anchoredPosition == new Vector2(-48, 84))
                    {
                        DestroyTrackers();
                        stepNumber = 19;
                    }
                    else
                    {
                        if (PieceGUI(wA2).GetComponent<RectTransform>().anchoredPosition != new Vector2(-144, 84))
                        {
                            // Undo
                            _MakeValidMove((Board.WhitePieces[A2], (-144, 84)));
                            Board.History[Player.White].RemoveLast();
                        }
                    }
                }
            }
            else if (stepNumber == 19)
            {
                if (tracker == 11)
                {
                    Invoke("Show", 0f);
                    // tracker++;
                }
                // DisplayText("Good job!");
                _DetectGameOver();
            }
        }
    }
}