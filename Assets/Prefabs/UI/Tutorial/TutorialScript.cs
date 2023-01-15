using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quits out of the UI on button press
/// </summary>
public class TutorialScript : MonoBehaviour
{
    private void buttonPressed()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    /// <summary>
    /// Finds the button component and adds a onclick listener to buttonPressed()
    /// </summary>
    void Start()
    {
        GetComponentsInChildren<Button>(true)[0].onClick.AddListener(delegate { buttonPressed(); });
    }
}
