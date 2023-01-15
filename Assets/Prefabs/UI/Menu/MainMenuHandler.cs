using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System;
using System.Globalization;
using TMPro;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Handles main menu interaction
/// </summary>
public class MainMenuHandler : MonoBehaviour
{
    public GameObject TutorialScreen;
    public GameObject CreditsScreen;
    public GameObject playerLoader;
    public GameObject codeInput;
    public GameObject crashPopUp;


    /// <summary>
    /// Checks the buttons name and operates accordingly
    /// </summary>
    /// <param name="name"></param>
    private void buttonPressed(string name)
    {
        if (name == "StartButton")
        {
            SceneManager.LoadScene(sceneName: "Scene1");
        }
        else if (name == "ContinueButton")
        {
            if (codeInput != null && codeInput.GetComponent<TMP_InputField>().text != "")
            {
                // Calls to the database with the code input field and proccesess the data
                StartCoroutine(GetRequest("https://ktprog.com/Verlich/index.php?dataid=" + Regex.Replace(codeInput.GetComponent<TMP_InputField>().text, "[^0-9]", "") + "&mode=get"));
                SceneManager.LoadScene(sceneName: "Scene1");
            }
        }
        else if (name == "TutorialButton")
        {
            Instantiate(TutorialScreen, transform.position, Quaternion.identity);
        }
        else if (name == "CreditsButton")
        {
            Instantiate(CreditsScreen, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Processes web requests and on success updates player data
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    IEnumerator GetRequest(string uri)
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
                    GameObject obj = Instantiate(playerLoader, transform.position, Quaternion.identity);
                    obj.GetComponent<playerLoader>().startUpdate(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    // Start is called before the first frame update
    /// <summary>
    /// Checks if there is a crash report and adds onclick listeners to the buttons
    /// </summary>
    void Start()
    {
        if (CrashReport.lastReport != null)
        {
            GameObject cpu = Instantiate(crashPopUp, transform.position, Quaternion.identity);
            cpu.GetComponent<CrashScript>().setError(CrashReport.lastReport.text);
            CrashReport.RemoveAll();
        }
        Transform tf = transform.GetChild(0);
        foreach (Transform child in tf)
        {
            if (child.GetComponent<Button>() != null)
            {
                child.GetComponent<Button>().onClick.AddListener(delegate { buttonPressed(child.name); });
            }
        }

        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
    }

    /// <summary>
    /// Used to overwrite Certificate checks to not be required
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