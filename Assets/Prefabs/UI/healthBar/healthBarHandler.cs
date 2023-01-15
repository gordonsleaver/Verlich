using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBarHandler : MonoBehaviour
{
    private CapsuleCollider2D cc2d;
    private RectTransform bar;
    private Player plr;
    private basicBehave enm;

    private Type type = typeof(Player);
    // Start is called before the first frame update
    /// <summary>
    /// Sets property values
    /// </summary>
    void Start()
    {
        cc2d = transform.parent.gameObject.GetComponentInChildren<CapsuleCollider2D>();
        bar = transform.GetChild(0).GetComponent<RectTransform>();
        plr = transform.parent.gameObject.GetComponentInChildren<Player>();
        enm = transform.parent.gameObject.GetComponentInChildren<basicBehave>();
    }

    // Update is called once per frame
    /// <summary>
    /// Updates all enemies and player health every frame
    /// </summary>
    void Update()
    {
        if (plr != null)
        {
            GetComponent<Transform>().rotation = Quaternion.identity;
            GetComponent<Transform>().position = cc2d.bounds.center;
            if (plr.health > 0)
            {
                bar.sizeDelta = new Vector2(plr.health * 2, 20);
            }
            else if (plr.health > 150)
            {
                bar.sizeDelta = new Vector2(300, 20);
            }
        }
        else if (enm != null) 
        {
            GetComponent<Transform>().rotation = Quaternion.identity;
            GetComponent<Transform>().position = cc2d.bounds.center;
            if (enm.health > 0)
            {
                bar.sizeDelta = new Vector2(enm.health * 2, 20);
            }
            else if (enm.health > 150)
            {
                bar.sizeDelta = new Vector2(300, 20);
            }
        }
    }
}
