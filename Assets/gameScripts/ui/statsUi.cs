using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;

public class statsUi : MonoBehaviour
{
    public GameObject goldStat;
    public GameObject levelStat;
    public GameObject xpStat;
    public GameObject depthStat;
    public GameObject saveAndQuitButton;
    public GameObject shoutObj;
    private float curGold;
    private float curLevel;
    private float curDepth;
    private Dictionary<char, string> toLet = new Dictionary<char, string>();

    // Updates stat ui values
    public void updateGold(float gold)
    {
        curGold = gold;
        goldStat.GetComponent<TextMeshProUGUI>().text = "gold : " + gold;
    }

    public void updateLevel(int level)
    {
        curLevel = level;
        levelStat.GetComponent<TextMeshProUGUI>().text = "level : " + level;
    }
    public void updateXp(int maxXp, int currentXp)
    {
        xpStat.GetComponent<Slider>().maxValue = maxXp;
        xpStat.GetComponent<Slider>().value = currentXp;
    }

    public void updateDepth(int depth)
    {
        curDepth = depth;
        depthStat.GetComponent<TextMeshProUGUI>().text = "depth : " + depth;
    }

    /// <summary>
    /// Loads player stats into the database for later use
    /// </summary>
    public void save()
    {
        // level, depth, gold rounded down
        int goldToSave = Mathf.FloorToInt(curGold);
        int levelToSave = Mathf.FloorToInt(curLevel);
        int depthToSave = Mathf.FloorToInt(curDepth);
        string dataString = "" + levelToSave.ToString() + "-" + depthToSave.ToString() + "-" + goldToSave.ToString() + "-";
        string saveString = "";
        foreach (char num in dataString)
        {
            saveString += toLet[num];
        }
        string toGive = "https://ktprog.com/Verlich/index.php?dataid=" + Regex.Replace(dataString, "[^0-9]", "") + "&mode=post&data=" + saveString;
        StartCoroutine(GetRequest(toGive, Regex.Replace(dataString, "[^0-9]", "")));
    }

    /// <summary>
    /// Handles the server request and tells the player their data code
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    IEnumerator GetRequest(string uri, string code)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.certificateHandler = new CertificateBypass();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    GameObject msg = Instantiate(shoutObj, new Vector3(0, 0, 0), Quaternion.identity);
                    msg.GetComponent<ShoutHandler>().shout("Your code is: " + code);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the properties
    /// </summary>
    void Start()
    {
        toLet.Add('0', "O");
        toLet.Add('1', "K");
        toLet.Add('2', "H");
        toLet.Add('3', "I");
        toLet.Add('4', "N");
        toLet.Add('5', "Y");
        toLet.Add('6', "A");
        toLet.Add('7', "R");
        toLet.Add('8', "E");
        toLet.Add('9', "T");
        toLet.Add('-', ".");
        saveAndQuitButton.GetComponent<Button>().onClick.AddListener(delegate { save(); });

        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
    }

    /// <summary>
    /// Overwrites the requirement for certificates
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="x509Certificate"></param>
    /// <param name="x509Chain"></param>
    /// <param name="sslPolicyErrors"></param>
    /// <returns></returns>
    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all Certificates are accepted
        return true;
    }
}
