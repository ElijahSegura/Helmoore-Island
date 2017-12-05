using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeodeSplitter : Item {
    public override GameObject getObject()
    {
        return null;
    }

    public override void Interact()
    {
        //throw new NotImplementedException();
        getChar().getCamera().openCrushingMenu(GetComponent<GeodeSplitter>());
    }

    public override void set(Item i)
    {
        //throw new NotImplementedException();
    }

    // Use this for initialization
    void Start () {
        setChar();
	}

    private Geode geode;
    public void setGeode(Geode g)
    {
        this.geode = g;
    }
    

}
