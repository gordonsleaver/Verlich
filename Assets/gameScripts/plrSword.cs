using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plrSword : MonoBehaviour
{
    public GameObject plr;
    public bool atking;
    public int dmg;
    public GameObject hitEffect;

    private List<GameObject> hit = new List<GameObject>();

    /// <summary>
    /// Sets attacking and clears the hit list if not
    /// </summary>
    /// <param name="a"></param>
    public void setAtking(bool a)
    {
        atking = a;
        if (!atking)
        {
            hit.Clear();
        }
    }

    /// <summary>
    /// Sets the damage
    /// </summary>
    /// <param name="d"></param>
    public void setDmg(int d)
    {
        dmg = d;
    }

    /// <summary>
    /// When sword collides with object handles result
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (atking && !hit.Contains(col.gameObject))
        {
            if (col.gameObject.tag == "enemy")
            {
                col.gameObject.GetComponent<basicBehave>().changeHealth(dmg * -1);
                Instantiate(hitEffect, col.transform.position, Quaternion.identity);
                hit.Add(col.gameObject);
            }
            if (col.gameObject.tag == "barrel")
            {
                col.gameObject.GetComponent<barrel>().destroyBarrel();
                Instantiate(hitEffect, col.transform.position, Quaternion.identity);
                hit.Add(col.gameObject);
            }
        }
    }
}
