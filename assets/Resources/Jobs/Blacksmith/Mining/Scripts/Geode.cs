using UnityEngine;

public class Geode : Item {
    [Range(0f, .33f)]
    public float chance;
    public Gem gem;
    public override GameObject getObject()
    {
        //throw new NotImplementedException();
        return null;
    }

    public override void Interact()
    {
        PickUp();
    }

    public override void set(Item i)
    {
        //throw new NotImplementedException();
    }

    // Use this for initialization
    void Start() {
        setChar();
    }

    public Gem split()
    {
        if(Random.Range(0f,1f) < chance)
        {
            return gem;
        }
        return null;
    }
	
}
