using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContainerInventory : MonoBehaviour, IDropHandler
{
    public GameObject inventorySlot;
    private Container con;
    private List<GameObject> UIElements = new List<GameObject>();
    public void loadInventory(List<Item> items)
    {
        foreach (UIInventoryItem ui in GetComponentsInChildren<UIInventoryItem>())
        {
            UIElements.Clear();
            Destroy(ui.gameObject);
        }
        foreach (Item item in items)
        {
            bool has = false;
            foreach (GameObject g in UIElements)
            {
                if(g.GetComponent<UIInventoryItem>().getItem().itemName.Equals(item.itemName)) {
                    g.GetComponent<UIInventoryItem>().stack.text = int.Parse(g.GetComponent<UIInventoryItem>().stack.text) + 1 + "";
                    has = true;
                }
            }
            if(!has)
            {
                GameObject i = GameObject.Instantiate(inventorySlot);
                i.GetComponent<UIInventoryItem>().setItem(item);
                i.transform.SetParent(gameObject.transform, false);
                i.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;
                UIElements.Add(i);
            }
        }
    }

    public void setContainer(Container c)
    {
        this.con = c;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject slot = eventData.pointerDrag.gameObject;
        if(slot.GetComponent<UIInventoryItem>() != null)
        {
            FindObjectOfType<Character>().removeFromInventory(slot.GetComponent<UIInventoryItem>().getItem());
            FindObjectOfType<Inventory>().resetInventory();
            con.addToContainer(slot.GetComponent<UIInventoryItem>().getItem());
            if(con != null)
            {
                loadInventory(con.containerItems);
            }
        }
    }
}
