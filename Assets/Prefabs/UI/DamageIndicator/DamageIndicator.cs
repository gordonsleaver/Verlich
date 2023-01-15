using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    /// <summary>
    /// Updates the indicator position and destroys it after 1 second
    /// </summary>
    void Start()
    {
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + Random.Range(0.0f, 1.0f), gameObject.transform.localPosition.y + Random.Range(0.0f, 1.0f), 0);
        Destroy(gameObject, 1f);
    }
}
