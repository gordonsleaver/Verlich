using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillList : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, string>> skills = new Dictionary<string, Dictionary<string,string>>();
    // Start is called before the first frame update, adds in all the skills
    void Awake()
    {
        skills.Add("Damage Buff", new Dictionary<string, string>() {
            {"Locked", "false"},
            {"Cost", "1"},
            {"Effect", "buffDamage"},
            {"effectValue", "2"},
        });
        skills.Add("Damage Buff 2", new Dictionary<string, string>() {
            {"Locked", "Damage Buff"},
            {"Cost", "1"},
            {"Effect", "buffDamage"},
            {"effectValue", "2"},
        });
        skills.Add("speed Buff", new Dictionary<string, string>() {
            {"Locked", "Damage Buff"},
            {"Cost", "1"},
            {"Effect", "buffSpeed"},
            {"effectValue", "2"},
        });
        skills.Add("health Buff", new Dictionary<string, string>() {
            {"Locked", "speed Buff"},
            {"Cost", "1"},
            {"Effect", "buffHealth"},
            {"effectValue", "20"},
        });
        skills.Add("attack speed Buff", new Dictionary<string, string>() {
            {"Locked", "Damage Buff 2"},
            {"Cost", "3"},
            {"Effect", "buffDPS"},
            {"effectValue", "2"},
        });
        skills.Add("dash speed Buff", new Dictionary<string, string>() {
            {"Locked", "speed Buff"},
            {"Cost", "1"},
            {"Effect", "buffDashSpeed"},
            {"effectValue", ".25"},
        });
        skills.Add("dash speed Buff 2", new Dictionary<string, string>() {
            {"Locked", "dash speed Buff"},
            {"Cost", "1"},
            {"Effect", "buffDashSpeed"},
            {"effectValue", ".25"},
        });
        skills.Add("dash length Buff", new Dictionary<string, string>() {
            {"Locked", "health Buff"},
            {"Cost", "1"},
            {"Effect", "buffDashLength"},
            {"effectValue", "200"},
        });
    }

    // Gets/Sets the skills
    public Dictionary<string, Dictionary<string, string>> getSkills()
    {
        return skills;
    }

    public void addSkill(string name, Dictionary<string,string> details)
    {
        skills.Add(name, details);
    }
}
