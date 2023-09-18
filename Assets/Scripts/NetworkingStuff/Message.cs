using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Message : MonoBehaviour
{
    public TMP_Text myMessage, receivedUsername;
    public TMP_InputField UserName;
    static string userName;

   
    //public string receivedUsername;


    void Start()
    {
        // On this object's creation (message) auto scroll to the bottom

        // this works
        // GameObject.FindGameObjectWithTag("scrollchat").GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;

        // but this one seems to not remove the cursor after sending message
        GameObject.FindGameObjectWithTag("scrollchat").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;

        // UserName is not assigned to anything (according to inspector), meaning that any code after this line may give trouble
        // cause this line is attempting to change something that does not exist, and it ends up giving an error
        // Maybe that's the reason the cursor disappear after sending a message?
        UserName.characterLimit = 15;
    }
    public void SetUserName()
    {
        userName = UserName.text.ToUpper();
        if(userName == ""){
            int num = Random.Range(10000, 99999);
            userName = "Hacker" + num.ToString();
        }
        Debug.Log(userName);
    }

    public void SetUserName(string newUserName)
    {
        userName = newUserName;
        Debug.Log(userName);
    }
    
    public string GetUserName(){
        return userName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
