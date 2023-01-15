using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShoutHandler : MonoBehaviour
{
    /// <summary>
    /// When called it will set its text to a certain message and then destroy itself 10 seconds later
    /// </summary>
    /// <param name="msg"></param>
    public void shout(string msg)
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        Destroy(gameObject, 10);
    }
}
