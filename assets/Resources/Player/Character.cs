﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Character : MonoBehaviour {
    Vector3 moveDirection = Vector3.zero;
    private Mob target;
    CharacterController control;
    public float speed, JumpStrength, sensitivity;
    public Vector3 Gravity;
    private bool female = false;
    private bool RC = false;
    private int dashes = 5;
    private int health, MaxHealth, healthRegen;
    private int mana, MaxMana, manaRegen;
    private float experience = 0.00f, experienceNeeded = 100.00f;
    private Class Class;
    private Vector3 rot = new Vector3(0, 0, 0);
    private double attackSpeed, regenRate;
    private int level = 1;

    public void stopFreeRun()
    {
        freeRunning = false;
    }

    private int dashI = 0;
    private bool ableToControl = true;
    private bool dash = true;
    private List<Item> Inventory = new List<Item>();
    private int gold = 0;
    private new PlayerCamera camera;
    private string characterName = "TestBUbba";
    private bool freeRunning = false;
    public void addGold(int amount)
    {
        this.gold += amount;
    }

    public string getCName()
    {
        return characterName;
    }

    void Start()
    {
        Application.targetFrameRate = 300;
        Application.runInBackground = true;
        camera = GetComponentInChildren<PlayerCamera>();
        control = GetComponent<CharacterController>();
        Physics.IgnoreLayerCollision(2, 9);
        Physics.IgnoreLayerCollision(2, 8);
        itemHover = GameObject.Instantiate(itemHover);
    }


    float pickupDistance = 2f;
    float npcDistance = 5f;
    private bool busy = false;
    private bool dashing = false;


    
    void Update() {
        detectClosestItemOrNpc();
        if (Input.GetButtonUp("Interact"))
        {
            doInteract();
        }


        if (ableToControl)
        {
            if (control.isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = JumpStrength;
                }
            }

            if (control.isGrounded || !control.isGrounded)
            {
                if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
                {
                    transform.Rotate(rot);
                    GetComponentInChildren<CameraControll>().reset(transform.rotation.eulerAngles);
                    rot.y = 0;
                }
                moveDirection.x = Input.GetAxis("Horizontal");
                moveDirection.z = Input.GetAxis("Vertical");
                Vector3 t = transform.TransformDirection(moveDirection);
                moveDirection.x = t.x * speed;
                moveDirection.z = t.z * speed;


                if(dash)
                {
                    if (dashI < dashes)
                    {
                        if ((Input.GetButton("Right Click")))
                        {
                            checkDash();
                            dashing = true;
                        }
                        else if(dashing)
                        {
                            endingDash();
                        }
                    }
                }

            }
            rot.y += Input.GetAxis("Mouse X") * sensitivity;
        }
        else
        {
            moveDirection = new Vector3(0, moveDirection.y, 0);
        }
        if (!control.isGrounded && !freeRunning)
        {
            moveDirection -= Gravity * Time.deltaTime;
        }
        else if(freeRunning)
        {
            moveDirection.y = 0;
        }
        control.Move(moveDirection * Time.deltaTime);
    }

    public void chooseClass(Class c)
    {
        this.Class = c;
    }

    public void disableDash()
    {
        this.dash = false;
    }

    public void enableDash()
    {
        this.dash = true;
    }

    public void setControl(bool controlablle)
    {
        this.ableToControl = controlablle;
    }

    public GameObject itemHover;

    private RaycastHit dashCast;
    private int dashCount = 0;
    private int maxD = 15;
    private void checkDash()
    {
        float dis = 7.5f;
        Vector3 moveDir = Vector3.zero;
        if(Input.GetButton("Vertical"))
        {
            moveDir += transform.forward * Input.GetAxis("Vertical");
        }
        if(Input.GetButton("Horizontal"))
        {
            moveDir += transform.right * Input.GetAxis("Horizontal");
        }
        
        if(dashCount < maxD)
        {
            dashCount++;
        }
        control.Move((moveDir * dis) * Time.deltaTime);
        camera.setFOV(((float)dashCount) / maxD, true);
    }

    private void endingDash()
    {
        dashCount--;
        camera.setFOV(((float)dashCount) / maxD, true);
        if (dashCount <= 0)
        {
            dashing = false;
        }
    }

    public int getMaxInvSize()
    {
        return maxInventory;
    }

    public int getCurrentInvSize()
    {
        return Inventory.Count;
    }

    public List<Item> getInventory()
    {
        return Inventory;
    }

    private int maxInventory = 200;
    public void addToInventory(Item pickedUp)
    {
        Inventory.Add(pickedUp);
    }

    public void pickup(Item item)
    {
        Inventory.Add(item);
    }

    public void modHealth(int mod)
    {
        health += mod;
        camera.resetHealthBar(health, MaxHealth);
    }

    public void mine(Vein self)
    {
        if(!self.gem)
        {
            sendSystemMessage("You have mined 1 " + self.Ore.itemName);
            Ore o = self.Ore;
            o.genOre();
            addToInventory(o);
        }
        else
        {
            sendSystemMessage("You have mined 1 " + self.geode.itemName);
            Geode g = self.geode;
            addToInventory(g);
        }
    }

    public void minePure(Vein self)
    {
        if (!self.gem)
        {
            sendSystemMessage("You have mine 1 " + self.Ore.itemName);
            Ore Ore = self.Ore;
            Ore.GetComponent<Item>().setChar();
            Ore.GetComponent<Ore>().genPureOre();
            addToInventory(Ore);
        }
        else if (self.gem)
        {
            sendSystemMessage("You have mine 1 " + self.Gem.itemName);
            Gem gem = self.Gem;
            gem.GetComponent<Item>().setChar();
            addToInventory(gem);
        }
    }

    public PlayerCamera getCamera()
    {
        return camera;
    }


    public void openUI()
    {
        GetComponentInChildren<CameraControll>().setControlable(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void closeUI()
    {
        GetComponentInChildren<CameraControll>().setControlable(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void halfControl()
    {
        GetComponentInChildren<CameraControll>().setControlable(true);
    }

    public void removeFromInventory(Item item)
    {
        Inventory.Remove(item);
    }

    public Chat chat;
    public void sendSystemMessage(string message)
    {
        chat.sendSystemMessage(message);
    }
    
    //References the first gameObject, clones it, but replaces the scripts with the dropped item
    //So, on first pickup, it will setActive(false) to that gameobject and replace the reference in the scripts with the deactivated one. allowing me to clone it when i drop
    public void drop(Item i)
    {
        foreach (Item item in Inventory)
        {
            if (item.itemName.Equals(i.itemName))
            {
                Vector3 dropPos = transform.position + (transform.forward * 1.5f);
                GameObject tempReference = GameObject.Instantiate(i.getObject());
                tempReference.transform.position = dropPos;
                tempReference.GetComponent<Item>().set(item);
                removeFromInventory(item);
                getCamera().resetInventory();
                break;
            }
        }
    }

    public void dropAll(Item i)
    {
        List<Item> temp = new List<Item>(Inventory);
        Vector3 dropPos = transform.position + (transform.forward * 1.5f);
        GameObject tempReference = null;
        foreach (Item item in temp)
        {
            if (item.itemName.Equals(i.itemName))
            {
                tempReference = GameObject.Instantiate(i.getObject());
                tempReference.transform.position = dropPos;
                tempReference.GetComponent<Item>().set(item);
                removeFromInventory(item);
                getCamera().resetInventory();
                break;
            }
        }
        temp = new List<Item>(Inventory);
        foreach (Item item in temp)
        {
            if (item.itemName.Equals(i.itemName))
            {
                Ore o = (Ore)item;
                tempReference.GetComponent<Item>().addToStack(o);
                removeFromInventory(item);
                getCamera().resetInventory();
            }
        }
    }

    Item closestItem = null;
    NPC closestNPC = null;
    private void detectClosestItemOrNpc()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 10f);
        float closest = 10f;
        foreach (Collider item in hits)
        {
            if(item.GetComponent<Item>() != null || item.GetComponent<NPC>() != null)
            {
                float distance = Vector3.Distance(transform.position, item.gameObject.transform.position);
                if(item.GetComponent<Item>() != null)
                {
                    if (distance < closest && distance <= pickupDistance)
                    {
                        closest = distance;
                        closestItem = item.GetComponent<Item>();
                    }
                }
                else if(item.GetComponent<NPC>() != null)
                {
                    if (distance < closest && distance <= npcDistance)
                    {
                        closest = distance;
                        closestNPC = item.GetComponent<NPC>();
                    }
                }
            }
        }

        if (closestItem != null)
        {
            if (Vector3.Distance(transform.position, closestItem.transform.position) > pickupDistance)
            {
                closestItem = null;
            }
            else
            {
                itemHover.SetActive(true);
                itemHover.transform.position = closestItem.transform.position;
                if(closestItem.pickupable && closestItem.GetComponent<TrailRenderer>() != null)
                {
                    ParticleSystem.MainModule psMain = itemHover.GetComponent<ParticleSystem>().main;
                    Color c = closestItem.GetComponent<TrailRenderer>().material.color;
                    c.a = 0.12f;
                    psMain.startColor = c;
                }
                
                if(closestItem.GetType() == typeof(Vein))
                {
                    camera.reviewItem(closestItem.itemName,"Mine X  " + ((Vein)closestItem).ores, closestItem.icon);
                }
                else if(closestItem.GetType() == typeof(Container))
                {
                    camera.reviewItem(closestItem.itemName, "Open" , closestItem.icon);
                }
                else
                {
                    camera.reviewItem(closestItem.itemName, "X" + closestItem.getStack().Count, closestItem.icon);
                }
            }
        }
        else
        {
            itemHover.SetActive(false);
            camera.hideItem();
        }
    }

    private void doInteract()
    {
        if (!busy)
        {
            if (closestItem != null && closestNPC == null)
            {
                if (!closestItem.isContainer && closestItem.pickupable)
                {
                    closestItem.GetComponent<Item>().Interact();
                    sendSystemMessage("You picked up " + closestItem.getStack().Count + " " + closestItem.itemName);
                }
                else if (closestItem.isContainer)
                {
                    camera.openContainer(((Container)closestItem).getItems(), (Container)closestItem);
                }
                else
                {
                    closestItem.Interact();
                }
            }
            else if(closestNPC != null)
            {
                closestItem.GetComponent<NPC>().Interact();
            }
        }
        else
        {
            sendSystemMessage("You Are Already Busy");
        }
    }

    public float pushPower = 2.0F;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }
    public void startFreeRun()
    {
        freeRunning = true;
    }

    private float xpMultiplier = 1f;
    public void getXP(float xp)
    {
        this.experience += xp * xpMultiplier;
        if(this.experience >= experienceNeeded)
        {
            this.level++;
            this.experience = experience - experienceNeeded;
            experienceNeeded *= 1.2f;
        }
        camera.refreshXp(experienceNeeded, experience);
    }
}