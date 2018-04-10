using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mob : MonoBehaviour {
    public int level;
    public float maxHealth = 200f;
    public float health = 200f;
    public Image HealthBar;
    public GameObject lootBag;
    public float givenExp = 20;
    public List<Item> lootTable;
    public int maxLoot;
    public GameObject crystal;
    
    void Start()
    {
        int cL = FindObjectOfType<Character>().level;
        if(level - cL > 5)
        {
            HealthBar.color = Color.red;
        }
        else if(level - cL > -3 && level - cL < 3)
        {
            HealthBar.color = Color.green;
        }
        else if(level - cL < -5)
        {
            HealthBar.color = Color.blue;
        }
    }

    public void damage(float damage)
    {
        if(health > 0 && damage > 0)
        {
            health -= damage;
            HealthBar.fillAmount = health / maxHealth;
        }
        if(health <= 0)
        {
            death();
        }
    }

    public void death()
    {
        FindObjectOfType<Character>().getXP(this.givenExp);
        GameObject temp = GameObject.Instantiate(lootBag);
        temp.transform.position = transform.position;
        GameObject.Instantiate(crystal).transform.position = transform.position;
        for (int i = 0; i < Random.Range(0, maxLoot); i++)
        {
            temp.GetComponent<Container>().addToContainer(lootTable[Random.Range(0, lootTable.Count)]);
        }
        Destroy(gameObject);
    }
}
