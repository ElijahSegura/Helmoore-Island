using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class PlayerCamera : MonoBehaviour {

    private GameObject BuildingObject;
    public Material BuildMaterial;
    public AnimationCurve c;
    public bool holdToRotate = false;
    public GameObject healthBar;
    public GameObject TimeBar;

    [SerializeField]
    private GameObject Menu;

    [SerializeField]
    private bool magnets = true;

    [SerializeField]
    private GameObject InventoryWindow, EquipmentWindow;
    [SerializeField]
    private GameObject Option;
    private GameObject Build;

    private bool CanBuildHere = true;
    
    int currentStruct = 0;

    private float lookLength = 8f;
    private float timer;
    private bool buildmode = false;
    private bool buildDown = false;
    private bool build = false;
    private RaycastHit hit;
    // Use this for initialization

    private bool inv = false;
    private bool invOpen = false;
    void Start () {
        s = smeltingUI.GetComponent<Smelting>();
        player = GetComponentInParent<Character>();
        InventoryWindow.SetActive(false);
        Menu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private bool r;
    private Vector3 Rotation = Vector3.zero;
    public float RotAmount = 90f;
    GameObject lookedAt;
    RaycastHit lookingAt;
    RaycastHit UI;
    private bool e = false;
    public GameObject ui;
    public GameObject itemText, itemAction;
    public InputField Chat;
    private bool moi = false;
    // Update is called once per frame
    private bool busy = false;
    private bool chatting = false;

    public void setBuildable(bool cb)
    {
        CanBuildHere = cb;
    }
    public GameObject buildingMenu;
    private RaycastHit dof;
	void Update () {
        //depth of field
        if(Physics.Raycast(transform.position, transform.forward, out dof, 20f))
        {

        }
        

        //controls first
        if (Input.GetButtonUp("Escape"))
        {
            if (Menu.activeSelf)
            {
                player.closeUI();
                Menu.SetActive(false);
                ui.SetActive(true);
            }
            else {
                if(InventoryWindow.activeSelf)
                {
                    closeInventory();
                }
                else if(Option.activeSelf)
                {
                    Option.SetActive(false);
                    Menu.SetActive(true);
                }
                else if(smeltingUI.activeSelf)
                {
                    player.closeUI();
                    smeltingUI.SetActive(false);
                    ui.SetActive(true);
                }
                else if(forge.activeSelf)
                {
                    player.closeUI();
                    forge.SetActive(false);
                    ui.SetActive(true);
                }
                else
                {
                    player.openUI();
                    Menu.SetActive(true);
                    ui.SetActive(false);
                }
            }
        }

        if(!chatting && !endChat)
        {
            if (Input.GetButtonUp("Submit"))
            {
                player.openUI();
                Chat.ActivateInputField();
                chatting = true;
            }
        }
        else if(!chatting && endChat)
        {
            if (Input.GetButtonUp("Submit"))
            {
                if(!busy)
                {
                    player.closeUI();
                }
                endChat = false;
            }
        }

        if(Input.GetButtonUp("Inventory") && !chatting)
        {
            if(InventoryWindow.activeSelf)
            {
                closeInventory();
            }
            else
            {
                openInventory(false, null);
            }
        }


        if (Input.GetButton("HudMenu"))
        {
            player.openUI();
        }
        else if(Input.GetButtonUp("HudMenu") && !moi)
        {
            if(!busy)
            {
                player.closeUI();
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Locked;
                player.halfControl();
            }
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }






        if(CanBuildHere)
        {
            if (Input.GetButton("Build"))
            {
                if (!buildDown)
                {
                    if (buildmode)
                    {
                        buildmode = false;
                        enableMagnets();
                        disableMagnets();
                        player.enableDash();
                        buildingMenu.SetActive(false);
                    }
                    else
                    {
                        player.disableDash();
                        buildmode = true;
                        if(magnets)
                        {
                            enableMagnets();
                        }
                        
                    }
                }
                buildDown = true;
            }
            else
            {
                buildDown = false;
            }
        }







        //then game feature
        if (buildmode)
        {

            
            if (Input.GetButton("Right Click"))
            {
                if(!holdToRotate)
                {
                    if (!r)
                    {
                        Rotation.y += RotAmount;
                        BuildingObject.transform.Rotate(Rotation);
                        Rotation = Vector3.zero;
                        r = true;
                    }
                }
                else
                {
                    Rotation.y += RotAmount;
                    BuildingObject.transform.Rotate(Rotation);
                    Rotation = Vector3.zero;
                }

            }
            else
            {
                r = false;
            }



            if(BuildingObject != null)
            {
                if (!BuildingObject.GetComponent<Architecture>().canPlace())
                {
                    BuildMaterial.color = new Color(1f, 0f, 0f, 0f);
                }
                else
                {
                    BuildMaterial.color = new Color(0f, 1f, 0f, 0f);
                }
                timer += Time.deltaTime;
                BuildMaterial.SetColor("_EmissionColor", BuildMaterial.color * (c.Evaluate(timer / 2f)));

                if (Physics.Raycast(transform.position, transform.forward, out hit, 10))
                {
                    BuildingObject.SetActive(true);
                    willSnap();
                }
                else
                {
                    BuildingObject.SetActive(false);
                }


                if (Input.GetButton("Click"))
                {
                    if (!build)
                    {
                        if (Physics.Raycast(transform.position, transform.forward, out hit, 10))
                        {
                            if (hit.collider.tag.Equals("Magnet"))
                            {
                                GameObject.Destroy(hit.collider.gameObject);
                            }
                            GameObject temp = GameObject.Instantiate(Build);
                            temp.transform.position = BuildingObject.transform.position;
                            temp.transform.rotation = BuildingObject.transform.rotation;
                            changeMagnets(BuildingObject.tag);
                        }

                    }
                    build = true;
                }
                else
                {
                    build = false;
                }

                if (timer > 2f)
                {
                    timer = 0f;
                }
            }
        }
	}


    private void changeMagnets(String type)
    {
        if(Magnets.Count > 0)
        {
            enableMagnets();
        }
        foreach (GameObject item in FindObjectsOfType<GameObject>())
        {
            if (!item.name.Equals(type + " Magnets") && item.tag.Equals("MagnetParent"))
            {
                if(item.activeSelf)
                {
                    item.SetActive(false);
                    Magnets.Add(item);
                }
            }
        }
    }

    public void resetInventory()
    {
        InventoryWindow.GetComponent<Inventory>().resetInventory();
    }


    private List<GameObject> Magnets = new List<GameObject>();

    private void enableMagnets()
    {
        foreach (GameObject item in Magnets)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }
        Magnets.Clear();
    }

    private void disableMagnets()
    {
        foreach (GameObject item in FindObjectsOfType<GameObject>())
        {
            if(item.tag.Equals("MagnetParent"))
            {
                Magnets.Add(item);
                item.SetActive(false);
            }
        }
    }
    
    private void changeStruct()
    {
        GameObject.Destroy(BuildingObject);
        BuildingObject.SetActive(true);
        foreach (MeshRenderer r in BuildingObject.GetComponentsInChildren<MeshRenderer>())
        {
            Material[] m = new Material[r.materials.Length];
            for (int i = 0; i < r.materials.Length; i++)
            {
                m[i] = BuildMaterial;
            }
            r.materials = m;
        }
        foreach (SkinnedMeshRenderer r in BuildingObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            Material[] m = new Material[r.materials.Length];
            for (int i = 0; i < r.materials.Length; i++)
            {
                m[i] = BuildMaterial;
            }
            r.materials = m;
        }
        GameObject.Destroy(BuildingObject.GetComponentInChildren<MeshCollider>());
        GameObject.Destroy(BuildingObject.GetComponentInChildren<HingeJoint>());
        GameObject.Destroy(BuildingObject.GetComponentInChildren<Rigidbody>());
        foreach (BoxCollider item in BuildingObject.GetComponentsInChildren<BoxCollider>())
        {
            Destroy(item);
        }
        foreach (Transform child in BuildingObject.transform)
        {
            if (child.tag.Equals("MagnetParent"))
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        changeMagnets(BuildingObject.tag);
    }

    public void quit()
    {
        Application.Quit();
    }


    private void willSnap()
    {
        String tag = BuildingObject.tag;
        if ((hit.collider.tag.Equals("Magnet") && hit.collider.transform.parent.gameObject.name.Equals(tag + " Magnets")) && magnets)
        {
            Debug.Log(hit.collider.transform.parent.parent);
            Vector3 rotDif = new Vector3(0, hit.collider.transform.parent.transform.parent.rotation.eulerAngles.y - BuildingObject.transform.rotation.eulerAngles.y, 0);
            Debug.Log(rotDif);
            BuildingObject.transform.Rotate(rotDif, Space.World);
            BuildingObject.transform.position = hit.collider.transform.position;
            
        }
        else
        {
            BuildingObject.transform.position = hit.point;
        }
    }

    public GameObject containerUI;
    public void openContainer(List<Item> Items, GameObject c)
    {
        containerUI.SetActive(true);
        InventoryWindow.GetComponent<Inventory>().setContainer(c);
        openInventory(true, null);
    }


    public Inventory Inventory;
    public void openInventory(bool container, Type filter)
    {
        if(filter != null)
        {
            moi = true;
            Inventory.setFilter(filter);
            InventoryWindow.SetActive(true);
        } 
        else
        {
            moi = true;
            InventoryWindow.SetActive(true);
        }
    }

    public void closeInventory()
    {
        moi = false;
        InventoryWindow.SetActive(false);
        player.closeUI();
        ui.SetActive(true);
        containerUI.SetActive(false);
    }
    
    public void resetHealthBar(int health, int maxHealth)
    {
        healthBar.GetComponent<Image>().fillAmount = (float)health / maxHealth;
    }

    public void openMining()
    {
        TimeBar.transform.parent.gameObject.SetActive(true);
    }

    public void closeMining()
    {
        TimeBar.transform.parent.gameObject.SetActive(false);
    }

    public void updateBar(float percent)
    {
        TimeBar.GetComponent<Slider>().value = percent;
    }

    public GameObject smeltingUI;
    public Smelting s;
    public void openSmelting()
    {
        smeltingUI.SetActive(true);
    }

    public void closeSmelting()
    {
        smeltingUI.SetActive(false);
    }

    private PersonalFurnace furnace;
    public void setFurnace(PersonalFurnace furnace)
    {
        this.furnace = furnace;
    }

    private Character player;

    public Character getPlayer()
    {
        return player;
    }

    public void setSmelting(MetalBar bar)
    {
        s.barName.GetComponent<Text>().text = bar.itemName;
        float max = 0f;
        foreach (Item item in player.getInventory())
        {
            if (item.itemName.Equals(bar.ore.itemName))
            {
                max += ((Ore)item).purity;
            }
        }
        s.Slider.GetComponent<Slider>().maxValue = Mathf.Floor(max);
        s.Slider.GetComponent<Slider>().value = Mathf.Floor(max);
    }

    public void startSmelting()
    {
        furnace.setMax((int)s.Slider.GetComponent<Slider>().value);
        s.Timer.SetActive(true);
        s.Timer.GetComponentInChildren<Text>().text = s.Slider.GetComponent<Slider>().value + "";
        s.StartButton.SetActive(false);
        s.startSmelting();
        furnace.startSmelting();
    }

    public void smeltOne()
    {
        s.Timer.GetComponentInChildren<Text>().text = s.Slider.GetComponent<Slider>().value - furnace.getCurrent() + "";
    }

    public void stopSmelting()
    {
        s.Timer.SetActive(false);
        s.StartButton.SetActive(true);
        s.stopSmelting();
        refreshFurnace();
    }

    private void refreshFurnace()
    {
        MetalBar bar = furnace.getCurrentBar();
        float max = 0f;
        foreach (Item item in player.getInventory())
        {
            if (item.itemName.Equals(bar.ore.itemName))
            {
                max += ((Ore)item).purity;
            }
        }
        s.Slider.GetComponent<Slider>().maxValue = Mathf.Floor(max);
        s.Slider.GetComponent<Slider>().value = Mathf.Floor(max);
    }

    public void updateFurnaceBar(float percent)
    {
        s.updateTimer(percent);
    }

    public void changeFurnaceBar(GameObject bar)
    {
        furnace.changeBar(bar);
    }

    public GameObject forge;
    public void openForge()
    {
        forge.SetActive(true);
    }

    public void setBusy(bool b)
    {
        this.busy = b;
    }

    public void setChatting(bool c)
    {
        this.chatting = c;
    }

    private bool endChat = false;
    public void endChatting()
    {
        setChatting(false);
        endChat = true;
    }

    public void setForgingRecipe(Recipe r)
    {

    }

    public bool hasContainer()
    {
        return InventoryWindow.GetComponent<Inventory>().hasContainer;
    }

    public GameObject InvLook;
    public GameObject farmingInventory;
    public void openFarming(GameObject Farm)
    {
        farmingInventory.SetActive(true);
        foreach (CropPlot c in Farm.GetComponentsInChildren<CropPlot>())
        {
            foreach (SeedBox s in farmingInventory.GetComponentsInChildren<SeedBox>())
            {
                if(c.index == s.index) {
                    s.setCP(c.gameObject);
                }
            }
        }
        openInventory(true, typeof(Seed));
    }
}
