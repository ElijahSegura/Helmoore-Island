using System;

public class Chef : Class
{
    Cooking skill = new Cooking();
    public float expBoost = 0F;
    public string benifit()
    {
        return "G";
    }

    public float boost()
    {
        return expBoost;
    }

    public Skill boosts()
    {
        return skill;
    }

    public bool combatRelated()
    {
        return false;
    }

    public string description()
    {
        return "";
    }

    public string name()
    {
        return "Chef";
    }

    public void select()
    {
        this.expBoost = 0.2F;
    }

    public void deselect()
    {
        this.expBoost = 0.05F;
    }
}
