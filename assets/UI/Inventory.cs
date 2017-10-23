using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    public GameObject look, space;
    private List<GameObject> UIelements = new List<GameObject>();
    private List<GameObject> containerElements = new List<GameObject>();
    public GameObject container, containerUI;
    public bool hasContainer = false;
    public GameObject playerEquipment;
    private Type Filter;


	void OnEnable()
    {
        loadInventory();
        space.GetComponent<Text>().text = Camera.main.transform.parent.parent.GetComponent<Character>().getInventory().Count + " / " + Camera.main.transform.parent.parent.GetComponent<Character>().getMaxInvSize();
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
        foreach (Item item in Camera.main.transform.parent.parent.GetComponent<Character>().getInventory())
        {
            Debug.Log(item);
            if(Filter != null)
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
            else
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

        if(hasContainer)
        {
            foreach (Item item in container.GetComponent<Container>().getItems())
            {
                if (item.stackable && !item.variableValues && containerElements.Count > 0)
                {
                    bool added = false;
                    foreach (GameObject d in containerElements)
                    {
                        if (d.GetComponent<UIInventoryItem>().item.itemName.Equals(item.itemName))
                        {
                            d.GetComponent<UIInventoryItem>().addMultiplier();
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        addUIContainer(item);
                    }
                }
                else if (item.stackable && item.variableValues && containerElements.Count > 0)
                {
                    bool added = false;
                    foreach (GameObject d in containerElements)
                    {
                        if (d.GetComponent<UIInventoryItem>().item.itemName.Equals(item.itemName))
                        {
                            d.GetComponent<UIInventoryItem>().addMultiplier();
                            added = true;
                        }
                    }

                    if (!added)
                    {
                        addUIContainer(item);
                    }
                }
                else
                {
                    addUIContainer(item);
                }
                
            }
        }
        foreach (GameObject item in UIelements)
        {
            item.GetComponent<UIInventoryItem>().draggable = true;
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

    public void addUIContainer(Item item)
    {
        GameObject tempUIelement = GameObject.Instantiate(look);
        tempUIelement.transform.SetParent(containerUI.transform, false);
        tempUIelement.GetComponent<UIInventoryItem>().SetData(item);
        tempUIelement.GetComponent<UIInventoryItem>().setItem(item);
        tempUIelement.GetComponent<UIInventoryItem>().setInContainer(true);
        containerElements.Add(tempUIelement);
    }

    public void setContainer(GameObject c)
    {
        this.container = c;
        hasContainer = true;
    }


    public void transferToContainer(Item i)
    {
        container.GetComponent<Container>().addToContainer(i);
        Camera.main.transform.parent.parent.GetComponent<Character>().getInventory().Remove(i);
    }

    public GameObject d;
    public void setDrag(GameObject drag)
    {
        d = drag;
    }

    public Seed seed;
    public void isDC()
    {
        if(d != null)
        {
            d.GetComponent<UIInventoryItem>().isDragAndContainer();
        }
    }

    public void isDraggingToInventory() {
        if(d != null)
        {
            if (d.GetComponent<UIInventoryItem>().isInContainer())
            {
                d.GetComponent<UIInventoryItem>().setTransferToInventory(true);
            }
        }
    }

    public void exitDTI()
    {
        d.GetComponent<UIInventoryItem>().setTransferToInventory(false);
    }

    public void cExit()
    {
        if (d != null)
        {
            d.GetComponent<UIInventoryItem>().exit();
        }
    }

    public void dragDrop()
    {
        transferToContainer(d.GetComponent<UIInventoryItem>().getItem());
        d.transform.SetParent(containerUI.transform, false);
        resetInventory();
    }

    public void dragDropAll()
    {
        string testName = d.GetComponent<UIInventoryItem>().getItem().itemName;
        List<Item> temp = new List<Item>(Camera.main.transform.parent.parent.GetComponent<Character>().getInventory());
        foreach (Item item in temp)
        {
            if(item.itemName.Equals(testName))
            {
                transferToContainer(item);
            }
        }
        d.transform.SetParent(containerUI.transform, false);
        resetInventory();
    }

    public void take()
    {
        container.GetComponent<Container>().removeFromContainer(d.GetComponent<UIInventoryItem>().getItem());
        Camera.main.transform.parent.parent.GetComponent<Character>().getInventory().Add(d.GetComponent<UIInventoryItem>().getItem());
        resetInventory();
    }

    public void takeAll()
    {
        string testName = d.GetComponent<UIInventoryItem>().getItem().itemName;
        List<Item> temp = new List<Item>(container.GetComponent<Container>().getItems());
        foreach (Item item in temp)
        {
            if (item.itemName.Equals(testName))
            {
                container.GetComponent<Container>().removeFromContainer(item);
                Camera.main.transform.parent.parent.GetComponent<Character>().getInventory().Add(item);
            }
        }
        resetInventory();
    }



}
