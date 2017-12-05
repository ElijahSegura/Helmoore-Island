using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hover : MonoBehaviour{
    public GameObject List;
    private GameObject tempParent;
    public GameObject text;
    public GameObject ListContent;
    private bool listOpen = false;

    void Start()
    {
        tempParent = transform.parent.parent.parent.parent.parent.gameObject;
    }

    public void addToList(string value)
    {
        text = GameObject.Instantiate(text);
        text.GetComponent<Text>().text = value;
        text.transform.SetParent(ListContent.transform, false);
    }

    public void removeFromList()
    {

    }

    public void changeList()
    {
        if (!listOpen)
        {
            anotherHover();
            List.transform.SetParent(tempParent.transform, false);
            listOpen = true;
        }
        else
        {
            List.transform.SetParent(transform, false);
            listOpen = false;
        }
    }

    public bool isOpen()
    {
        return listOpen;
    }

    public void anotherHover()
    {
        foreach (Hover i in FindObjectsOfType<Hover>())
        {
            if (i.isOpen())
            {
                i.changeList();
            }
        }
    }
    
}
