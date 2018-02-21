using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    public GameObject look, space;
    private List<GameObject> UIelements = new List<GameObject>();
    private List<GameObject> containerElements = new List<GameObject>();
    public bool hasContainer = false;
    public GameObject playerEquipment;
    private Type Filter;


	void OnEnable()
    {
        loadInventory();
        space.GetComponent<Text>().text = "Space: " + Camera.main.transform.parent.parent.GetComponent<Character>().getInventory().Count + " / " + Camera.main.transform.parent.parent.GetComponent<Character>().getMaxInvSize();
    }

    public void setFilter(Type type)
    {
        this.Filter = type;
    }

    void OnDisable()
    {
        Filter = null;
        foreach (GameObject item in UIelements)
        {
            Destroy(item);
        }

        foreach (GameObject item in containerElements)
        {
            Destroy(item);
        }
        d = null;
        containerElements.Clear();
        UIelements.Clear();
    }

    public void resetInventory()
    {
        foreach (GameObject item in UIelements)
        {
            Destroy(item);
        }

        foreach (GameObject item in containerElements)
        {
            Destroy(item);
        }
        seed = null;
        containerElements.Clear();
        UIelements.Clear();
        loadInventory();
        space.GetComponent<Text>().text = Camera.main.transform.parent.parent.GetComponent<Character>().getInventory().Count + " / " + Camera.main.transform.parent.parent.GetComponent<Character>().getMaxInvSize();
    }

    private void loadInventory()
    {
        if(Filter != null)
        {
            foreach (Item item in FindObjectOfType<Character>().getInventory())
            {
                if (Filter != null)
                {
                    if (item.GetType() == Filter)
                    {
                        if (item.stackable && !item.variableValues && UIelements.Count > 0)
                        {
                            bool added = false;
                            foreach (GameObject d in UIelements)
                            {
                                if (d.GetComponent<UIInventoryItem>().item.itemName.Equals(item.itemName))
                                {
                                    d.GetComponent<UIInventoryItem>().addMultiplier();
                                    added = true;
                                }
                            }
                            if (!added)
                            {
                                addUI(item);
                            }
                        }
                        else if (item.stackable && item.variableValues && UIelements.Count > 0)
                        {
                            bool added = false;
                            foreach (GameObject d in UIelements)
                            {
                                if (d.GetComponent<UIInventoryItem>().item.itemName.Equals(item.itemName))
                                {
                                    d.GetComponent<UIInventoryItem>().addMultiplier();
                                    added = true;
                                    break;
                                }
                            }

                            if (!added)
                            {
                                addUI(item);
                            }
                        }
                        else
                        {
                            addUI(item);
                        }
                    }
                }
            }
        }
        else
        {
            foreach (Item item in FindObjectOfType<Character>().getInventory())
            {
                bool has = false;
                foreach (GameObject ui in UIelements)
                {
                    if(ui.GetComponent<UIInventoryItem>().getItem().itemName.Equals(item.itemName))
                    {
                        has = true;
                    }
                }
                if(!has)
                {
                    addUI(item);
                }
            }
        }
        
    }

    public void addUI(Item item)
    {
        GameObject tempUIelement = GameObject.Instantiate(look);
        tempUIelement.transform.SetParent(transform, false);
        tempUIelement.GetComponent<UIInventoryItem>().SetData(item);
        tempUIelement.GetComponent<UIInventoryItem>().setItem(item);
        UIelements.Add(tempUIelement);
    }
    
    public GameObject d;
    public void setDrag(GameObject drag)
    {
        d = drag;
    }

    public Seed seed;
    
    


}
