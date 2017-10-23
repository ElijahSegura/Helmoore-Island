using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenu : MonoBehaviour {
    PlayerCamera c;
    private Item ti;
	public void show(Item ii)
    {
        ti = ii;
        c = Camera.main.transform.parent.parent.GetComponent<Character>().getCamera();
        if (!ii.getEquippable())
        {
            transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
        else
        {
            transform.GetChild(0).GetComponent<Button>().interactable = true;
        }
        if(c.hasContainer())
        {
            transform.GetChild(3).GetComponent<Button>().interactable = true;
        }
        else
        {
            transform.GetChild(3).GetComponent<Button>().interactable = false;
        }
    }

    public void drop()
    {
        Camera.main.transform.parent.parent.GetComponent<Character>().drop(ti);
        gameObject.SetActive(false);
    }

    public void dropAll()
    {
        Camera.main.transform.parent.parent.GetComponent<Character>().dropAll(ti);
        gameObject.SetActive(false);
    }
}
