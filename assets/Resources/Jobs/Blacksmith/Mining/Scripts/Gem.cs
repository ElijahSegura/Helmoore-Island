using UnityEngine;

public class Gem : Item {
    private float purity = 1.0f;
    public override void Interact()
    {
        PickUp();
    }

    // Use this for initialization
    void Start () {
        setChar();
        stackable = true;
        name = itemName;
        gen();
    }

    public void gen()
    {
        value = (int)Mathf.Floor(maxValue * purity);
    }
    public override GameObject getObject()
    {
        return null;
    }
    public override void set(Item i)
    {

    }
}
