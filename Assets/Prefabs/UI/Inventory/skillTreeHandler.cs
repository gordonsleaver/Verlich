using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

// Makes sure it can get scripts potentially out of context
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(skillList))]

public class skillTreeHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private skillList skillList;
    private Player plr;
    public bool db;
    private Canvas canvas;

    public GameObject spc;

    /// <summary>
    /// Sets property values
    /// </summary>
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        skillList = GetComponent<skillList>();
        plr = GameObject.Find("Player").GetComponent<Player>();
    }

    // Start is called before the first frame update
    /// <summary>
    /// Adds on click listeners to the buttons
    /// </summary>
    void Start()
    {
        canvas = GetComponentsInChildren<Canvas>()[0];
        canvas.enabled = false;
        foreach (Transform child in canvas.transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                Button btn = child.GetComponent<Button>();
                btn.onClick.AddListener(delegate { clickOnSkill(btn, child.name); });
            }
        }
    }

    // Update is called once per frame
    /// <summary>
    /// Checks if the player has pressed E and shows the ui accordingly
    /// </summary>
    void Update()
    {
        if (playerInput.sTreeInput)
        {
            if (!db)
            {
                db = true;
                canvas.enabled = true;
            }
            spc.GetComponent<TMPro.TextMeshProUGUI>().text = plr.getSkillPoints().ToString() + " skill points";
        }
        else if (db)
        {
            db = false;
            canvas.enabled = false;
        }
    }

    /// <summary>
    /// Updates skill points and skill tree ui, and adds the skill to the player
    /// </summary>
    /// <param name="sn">Skill Name</param>
    /// <param name="si">Skill Info</param>
    /// <param name="c">Cost</param>
    /// <param name="obj"></param>
    private void addSkill(string sn, Dictionary<string, string> si, int c, Button obj)
    {
        int plrSkillPoints = plr.getSkillPoints();
        plr.setSkillPoints(plrSkillPoints -= c);
        plr.addPlayerSkill(sn, si, false);
        ColorBlock cb = obj.colors;
        cb.colorMultiplier = .5F;
        obj.colors = cb;
    }

    /// <summary>
    /// Checks if the player can buy the skill requested and updates the dictionaries accordingly
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="skillName"></param>
    public void clickOnSkill(Button btn, string skillName)
    {
        Dictionary<string, Dictionary<string, string>> skills = skillList.getSkills();
        Dictionary<string,string> skillInfo = skills[skillName];
        if (skillInfo != null)
        {
            string locked = skillInfo["Locked"];
            string cost = skillInfo["Cost"];
            string effect = skillInfo["Effect"];
            string effectValue = skillInfo["effectValue"];

            int plrSkillPoints = plr.getSkillPoints();
            Dictionary<string, Dictionary<string, string>> list = plr.getPlayerSkills();
            if (plrSkillPoints >= int.Parse(cost) && !list.ContainsKey(skillName))
            {
                if (locked == "false")
                {
                    addSkill(skillName, skillInfo, int.Parse(cost), btn);
                }
                else
                {
                    
                    if (list.ContainsKey(locked))
                    {
                        addSkill(skillName, skillInfo, int.Parse(cost), btn);
                    }
                }
            }

            
        }
    }
}
