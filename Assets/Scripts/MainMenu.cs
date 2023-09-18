using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject creditsScene, playerVsAIButton, singleButton, versusPlayPanel, versusPanel, tutPanelNew, head_animation;
    public GameObject easyButton, hardButton, tutorialButton, multiplayerButton, backButton1, backButton2, newTutButton, tutorialDescription, easyDescription, hardDescription;
    public GameObject startButton, creditsButton, quitButton, quitPanel, hardPanel, cancelPlayerVsAI, easyPanel, tutorialPanel, tutPanelTrue, playerVsAIPanel;
    public GameObject /*GlitchAnimation*/ GlitchAnimation2, /*GlitchAnimation3, */GlitchAnimation4, player1Tab, player2Tab;
    public GameObject toEasy, toHard, backFromHard, toTutorial, arrowRight1, arrowRight2, arrowLeft1, arrowLeft2, fileFilling, fileBanner, playTutorial, playEasy, playHard, playerVsAIScreen;
    public GameObject  multiPlayPanel, loadLobby, blackScreen, multiPanel;
    public GameObject tutTab, closeTutTab, piecesText, piece1, piece2, piece3, piece4, piece5, QueenImg, PiecesPage2, Page2Panel, Page2Text, ToPage2, ToPage3, ToPage4, ToPage5, ToPage6, ToPage7, ToPage8;
    public GameObject Page3Text, Page3Text2, Page3Panel, Page3Panel2, Page3Images1, Page3Images2;
    public GameObject Page4Text, Page4Panel, Page4Panel2, Page4Pieces;
    public GameObject Page5Text, Page5Text2, Page5Panel, Page5Panel2, Page5Panel3, Page5Panel4, PiecePage5Queen, PiecesPage52, PiecesPage51, PiecePage5CScript;
    public GameObject Page6Text, Page6Panel, Page61Main, Page6Pieces1, Page6Panel1, Page6Panel2, PiecesPage62, Page62Main, ChangeTo2, ChangeTo1, PiecesPage63tab, PiecesPage63Main;
    public GameObject Page7Text, Page7Pieces, Page7Back, Page7Arrow, Page8Text;

    public void OnClickedStart()
    {
        playerVsAIButton.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);
        backButton1.gameObject.SetActive(true);
        newTutButton.gameObject.SetActive(true);

        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
        backButton2.gameObject.SetActive(false);
        creditsScene.gameObject.SetActive(false);

    }

    public void OnClikedBackfromCredits()
    {
        creditsScene.gameObject.SetActive(false);
        backButton2.gameObject.SetActive(false);

        startButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditsButton.gameObject.SetActive(true);
    }
    public void OnClickedBackFromStart()
    {
        newTutButton.gameObject.SetActive(false);
        playerVsAIButton.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(false);
        backButton1.gameObject.SetActive(false);

        startButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditsButton.gameObject.SetActive(true);
    }

    public void OnClickBackFromVsAI()
    {
        newTutButton.gameObject.SetActive(true);
        playerVsAIButton.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);
        backButton1.gameObject.SetActive(true);

        easyButton.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);
        hardButton.gameObject.SetActive(false);

        playerVsAIPanel.gameObject.SetActive(false);
    }



    public void OnClickedOpenCredits()
    {

        creditsScene.gameObject.SetActive(true);
        backButton2.gameObject.SetActive(true);


        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
    }

    /// <summary>/////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void OnClickVsAIMode()
    {
        Invoke("Show2", 0f);
        playerVsAIScreen.gameObject.SetActive(true);
        easyButton.gameObject.SetActive(true);
        playEasy.gameObject.SetActive(true);
        arrowRight2.gameObject.SetActive(true);
        toHard.gameObject.SetActive(true);
        

        newTutButton.gameObject.SetActive(false);
        playerVsAIButton.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(false);
        head_animation.gameObject.SetActive(false);
        backButton1.gameObject.SetActive(false);
        playerVsAIPanel.gameObject.SetActive(false);   

    }

    public void OnClickBackFromVsAIMode()
    {
        head_animation.gameObject.SetActive(true);
        backButton1.gameObject.SetActive(true);
        newTutButton.gameObject.SetActive(true);
        playerVsAIButton.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);

        playerVsAIScreen.gameObject.SetActive(false);
        arrowLeft1.gameObject.SetActive(false);
        hardButton.gameObject.SetActive(false);
        easyDescription.gameObject.SetActive(false);
        playHard.gameObject.SetActive(false);
        backFromHard.gameObject.SetActive(false);
    }
/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

    // Versus Screen
    public void OnClickVersus()
    {
        versusPlayPanel.gameObject.SetActive(true);

        newTutButton.gameObject.SetActive(false);
        playerVsAIButton.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(false);
        head_animation.gameObject.SetActive(false);
        backButton1.gameObject.SetActive(false);
        versusPanel.gameObject.SetActive(false);
    }

    public void Show4()
    {
        GlitchAnimation4.gameObject.SetActive(true);
        Invoke("Hide4", 5f);
    }

    // hide animation after X seconds
    public void Hide4()
    {
        GlitchAnimation4.gameObject.SetActive(false);
    }

    //public void Show3()
    //{
    //    GlitchAnimation3.gameObject.SetActive(true);
    //    Invoke("Hide3", 1f);
    //}

    // hide animation after X seconds
    //public void Hide3()
    //{
    //    GlitchAnimation3.gameObject.SetActive(false);
    //}

    public void Show2()
    {
        GlitchAnimation2.gameObject.SetActive(true);
        Invoke("Hide2", 1f);
    }

    // hide animation after X seconds
    public void Hide2()
    {
        GlitchAnimation2.gameObject.SetActive(false);
    }

    // show animation
    //public void Show()
    //{
    //    GlitchAnimation.gameObject.SetActive(true);
    //    Invoke("Hide", 1f);
    //}

    //// hide animation after X seconds
    //public void Hide()
    //{
    //    GlitchAnimation.gameObject.SetActive(false);
    //}

    //public void OnClickBackFromVersus()
    //{
    //    head_animation.gameObject.SetActive(true);
    //    backButton1.gameObject.SetActive(true);
    //    playerVsAIButton.gameObject.SetActive(true);
    //    singleButton.gameObject.SetActive(true);
    //    multiplayerButton.gameObject.SetActive(true);

    //    singleVersus.gameObject.SetActive(false);
    //    cancelVersus.gameObject.SetActive(false);
    //    startVersus.gameObject.SetActive(false);
    //}

    //public void OnClickedPlayVersus()
    //{
    //    versusPlayPanel.gameObject.SetActive(true);
    //    player1Tab.gameObject.SetActive(false);
    //    player2Tab.gameObject.SetActive(false);
    //    startVersus.gameObject.SetActive(false);
    //    cancelVersus.gameObject.SetActive(false);
    //}

    public void OnClickedYesVersus()
    {
        loadLobby.gameObject.SetActive(true);
        StartCoroutine(ToLocalBoard());
    }

    public void OnClickedNoVersus()
    {
        playerVsAIButton.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);
        head_animation.gameObject.SetActive(true);
        backButton1.gameObject.SetActive(true);

        versusPlayPanel.gameObject.SetActive(false);
    }


    //Easy Button click start/////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void OnCLickEasyNo()
    {
        playEasy.gameObject.SetActive(true);
        fileBanner.gameObject.SetActive(true);
        fileFilling.gameObject.SetActive(true);
        cancelPlayerVsAI.gameObject.SetActive(true);
        arrowRight2.gameObject.SetActive(true);
        toHard.gameObject.SetActive(true);
        easyButton.gameObject.SetActive(true);

        easyDescription.gameObject.SetActive(false);
        easyPanel.gameObject.SetActive(false);
    }

    public void OnCLickEasyYes()
    {
        loadLobby.gameObject.SetActive(true);
        StartCoroutine(ToAiEasy());
    }
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    // Hard Button click start
    public void OnCLickHardYes()
    {
        loadLobby.gameObject.SetActive(true);
        StartCoroutine(ToAiHard());
    }

    public void OnCLickHardNo()
    {
        playHard.gameObject.SetActive(true);
        fileBanner.gameObject.SetActive(true);
        fileFilling.gameObject.SetActive(true);
        cancelPlayerVsAI.gameObject.SetActive(true);
        arrowLeft1.gameObject.SetActive(true);
        backFromHard.gameObject.SetActive(true);
        hardButton.gameObject.SetActive(true);

        hardDescription.gameObject.SetActive(false);
        hardPanel.gameObject.SetActive(false);
    }

    //Tutorial Button click start////////////////////////////////////////////////////////////////////////////////////////
    public void OnCLickTutorialYes()
    {
        Invoke("Show4", 0f);
        tutTab.gameObject.SetActive(true);

        newTutButton.gameObject.SetActive(false);
        playerVsAIButton.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(false);
        head_animation.gameObject.SetActive(false);
        backButton1.gameObject.SetActive(false);
        tutPanelNew.gameObject.SetActive(false);
    }

    public void OnCLickTutorialNo()
    {
        newTutButton.gameObject.SetActive(true);
        playerVsAIButton.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);
        backButton1.gameObject.SetActive(true);
        head_animation.gameObject.SetActive(true);

        tutPanelNew.gameObject.SetActive(false);
        tutPanelTrue.gameObject.SetActive(false);
    }

    public void OnClickInGame()
    {
        //walktrough scene here
        Debug.Log("In-Game walkthrough");
        loadLobby.gameObject.SetActive(true);
        StartCoroutine(ToTutorial());
    }

    public void OnClickStartTutorialMode()
    {
        tutPanelTrue.gameObject.SetActive(true);
        ToPage2.gameObject.SetActive(true);
        Page3Images1.gameObject.SetActive(true);
        Page3Images2.gameObject.SetActive(true);
        Page3Panel.gameObject.SetActive(true);
        Page3Panel2.gameObject.SetActive(true);
        Page3Text.gameObject.SetActive(true);
        Page3Text2.gameObject.SetActive(true);

        newTutButton.gameObject.SetActive(false);
        head_animation.gameObject.SetActive(false);
        playerVsAIButton.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(false);
        backButton1.gameObject.SetActive(false);
        tutPanelNew.gameObject.SetActive(false);

    }

    public void OnClickedCloseTutTab()
    {
        newTutButton.gameObject.SetActive(true);
        playerVsAIButton.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);
        head_animation.gameObject.SetActive(true);
        backButton1.gameObject.SetActive(true);

        tutPanelNew.gameObject.SetActive(false);
        tutTab.gameObject.SetActive(false);
        tutPanelTrue.gameObject.SetActive(false);
        tutorialPanel.gameObject.SetActive(false);
        ToPage3.gameObject.SetActive(false);
        Page2Panel.gameObject.SetActive(false);
        Page2Text.gameObject.SetActive(false);
        QueenImg.gameObject.SetActive(false);
        PiecesPage2.gameObject.SetActive(false);
        ToPage4.gameObject.SetActive(false);
        Page3Images1.gameObject.SetActive(false);
        Page3Images2.gameObject.SetActive(false);
        Page3Panel.gameObject.SetActive(false);
        Page3Panel2.gameObject.SetActive(false);
        Page3Text.gameObject.SetActive(false);
        Page3Text2.gameObject.SetActive(false);
        ToPage5.gameObject.SetActive(false);
        Page4Panel.gameObject.SetActive(false);
        Page4Panel2.gameObject.SetActive(false);
        Page4Text.gameObject.SetActive(false);
        Page4Pieces.gameObject.SetActive(false);
        ToPage6.gameObject.SetActive(false);
        Page5Panel.gameObject.SetActive(false);
        Page5Panel2.gameObject.SetActive(false);
        Page5Panel3.gameObject.SetActive(false);
        Page5Panel4.gameObject.SetActive(false);
        Page5Text.gameObject.SetActive(false);
        Page5Text2.gameObject.SetActive(false);
        PiecePage5CScript.gameObject.SetActive(false);
        PiecePage5Queen.gameObject.SetActive(false);
        PiecesPage51.gameObject.SetActive(false);
        PiecesPage52.gameObject.SetActive(false);
        Page61Main.gameObject.SetActive(false);
        Page62Main.gameObject.SetActive(false);
        Page6Panel.gameObject.SetActive(false);
        Page6Panel1.gameObject.SetActive(false);
        Page6Panel2.gameObject.SetActive(false);
        Page6Pieces1.gameObject.SetActive(false);
        ToPage7.gameObject.SetActive(false);
        PiecesPage62.gameObject.SetActive(false);
        ChangeTo2.gameObject.SetActive(false);
        Page6Text.gameObject.SetActive(false);
        ChangeTo1.gameObject.SetActive(false);
        PiecesPage63tab.gameObject.SetActive(false);
        PiecesPage63Main.gameObject.SetActive(false);
        Page7Arrow.gameObject.SetActive(false);
        Page7Back.gameObject.SetActive(false);
        Page7Pieces.gameObject.SetActive(false);
        Page7Text.gameObject.SetActive(false);
        ToPage4.gameObject.SetActive(false);
        piece1.gameObject.SetActive(false);
        piece2.gameObject.SetActive(false);
        piece3.gameObject.SetActive(false);
        piece4.gameObject.SetActive(false);
        piece5.gameObject.SetActive(false);
        piecesText.gameObject.SetActive(false);
        Page8Text.gameObject.SetActive(false);
        ToPage8.gameObject.SetActive(false);
    }

    // TUT PANEL

    public void OnClickedToPage2()
    {
        
        ToPage3.gameObject.SetActive(true);
        Page2Panel.gameObject.SetActive(true);
        Page2Text.gameObject.SetActive(true);
        QueenImg.gameObject.SetActive(true);
        PiecesPage2.gameObject.SetActive(true);

        Page3Images1.gameObject.SetActive(false);
        Page3Images2.gameObject.SetActive(false);
        Page3Panel.gameObject.SetActive(false);
        Page3Panel2.gameObject.SetActive(false);
        Page3Text.gameObject.SetActive(false);
        Page3Text2.gameObject.SetActive(false);
        ToPage2.gameObject.SetActive(false);
        ToPage8.gameObject.SetActive(false);
        ToPage7.gameObject.SetActive(false);
        ToPage6.gameObject.SetActive(false);
        ToPage5.gameObject.SetActive(false);
        ToPage4.gameObject.SetActive(false);
    }

    public void OnClickedToPage3()
    {
        ToPage4.gameObject.SetActive(true);

        piece1.gameObject.SetActive(true);
        piece2.gameObject.SetActive(true);
        piece3.gameObject.SetActive(true);
        piece4.gameObject.SetActive(true);
        piece5.gameObject.SetActive(true);
        piecesText.gameObject.SetActive(true);

        ToPage3.gameObject.SetActive(false);
        Page2Panel.gameObject.SetActive(false);
        Page2Text.gameObject.SetActive(false);
        QueenImg.gameObject.SetActive(false);
        PiecesPage2.gameObject.SetActive(false);
        ToPage8.gameObject.SetActive(false);
        ToPage7.gameObject.SetActive(false);
        ToPage6.gameObject.SetActive(false);
        ToPage5.gameObject.SetActive(false);
        ToPage2.gameObject.SetActive(false);
    }
    
    public void OnClickedToPage4()
    {
        ToPage5.gameObject.SetActive(true);
        Page4Panel.gameObject.SetActive(true);
        Page4Panel2.gameObject.SetActive(true);
        Page4Text.gameObject.SetActive(true);
        Page4Pieces.gameObject.SetActive(true);

        ToPage4.gameObject.SetActive(false);
        piece1.gameObject.SetActive(false);
        piece2.gameObject.SetActive(false);
        piece3.gameObject.SetActive(false);
        piece4.gameObject.SetActive(false);
        piece5.gameObject.SetActive(false);
        piecesText.gameObject.SetActive(false);
        ToPage8.gameObject.SetActive(false);
        ToPage7.gameObject.SetActive(false);
        ToPage6.gameObject.SetActive(false);
        ToPage3.gameObject.SetActive(false);
        ToPage2.gameObject.SetActive(false);
    }

    public void OnClickedToPage5()
    {
        ToPage6.gameObject.SetActive(true);
        Page5Panel.gameObject.SetActive(true);
        Page5Panel2.gameObject.SetActive(true);
        Page5Panel3.gameObject.SetActive(true);
        Page5Panel4.gameObject.SetActive(true);
        Page5Text.gameObject.SetActive(true);
        Page5Text2.gameObject.SetActive(true);
        PiecePage5CScript.gameObject.SetActive(true);
        PiecePage5Queen.gameObject.SetActive(true);
        PiecesPage51.gameObject.SetActive(true);
        PiecesPage52.gameObject.SetActive(true);

        ToPage5.gameObject.SetActive(false);
        Page4Panel.gameObject.SetActive(false);
        Page4Panel2.gameObject.SetActive(false);
        Page4Text.gameObject.SetActive(false);
        Page4Pieces.gameObject.SetActive(false);

    }

    public void OnClickedToPage6()
    {
        Page61Main.gameObject.SetActive(true);
        Page62Main.gameObject.SetActive(true);
        Page6Panel.gameObject.SetActive(true);
        Page6Panel1.gameObject.SetActive(true);
        Page6Panel2.gameObject.SetActive(true);
        Page6Pieces1.gameObject.SetActive(true);
        ToPage7.gameObject.SetActive(true);
        PiecesPage62.gameObject.SetActive(true);
        ChangeTo2.gameObject.SetActive(true);
        Page6Text.gameObject.SetActive(true);

        ToPage6.gameObject.SetActive(false);
        Page5Panel.gameObject.SetActive(false);
        Page5Panel2.gameObject.SetActive(false);
        Page5Panel3.gameObject.SetActive(false);
        Page5Panel4.gameObject.SetActive(false);
        Page5Text.gameObject.SetActive(false);
        Page5Text2.gameObject.SetActive(false);
        PiecePage5CScript.gameObject.SetActive(false);
        PiecePage5Queen.gameObject.SetActive(false);
        PiecesPage51.gameObject.SetActive(false);
        PiecesPage52.gameObject.SetActive(false);
    }

    //Change To example 2 page 6
    public void OnClickedChangeToEx2()
    {
        ChangeTo1.gameObject.SetActive(true);
        PiecesPage63tab.gameObject.SetActive(true);
        PiecesPage63Main.gameObject.SetActive(true);

        Page6Pieces1.gameObject.SetActive(false);
        ChangeTo2.gameObject.SetActive(false);
        Page61Main.gameObject.SetActive(false);
    }

    //Change To example 1 page 6
    public void OnClickedChangeToEx1()
    {
        ChangeTo1.gameObject.SetActive(false);
        PiecesPage63tab.gameObject.SetActive(false);
        PiecesPage63Main.gameObject.SetActive(false);

        Page6Pieces1.gameObject.SetActive(true);
        ChangeTo2.gameObject.SetActive(true);
        Page61Main.gameObject.SetActive(true);
    }

    public void OnClickedToPage7()
    {
        Page8Text.gameObject.SetActive(true);
        ToPage8.gameObject.SetActive(true);

        Page61Main.gameObject.SetActive(false);
        Page62Main.gameObject.SetActive(false);
        Page6Panel.gameObject.SetActive(false);
        Page6Panel1.gameObject.SetActive(false);
        Page6Panel2.gameObject.SetActive(false);
        Page6Pieces1.gameObject.SetActive(false);
        ToPage7.gameObject.SetActive(false);
        PiecesPage62.gameObject.SetActive(false);
        ChangeTo2.gameObject.SetActive(false);
        Page6Text.gameObject.SetActive(false);
        ChangeTo1.gameObject.SetActive(false);
        PiecesPage63tab.gameObject.SetActive(false);
        PiecesPage63Main.gameObject.SetActive(false);
    }

    public void OnClickedToPage8()
    {
        Page7Arrow.gameObject.SetActive(true);
        Page7Back.gameObject.SetActive(true);
        Page7Pieces.gameObject.SetActive(true);
        Page7Text.gameObject.SetActive(true);

        Page8Text.gameObject.SetActive(false);
        ToPage8.gameObject.SetActive(false);
        ToPage7.gameObject.SetActive(false);
        ToPage6.gameObject.SetActive(false);
        ToPage5.gameObject.SetActive(false);
        ToPage4.gameObject.SetActive(false);
        ToPage3.gameObject.SetActive(false);
        ToPage2.gameObject.SetActive(false);
    }

    public void OnClickedBakToPage1()
    {
        tutorialPanel.gameObject.SetActive(true);
        Page3Images1.gameObject.SetActive(true);
        Page3Images2.gameObject.SetActive(true);
        Page3Panel.gameObject.SetActive(true);
        Page3Panel2.gameObject.SetActive(true);
        Page3Text.gameObject.SetActive(true);
        Page3Text2.gameObject.SetActive(true);
        ToPage2.gameObject.SetActive(true);

        Page7Arrow.gameObject.SetActive(false);
        Page7Back.gameObject.SetActive(false);
        Page7Pieces.gameObject.SetActive(false);
        Page7Text.gameObject.SetActive(false);
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// 
    //public void TO_EASY()
    //{
    //    easyButton.gameObject.SetActive(true);
    //    playEasy.gameObject.SetActive(true);
    //    arrowLeft2.gameObject.SetActive(true);
    //    arrowRight2.gameObject.SetActive(true);
    //    toTutorial.gameObject.SetActive(true);
    //    toHard.gameObject.SetActive(true);

    //    arrowRight1.gameObject.SetActive(false);
    //    tutorialButton.gameObject.SetActive(false);
    //    easyDescription.gameObject.SetActive(false);
    //    playTutorial.gameObject.SetActive(false);
    //    toEasy.gameObject.SetActive(false);
    //}

    public void TO_HARD()
    {
        hardButton.gameObject.SetActive(true);
        playHard.gameObject.SetActive(true);
        arrowLeft1.gameObject.SetActive(true);
        toHard.gameObject.SetActive(true);
        backFromHard.gameObject.SetActive(true);


        arrowRight2.gameObject.SetActive(false);
        arrowRight1.gameObject.SetActive(false);
        easyButton.gameObject.SetActive(false);
        hardDescription.gameObject.SetActive(false);
        playEasy.gameObject.SetActive(false);
        toHard.gameObject.SetActive(false);
    }

    public void BACK_FROM_HARD()
    {
        easyButton.gameObject.SetActive(true);
        playEasy.gameObject.SetActive(true);
        arrowRight2.gameObject.SetActive(true);
        toHard.gameObject.SetActive(true);

        arrowLeft1.gameObject.SetActive(false);
        hardButton.gameObject.SetActive(false);
        easyDescription.gameObject.SetActive(false);
        playHard.gameObject.SetActive(false);
        backFromHard.gameObject.SetActive(false);
    }

    //public void TO_TUTORIAL()
    //{
    //    tutorialButton.gameObject.SetActive(true);
    //    playTutorial.gameObject.SetActive(true);
    //    arrowRight1.gameObject.SetActive(true);
    //    toEasy.gameObject.SetActive(true);

    //    arrowRight2.gameObject.SetActive(false);
    //    arrowLeft2.gameObject.SetActive(false);
    //    easyButton.gameObject.SetActive(false);
    //    tutorialDescription.gameObject.SetActive(false);
    //    playEasy.gameObject.SetActive(false);
    //    toHard.gameObject.SetActive(false);
    //    toTutorial.gameObject.SetActive(false);
    //}


    public void SelectSideWhite()
    {

    }


    ///////////////////////////////////////////////////////////
    // Multiplayer Panel

    public void OnClickedMultiplayer()
    {   
        multiPlayPanel.gameObject.SetActive(true);

        head_animation.gameObject.SetActive(false);
        newTutButton.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(false);
        playerVsAIButton.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(false);
        backButton1.gameObject.SetActive(false);
        multiPanel.gameObject.SetActive(false);
    }

    //public void OnClickedCancelMultiTab()
    //{
    //    tutorialButton.gameObject.SetActive(true);
    //    singleButton.gameObject.SetActive(true);
    //    multiplayerButton.gameObject.SetActive(true);
    //    backButton1.gameObject.SetActive(true);
    //    head_animation.gameObject.SetActive(true);

    //    multiTab.gameObject.SetActive(false);
    //}

    //public void OnClickedPlayMultiplayer()
    //{
        

    //    multiPanel.gameObject.SetActive(false);
    //    multiCancel.gameObject.SetActive(false);
    //    multiStart.gameObject.SetActive(false);
        
    //}

    // load loading scene to lobby
    public void OnClickedYesMulti()
    {
        loadLobby.gameObject.SetActive(true);
        StartCoroutine(ToLobby());
        //SceneManager.LoadScene("LoadingScene");
    }

    public void OnClickedNoMulti()
    {
        newTutButton.gameObject.SetActive(true);
        singleButton.gameObject.SetActive(true);
        playerVsAIButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);
        backButton1.gameObject.SetActive(true);
        head_animation.gameObject.SetActive(true);

        multiPlayPanel.gameObject.SetActive(false);
        multiPanel.gameObject.SetActive(false);
    }


    IEnumerator ToLobby()
    {
        yield return new WaitForSeconds(2);
        //loadLobby.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator ToTutorial()
    {
        yield return new WaitForSeconds(2);
        //loadLobby.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);
        SceneManager.LoadScene("LoadingScreenToTutorial");
    }

    IEnumerator ToLocalBoard()
    {
        yield return new WaitForSeconds(2);
        //loadLobby.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);
        SceneManager.LoadScene("LoadingSceneToVersus");
    }

    IEnumerator ToAiEasy()
    {
        yield return new WaitForSeconds(2);
        //loadLobby.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);
        SceneManager.LoadScene("LoadingScreenToAIEasy");
    }

    IEnumerator ToAiHard()
    {
        yield return new WaitForSeconds(2);
        //loadLobby.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);
        SceneManager.LoadScene("LoadingSceneToAI");
    }


    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public void OnClickStartEasyMode()
    {
        easyPanel.gameObject.SetActive(true);

        easyButton.gameObject.SetActive(false);
        cancelPlayerVsAI.gameObject.SetActive(false);
        fileBanner.gameObject.SetActive(false);
        fileFilling.gameObject.SetActive(false);
        arrowLeft2.gameObject.SetActive(false);
        arrowRight2.gameObject.SetActive(false);
        toHard.gameObject.SetActive(false);
        toTutorial.gameObject.SetActive(false);

    }

    public void OnClickStartHardMode()
    {
        hardPanel.gameObject.SetActive(true);

        easyButton.gameObject.SetActive(false);
        hardButton.gameObject.SetActive(false);
        cancelPlayerVsAI.gameObject.SetActive(false);
        fileBanner.gameObject.SetActive(false);
        fileFilling.gameObject.SetActive(false);
        arrowLeft1.gameObject.SetActive(false);
        backFromHard.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        quitPanel.gameObject.SetActive(true);

        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
    }

    public void OnCLickQuitNo()
    {

        startButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditsButton.gameObject.SetActive(true);

        quitPanel.gameObject.SetActive(false);
    }

    public void OnCLickQuitYes()
    {
        Application.Quit();
    }


    // Glitch animation
    IEnumerator LateCall()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        gameObject.SetActive(false);
        //Do Function here...

    }
}

