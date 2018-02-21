using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject icon, multiplier, HUD;
    private GameObject invParent;
    private int multi = 1;
    private bool inContainer = false;
    public Item item;
    Resolution reference;
    private bool leftDrag = false;
    private int items;
    public GameObject rcmenu;
    public GameObject parent;
    public bool draggable;
    public Text stack;

    void Start()
    {
        HUD = GameObject.FindGameObjectWithTag("HUD");
        parent = transform.parent.gameObject;
    }

    public void SetData(Item data)
    {
        reference = Screen.currentResolution;
        if (data.getIcon() != null)
        {
            icon.GetComponent<Image>().sprite = data.getIcon();
        }
    }
    

    public void addHover(string value)
    {
        
    }



    public void addMultiplier()
    {
        multi++;
    }
    
    
    
    public void dropAll()
    {
    }

    public void setItem(Item i)
    {
        this.item = i;
    }
    
    public Item getItem() {
        return item;
    }
    
    public void setInContainer(bool ic)
    {
        this.inContainer = ic;
    }

    public bool isInContainer()
    {
        return inContainer;
    }

    Vector3 startPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(draggable)
        {
            startPos = transform.position;
            transform.SetParent(HUD.transform, false);
            transform.GetChild(2).GetComponent<Image>().raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggable)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(draggable)
        {
            transform.position = startPos;
            transform.SetParent(parent.transform, false);
            transform.GetChild(2).GetComponent<Image>().raycastTarget = true;
        }
    }
}
