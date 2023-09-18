using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintDisplay : MonoBehaviour
{
    public int randNum;
    public GameObject hintDisplay;
    public bool genHint = false;

    // Start is called before the first frame update
    void Update()
    {
        if (genHint == false)
        {
            hintDisplay.GetComponent<Animator>().Play("New State");
            genHint = true;
            StartCoroutine(HintTracker());
        }
    }

    IEnumerator HintTracker()
    {
        randNum = Random.Range(1, 20);
        if (randNum == 1)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Can't see the entire board? Right click on a piece on the board to move it.";
        }
        if (randNum == 2)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Press the key SPACE to re-center the circuit on the screen.";
        }
        if (randNum == 3)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Want to go back to the Main Menu? Press the key ESC to open the Pause Game window.";
        }
        if (randNum == 4)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Miss-clicked a move and want a second chance? Press the key U to undo the most recent move.";
        }
        if (randNum == 5)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "The Firewall can be easily underestimated, but moving it on top of your opponents Hacker can be a devastating move as you can then place directly around it.";
        }
        if (randNum == 6)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Keeping Viruses in reserve can be beneficial as they can be used to get into spots that are surrounded.";
        }
        if (randNum == 7)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Malware can be effective as short runners when the Circuit is still small, so they may be beneficial to place early in the game.";
        }
        if (randNum == 8)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "The C# Script is the most mobile piece in your arsenal, don't let it get pinned!";
        }
        if (randNum == 9)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Opponent's pieces closing in on your Hacker? Don't forget that you can move your Hacker, often crippling your opponent's plan.";
        }
        if (randNum == 10)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Try to keep your pieces as mobile as possible; the more pieces you have mobile, the more strength you have.";
        }
        if (randNum == 11)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "If a player has no availables moves or placements, their opponent keeps playing until the player either has an available move or the game is over. Try to pin your opponent!";
        }
        if (randNum == 12)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "If both players repeat the same move 3 times in a row, the game will count as a draw.";
        }
         if (randNum == 13)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "To zoom in and out of the Circuit, use the Scroll Wheel on your mouse!";
        }
        if(randNum == 14)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Playing against the computer and want to switch sides? Reset the game and choose a different color.";
        }
        if(randNum == 15)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "Want to play with a friend? Create a lobby and play together using the same lobby code!";
        }
        if(randNum == 16)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "If a Firewall is on top of another piece, that space assumes the color of which piece is on top for placing purposes.";
        }
        if(randNum == 17)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "The Firewall and the Virus are both useful for getting in/out of surrounded spaces. Try keeping a few around your Hacker in case you're in need of a quick escape.";
        }
        if(randNum == 18)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "You can't move any of your placed pieces until your Hacker is placed, so be sure to place your Hacker quickly to gain mobility!";
        }
        if(randNum == 19)
        {
            hintDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = "A 'pin' is when a piece is unable to move because it is restricted by the 'Freedom of Movement' rule or the 'One Circuit' rule. Pin your Opponent's valuable pieces before they pin yours!";
        }


        hintDisplay.GetComponent<Animator>().Play("Text Anim Load Screen Hints");
        yield return new WaitForSeconds(8);

        genHint = false;
    }
}
