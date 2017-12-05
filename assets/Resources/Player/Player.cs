using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    private Dictionary<string, Skill> skills = new Dictionary<string, Skill>() { {"Cooking", new Cooking()} };

    public Skill getSkill(string skillName)
    {
        return skills[skillName];
    }

    public void addExp(string skill)
    {
        skills[skill].levelUp();
    }
}
