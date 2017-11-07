using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {
    Vector3 moveDirection = Vector3.zero;
    CharacterController control;
    public float speed, JumpStrength, Gravity, sensitivity;
    private bool female = false;
    private bool RC = false;
    private int dashes = 5;
    private int health, MaxHealth, healthRegen;
    private int mana, MaxMana, manaRegen;
    private double exp = 0.00, nextLevel = 100.00;
    private Class Class;
    private Vector3 rot = new Vector3(0, 0, 0);
    private double attackSpeed, regenRate;
    private int dashI = 0;
    private bool ableToControl = true;
    private bool dash = true;
    private List<Item> Inventory = new List<Item>();
    private new PlayerCamera camera;

    private string characterName = "TestBUbba";
    // Update is called once per frame

    public string getCName()
    {
        return characterName;
    }

    void Start()
    {
        Application.targetFrameRate = 300;
        camera = GetComponentInChildren<PlayerCamera>();
        control = GetComponent<CharacterController>();
        Physics.IgnoreLayerCollision(9, 10);
        Physics.IgnoreLayerCollision(2, 10);
    }


    float pickupDistance = 2f;
    float displayDistance = 10f;
    private bool busy = false;
    private bool dashing = false;


    
    void Update() {
        detectClosestItem();
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


                if (dashI < dashes)
                {
                    if (Input.GetButtonDown("Right Click") && !dashing)
                    {
                        dashCount = 5;
                        dashing = true;
                        checkDash();
                    }
                    else if (dashing)
                    {
                        checkDash();
                    }
                }

            }
            rot.y += Input.GetAxis("Mouse X") * sensitivity;
        }
        else
        {
            moveDirection = new Vector3(0, moveDirection.y, 0);
        }
        if (!control.isGrounded)
        {
            moveDirection.y -= Gravity * Time.deltaTime;
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

    private RaycastHit dashCast;
    private int dashCount = 10;
    private void checkDash()
    {
        Vector3 pos = (transform.position + (transform.forward * 2));
        Vector3 to = -transform.up;

        float dis = 1.3f;




        //if nothing is in the way
        if (dashCount > 0)
        {
            if (!Physics.Raycast(transform.position, transform.forward, dis))
            {
                //if something is close to player, put player in front of it
                if (Physics.Raycast(pos, to, out dashCast, dis))
                {
                    if (dashCast.collider.tag.Equals("Ground"))
                    {
                        float differenceY = pos.y - dashCast.point.y;
                        transform.position = transform.position += transform.forward * dis;
                        dashCount--;
                        foreach (Fade g in FindObjectsOfType<Fade>())
                        {
                            g.initDelay();
                        }
                    }
                }
                else if (!Physics.Raycast(pos, to, dis))
                {
                    transform.position += transform.forward * dis;
                    foreach (Fade g in FindObjectsOfType<Fade>())
                    {
                        g.initDelay();
                    }
                    dashCount--;
                }
            }
            else
            {
                dashing = false;
            }
        }
        else
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
        sendSystemMessage("You have mine 1 " + self.Ore.GetComponent<Item>().itemName);
        Ore o = (Ore)self.Ore.GetComponent<Item>();
        o.genOre();
        o.setChar();
        addToInventory(o);
    }

    public void minePure(Vein self)
    {
        if (!self.gem)
        {
            sendSystemMessage("You have mine 1 " + self.Ore.GetComponent<Item>().itemName);
            GameObject Ore = GameObject.Instantiate(self.Ore);
            Ore.GetComponent<Item>().setChar();
            Ore.GetComponent<Ore>().genPureOre();
            Ore.GetComponent<Ore>().Interact();
        }
        else if (self.gem)
        {
            sendSystemMessage("You have mine 1 " + self.Gem.GetComponent<Item>().itemName);
            GameObject Ore = GameObject.Instantiate(self.Gem);
            Ore.GetComponent<Item>().setChar();
            Ore.GetComponent<Gem>().Interact();
        }
    }

    public PlayerCamera getCamera()
    {
        return camera;
    }


    public void openUI()
    {
        GetComponentInChildren<CameraControll>().setControlablle(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void closeUI()
    {
        GetComponentInChildren<CameraControll>().setControlablle(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void halfControl()
    {
        GetComponentInChildren<CameraControll>().setControlablle(true);
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
    private void detectClosestItem()
    {
        float closest = 10f;
        foreach (Item item in FindObjectsOfType<Item>())
        {
            float distance = Vector3.Distance(transform.position, item.gameObject.transform.position);
            if (distance < closest && distance <= pickupDistance)
            {
                closest = distance;
                closestItem = item;
            }
        }



        if (closestItem != null)
        {
            //closestItem.setAsActive();
        }
    }

    private void doInteract()
    {
        if (!busy)
        {
            if (closestItem != null)
            {
                if (!closestItem.isContainer && closestItem.pickupable)
                {
                    closestItem.GetComponent<Item>().Interact();
                    sendSystemMessage("You picked up " + closestItem.getStack().Count + " " + closestItem.itemName);
                }
                else if (closestItem.isContainer)
                {
                    camera.openContainer(((Container)closestItem).getItems(), closestItem.gameObject);
                }
                else
                {
                    closestItem.Interact();
                }
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
}





/*

    if (dashI < dashIter && RC)
                {
                    transform.Rotate(rot);
                    GetComponentInChildren<CameraControll>().reset(transform.rotation.eulerAngles);
                    rot.y = 0;
                    dashC();
                    dashI++;
                }
                else
                {
                    if (Input.GetButtonUp("Right Click") && dashes > 0 && dash)
                    {
                        if (!RC)
                        {
                            transform.Rotate(rot);
                            GetComponentInChildren<CameraControll>().reset(transform.rotation.eulerAngles);
                            rot.y = 0;
                            dashC();
                            dashI++;
                            dashes--;
                        }
                        RC = true;
                    }
                    else
                    {
                        RC = false;
                        dashI = 0;
                    }
                }
    */
