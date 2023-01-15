using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CrashScript : MonoBehaviour
{
    /// <summary>
    /// Adds a on click listener to the buttons to the checkBtn function
    /// </summary>
    private void Awake()
    {
        foreach (Button btn in GetComponentsInChildren<Button>())
        {
            btn.onClick.AddListener(delegate { checkBtn(btn.gameObject.name); });
        }
    }

    /// <summary>
    /// Sets the errors text to a new text
    /// </summary>
    /// <param name="newmsg"></param>
    public void setError(string newmsg)
    {
        GameObject.Find("msg").GetComponent<TextMeshProUGUI>().text = "A crash has happened with the error " + newmsg + " Send a email to report it?";
    }
    
    /// <summary>
    /// Checks the button names and passes them to different functions accordingly
    /// </summary>
    /// <param name="name"></param>
    private void checkBtn(string name)
    {
        if (name == "YesButton")
        {
            email();
        }
        else if (name == "NoButton")
        {
            exit();
        }
    }

    /// <summary>
    /// Quits out of the UI
    /// </summary>
    private void exit()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Sends a email for error handling
    /// </summary>
    private void email()
    {
        Application.OpenURL("mailto:gordonsleaver@gmail.com");
        exit();
    }
}
