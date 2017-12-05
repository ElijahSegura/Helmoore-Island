using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem {
    private GameObject Item;
    private Item itemData;

    public InventoryItem(GameObject item, Item data)
    {
        Item = item;
        itemData = data;
    }

    public Item getData()
    {
        return itemData;
    }

    internal GameObject gameObject()
    {
        return Item;
    }
}