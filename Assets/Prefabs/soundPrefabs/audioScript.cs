using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys the sound object 3 seconds after it initializes
/// </summary>
public class audioScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3f);
    }
}
