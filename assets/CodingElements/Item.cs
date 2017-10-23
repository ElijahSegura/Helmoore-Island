using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour {
    public int value;
    public string itemName;
    public DB add;
    public Sprite icon;
    public Sprite backGround;
    public int maxValue;
    public bool stackable = false;
    public bool variableValues = false;
    public bool isContainer;
    private int stack = 0;
    private List<Item> itemStack = new List<Item>();
    private bool equippable = false;
    private Character Character;
    private GameObject inventory;
    private List<Component> componenets;
    public bool pickupable = true;
    public GameObject E;
    
    public void display()
    {
        if(E != null)
        {
            E.SetActive(true);
        }
    }
    
    public void stopDisplay()
    {
        if(E != null)
        {
            E.SetActive(false);
        }
    }
    

    void Start()
    {
        setChar();
        addE();
    }

    public void addE()
    {
        if (E != null)
        {
            E = GameObject.Instantiate(E);
            E.GetComponent<Projector>().setLookAt(gameObject);
            E.transform.position = transform.position;
            E.SetActive(false);
        }
    }

    public List<Component> getComps()
    {
        return componenets;
    }

    public List<Item> getStack()
    {
        return itemStack;
    }

    public void setChar()
    {
        Character = Camera.main.transform.parent.parent.gameObject.GetComponent<Character>();
        addself();
        componenets = new List<Component>(GetComponents<Component>());
    }

    public Character getChar()
    {
        return Character;
    }

    public PlayerCamera getCamera()
    {
        return Character.getCamera();
    }

    public void addself()
    {
        itemStack.Add(GetComponent<Item>());
        stack++;
    }

    public int getcount()
    {
        return stack;
    }

    public void PickUp()
    {
        int max = getChar().getMaxInvSize();
        int current = getChar().getCurrentInvSize();
        if (current < max)
        {
            if(itemStack.Count == 1)
            {
                Character.pickup(gameObject.GetComponent<Item>());
                Destroy(gameObject);
                
            }
            else
            {
                Debug.Log("Stack");
                foreach (Item item in itemStack)
                {
                    Character.addToInventory(item);
                }
                Destroy(gameObject);
            }
        }
    }
    
    
    public void openContainer(List<Item> Items, GameObject c)
    {
        Character.gameObject.GetComponentInChildren<PlayerCamera>().openContainer(Items, c);
    }

    public Sprite getIcon()
    {
        return icon;
    }

    public bool hasBackground()
    {
        if(backGround != null)
        {
            return true;
        }
        return false;
    }

    public Sprite getBackground()
    {
        return backGround;
    }

    public bool getStackable()
    {
        return stackable;
    }

    public bool getVV()
    {
        return variableValues;
    }

    public void setContainer(GameObject c)
    {
        Character.GetComponentInChildren<PlayerCamera>().gameObject.GetComponentInChildren<Inventory>().setContainer(c);
    }

    public void addToStack(Item i)
    {
        itemStack.Add(i);
        stack++;
    }

    public void removeFromStack(Item i)
    {
        itemStack.Remove(i);
        stack--;
    }

    public bool getEquippable()
    {
        return equippable;
    }
    
    public abstract GameObject getObject();
    public abstract void set(Item i);
    public abstract void Interact();
}