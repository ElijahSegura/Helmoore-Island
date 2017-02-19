public class Skill {
    string name;
    int level;
    int exp = 0;
    int toNextLevel;

    int getLevel()
    {
        return level;
    }

    public void levelUp()
    {
        this.level += 1;
        this.exp = 0;
        this.toNextLevel += (int)(toNextLevel * 1.2) + (int)(toNextLevel * 0.2 * -1) + (int)(toNextLevel * 0.4);
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public int getExp()
    {
        return exp;
    }

    public void loadSkill(Skill s)
    {
        this.name = s.name;
        this.level = s.level;
        this.exp = s.exp;
        this.toNextLevel = s.toNextLevel;
    }
}
