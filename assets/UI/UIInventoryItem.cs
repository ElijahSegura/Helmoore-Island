using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour {
    public GameObject icon, multiplier, inventoryConstant, iconBackground, HUD;
    private GameObject invParent;
    private int multi = 1;
    private bool inContainer = false;
    public Item item;
    Resolution reference;
    private bool leftDrag = false;
    private int items;
    public GameObject rcmenu;
    public GameObject parent;

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
        if (data.hasBackground())
        {
            iconBackground.GetComponent<Image>().sprite = data.getBackground();
        }
    }
    

    public void addHover(string value)
    {
        
    }



    public void addMultiplier()
    {
        multi++;
    }

    public bool draggable;
    private bool drag = false;
    public void startDrag()
    {
        
        if(draggable)
        {

            if (!drag)
            {
                if (item.GetType() == typeof(Seed))
                {
                    FindObjectOfType<Inventory>().seed = (Seed)item;
                }
                gameObject.transform.SetParent(HUD.transform, false);
                //replace with drop function in item inventoryConstant.GetComponent<Inventory>().setDrag(gameObject);
            }
            Vector3 point = (Input.mousePosition);
            transform.localPosition = point;
            drag = true;
            GetComponent<RectTransform>().sizeDelta = new Vector2(160, 160);
            if (Input.GetButton("Click"))
            {
                leftDrag = true;
            }
            else if (Input.GetButton("Right Click"))
            {
                items = multi;
            }
            reference = Screen.currentResolution;
        }
    }

    void Update()
    {
        if(drag)
        {
            Vector3 point = Input.mousePosition;
            float xRatio = point.x / (reference.width / 2);
            float yRatio = point.y / (reference.height / 2);
            xRatio -= 1f;
            yRatio -= 1f;
            point.x = (reference.width * xRatio);
            point.y = (reference.height * yRatio);
            point.z = 0;
            transform.localPosition = point;
        }
        if(leftDrag)
        {
            if (Input.GetAxis("Click") == 0 && drag && (!container && !transferToInventory))
            {
                stopDrag();
            }
            else if ((Input.GetAxis("Click") == 0 && drag && container) || (Input.GetAxis("Click") == 0 && drag && transferToInventory))
            {
                drop();
            }
        }
        else
        {
            if (Input.GetAxis("Right Click") == 0 && drag && (!container && !transferToInventory))
            {
                stopDrag();
            }
            else if ((Input.GetAxis("Right Click") == 0 && drag && container) || (Input.GetAxis("Right Click") == 0 && drag && transferToInventory))
            {
                dropAll();
            }
        }
    }


    public void stopDrag()
    {
        drag = false;
        gameObject.transform.SetParent(parent.transform, false);
    }


    private bool container = false;
    public void isDragAndContainer()
    {
        if (drag)
        {
            if(transform.parent.GetComponent<Inventory>().hasContainer)
            {
                container = true;
            }
        }
    }

    public void exit()
    {
        container = false;
    }

    public void drop()
    {
        if (transferToInventory)
        {
            inventoryConstant.GetComponent<Inventory>().take();
        }
        else
        {
            if (!inContainer)
            {
                inventoryConstant.GetComponent<Inventory>().dragDrop();
            }
        }
        drag = false;
        
    }

    public void dropAll()
    {
        drag = false;
        if(transferToInventory)
        {
            inventoryConstant.GetComponent<Inventory>().takeAll();
        }
        else
        {
            if(!inContainer)
            {
                inventoryConstant.GetComponent<Inventory>().dragDropAll();
            }
        }
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

    private bool transferToInventory = false;
    public void setTransferToInventory(bool tti)
    {
        this.transferToInventory = tti;
    }

    public void rightClick()
    {
        if(Input.GetButtonUp("Right Click"))
        {
            Vector3 point = Input.mousePosition;
            float xRatio = point.x / (reference.width / 2);
            float yRatio = point.y / (reference.height / 2);
            xRatio -= 1f;
            yRatio -= 1f;
            point.x = (reference.width * xRatio);
            point.y = (reference.height * yRatio);
            point.z = 0;
            Debug.Log(rcmenu.activeSelf);
            if(!GameObject.Find(rcmenu.name))
            {
                rcmenu = GameObject.Instantiate(rcmenu);
                rcmenu.SetActive(true);
            }
            rcmenu.transform.SetParent(inventoryConstant.transform, false);
            rcmenu.GetComponent<RightClickMenu>().show(item);
            rcmenu.transform.localPosition = point;
        }
    }
}
