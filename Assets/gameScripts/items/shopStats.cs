using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopStats : MonoBehaviour
{
    public int weight;
    public float cost;
    public bool sold;
    public GameObject text;
    public int healing;

    public string effectName;
    public string effect;
    public string effectValue;

    Dictionary<string, Dictionary<string, string>> skills = new Dictionary<string, Dictionary<string, string>>();

    public List<string> skillNames;

    /// <summary>
    /// Updates shop costs
    /// </summary>
    void Start()
    {
        text.GetComponent<TextMesh>().text = cost + " gold";
        
        if(effect != null)
        {
            skillNames.Add(effectName);

            skills.Add(effectName, new Dictionary<string, string>() {
                {"Locked", "false"},
                {"Cost", "0"},
                {"Effect", effect},
                {"effectValue", effectValue},
            });
        }

        
    }

    // Gets certain properties
    public int getWeight()
    {
        return weight;
    }

    public float getCost()
    {
        return cost;
    }

    public int getHealing()
    {
        return healing;
    }

    public Dictionary<string, Dictionary<string, string>> getSkills()
    {
        return skills;
    }

    public List<string> getSkillNames()
    {
        return skillNames;
    }

    /// <summary>
    /// Attempts to buy a item
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public GameObject buy(float money)
    {
        if (money < cost || sold)
        {
            return null;
        }
        else
        {
            sold = true;
            return gameObject;
        }
    }
}
