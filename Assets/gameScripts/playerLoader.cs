using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class playerLoader : MonoBehaviour
{
    private Dictionary<char, string> toNum = new Dictionary<char, string>();


    // Start is called before the first frame update, updates properties
    void Start()
    {
        toNum.Add('O', "0");
        toNum.Add('K', "1");
        toNum.Add('H', "2");
        toNum.Add('I', "3");
        toNum.Add('N', "4");
        toNum.Add('Y', "5");
        toNum.Add('A', "6");
        toNum.Add('R', "7");
        toNum.Add('E', "8");
        toNum.Add('T', "9");
    }

    /// <summary>
    /// Checks if the id was invalid
    /// </summary>
    /// <param name="inp"></param>
    public void startUpdate(string inp)
    {
        if (inp != "nil" && inp != "Invalid ID")
        {
            StartCoroutine(UpdatePlayer(inp));
        }
    }

    /// <summary>
    /// Updates the player stats based on what the datastore returned
    /// </summary>
    /// <param name="inpStr"></param>
    /// <returns></returns>
    IEnumerator UpdatePlayer(string inpStr)
    {
        DontDestroyOnLoad(transform.gameObject);
        while (GameObject.Find("Player") == null)
        {
            yield return null;
        }
        GameObject plr = GameObject.Find("Player");
        Player player = plr.GetComponent<Player>();

        string levelStr = "";
        string depthStr = "";
        string goldStr = "";
        string updateStr = "";
        int count = 0;
        foreach (char let in inpStr)
        {
            if (let == '.')
            {
                count++;
                if (count == 1)
                {
                    levelStr = updateStr;
                }
                else if (count == 2)
                {
                    depthStr = updateStr;
                }
                else if (count == 3)
                {
                    goldStr = updateStr;
                }
                updateStr = "";
            }
            else
            {
                updateStr += toNum[let];
            }
        }
        player.levelUp(Convert.ToInt32(levelStr));
        GameObject.Find("mapControl").GetComponent<mapControl>().setDepth(Convert.ToInt32(depthStr));
        player.addMoney((float)Convert.ToInt32(goldStr));
    }
}
