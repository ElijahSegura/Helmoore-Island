using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using assets.Resources.Player;

public class Character : NetworkBehaviour {
    private Vector3 moveDirection = Vector3.zero;
    private Mob target;
    CharacterController control;
    public float speed, JumpStrength, sensitivity;
    public Vector3 Gravity;
    private bool RC = false;
    private int dashes = 5;
    private int health, MaxHealth, healthRegen;
    private int mana, MaxMana, manaRegen;
    private float experience = 0.00f, experienceNeeded = 100.00f;
    private Class Class;
    private Vector3 rot = new Vector3(0, 0, 0);
    private double attackSpeed;
    public int level = 1;
    
    private int dashI = 0;
    private bool ableToControl = true;
    private bool dash = true;
    private List<Item> Inventory = new List<Item>();
    private int gold = 0;
    private string characterName = "TestBUbba";
    private bool freeRunning = false;

    #region needed References
    [SerializeField]
    private PlayerCamera camera;
    [SerializeField]
    private Animator characterAnimation;
    [SerializeField]
    private CameraControll cc;
    #endregion

    #region Getters And Setters
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
    public string getCName()
    {
        return characterName;
    }

    public void setControl(bool controlablle)
    {
        this.ableToControl = controlablle;
    }
    #endregion


    public void stopFreeRun()
    {
        freeRunning = false;
    }

    public void addGold(int amount)
    {
        this.gold += amount;
    }

    

    float pickupDistance = 2f;
    float npcDistance = 5f;
    private bool busy = false;
    private bool dashing = false;
    private float dashRegenTime = 6f;
    private float regenTimer = 6f;

    [SyncVar(hook = "OnServerStateChanged")]
    public PlayerState state;

    private PlayerState predictedState;
    private List<PlayerInput> pendingMoves;

    void Awake()
    {
        InitState();
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            pendingMoves = new List<PlayerInput>();
            GetComponentInChildren<PlayerCamera>().enabled = true;
        }
        else
        {
            Destroy(GetComponentInChildren<PlayerCamera>().gameObject);
        }
    }

    private void InitState()
    {
        state = new PlayerState
        {
            timestamp = 0,
            position = Vector3.zero,
            rotation = Quaternion.Euler(0, 0, 0),
        };
    }

    [Command]
    void CmdMoveOnServer(PlayerInput playerInput)
    {
        state = ProcessPlayerInput(state, playerInput);
    }

    void SyncState()
    {
        if (isServer)
        {
            transform.position = state.position;
            transform.rotation = state.rotation;
            return;
        }

        PlayerState stateToRender = isLocalPlayer ? predictedState : state;

        transform.position = Vector3.Lerp(transform.position,
            stateToRender.position * Settings.PlayerLerpSpacing,
            Settings.PlayerLerpEasing);
        transform.rotation = Quaternion.Lerp(transform.rotation,
            stateToRender.rotation,
            Settings.PlayerLerpEasing);
    }

    private PlayerInput GetPlayerInput()
    {
        PlayerInput playerInput = new PlayerInput();
        playerInput.forward += (sbyte)(Input.GetKey(KeyCode.W) ? 1 : 0);
        playerInput.forward += (sbyte)(Input.GetKey(KeyCode.S) ? -1 : 0);
        playerInput.rotate += (sbyte)(Input.GetKey(KeyCode.D) ? 1 : 0);
        playerInput.rotate += (sbyte)(Input.GetKey(KeyCode.A) ? -1 : 0);
        if (playerInput.forward == 0 && playerInput.rotate == 0)
            return null;
        return playerInput;
    }

    public PlayerState ProcessPlayerInput(PlayerState previous, PlayerInput playerInput)
    {
        Vector3 newPosition = previous.position;
        Quaternion newRotation = previous.rotation;

        if (playerInput.rotate != 0)
        {
            newRotation = previous.rotation
                * Quaternion.Euler(Vector3.up
                    * Settings.PlayerFixedUpdateInterval
                    * Settings.PlayerRotateSpeed
                    * playerInput.rotate);
        }
        if (playerInput.forward != 0)
        {
            newPosition = previous.position
                + newRotation
                    * Vector3.forward
                    * playerInput.forward
                    * Settings.PlayerFixedUpdateInterval
                    * Settings.PlayerMoveSpeed;
        }
        return new PlayerState
        {
            timestamp = previous.timestamp + 1,
            position = newPosition,
            rotation = newRotation
        };
    }

    public void OnServerStateChanged(PlayerState newState)
    {
        state = newState;
        if (pendingMoves != null)
        {
            while (pendingMoves.Count >
                  (predictedState.timestamp - state.timestamp))
            {
                pendingMoves.RemoveAt(0);
            }
            UpdatePredictedState();
        }
    }

    public void UpdatePredictedState()
    {
        predictedState = state;
        foreach (PlayerInput playerInput in pendingMoves)
        {
            predictedState = ProcessPlayerInput(predictedState, playerInput);
        }
    }
    

    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer)
        {
            Debug.Log("NOT MY CHARACTER!!");
            return;
        }
        gameObject.name = "LOCAL Player";
        Application.targetFrameRate = 300;
        camera = GetComponentInChildren<PlayerCamera>();
        control = GetComponent<CharacterController>();
        Physics.IgnoreLayerCollision(9, 10);
        Physics.IgnoreLayerCollision(2, 10);

        if (isLocalPlayer)
        {
            pendingMoves = new List<PlayerInput>();
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<PlayerCamera>().enabled = true;
        }
        else
        {
            Destroy(GetComponentInChildren<PlayerCamera>().gameObject);
            //Destroy(GetComponentInChildren<Camera>().gameObject);
            //GetComponentInChildren<PlayerCamera>().enabled = false;
            //GetComponentInChildren<Camera>().enabled = false;
        }
        base.OnStartLocalPlayer();
    }

    //void Start()
    //{
    //    state = new PlayerState()
    //    {
    //        moveDirection = Vector3.zero,
    //        rotation = Quaternion.Euler(0, 0, 0)
    //    };

    //    if(!isLocalPlayer)
    //    {
    //        Debug.Log("NOT MY CHARACTER!");
    //        return;
    //    }
    //    //Application.targetFrameRate = 300;
    //    //camera = GetComponentInChildren<PlayerCamera>();
    //    //control = GetComponent<CharacterController>();
    //    //Physics.IgnoreLayerCollision(9, 10);
    //    //Physics.IgnoreLayerCollision(2, 10);
    //}

       

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            detectClosestItemOrNpc();
            if (Input.GetButtonUp("Interact"))
            {
                doInteract();
            }
            //CmdMoveOnServer(GetPlayerActions());

            if (ableToControl)
            {
                //ProcessPlayerInput(state, control);
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
                        //transform.rotation = rot;
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
                moveDirection.y -= Gravity.magnitude * Time.deltaTime;
            }
            control.Move(moveDirection * Time.deltaTime);
        }
        //SyncState();
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

    

    public GameObject itemHover;

    private RaycastHit dashCast;
    private int dashCount = 0;
    private int maxD = 15;
    private float dashTime = 5f;
    private float maxDashTime = 5f;
    private void checkDash()
    {
        float dis = 15f;
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
        dashTime -= Time.deltaTime;
        camera.updateDashBar(dashTime / maxDashTime);
        control.Move((moveDir * dis) * Time.deltaTime);
        camera.setFOV(((float)dashCount) / maxD, true);
    }


    public void teleport(Vector3 pos)
    {
        transform.position = pos;
    }

    public void teleport(GameObject target)
    {

    }

    public void addRegen()
    {
        if(regenTimer < 6f)
        {
            regenTimer += Time.deltaTime;
        }
    }

    private void endingDash()
    {
        dashCount--;
        regenTimer = dashRegenTime * (dashTime / maxDashTime);
        camera.setFOV(((float)dashCount) / maxD, true);
        if (dashCount <= 0.5f)
        {
            dashing = false;
        }
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
        cc.setControlable(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void closeUI()
    {
        cc.setControlable(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void halfControl()
    {
        cc.setControlable(true);
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
                    if (distance < closest || distance <= npcDistance)
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
        if(closestNPC != null)
        {
            if (Vector3.Distance(transform.position, closestNPC.transform.position) > npcDistance)
            {
                closestNPC = null;
            }
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
                closestNPC.GetComponent<NPC>().Interact();
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