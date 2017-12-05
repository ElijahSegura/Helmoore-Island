using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class PlayerCamera : MonoBehaviour {

    private GameObject BuildingObject;
    public Material BuildMaterial;
    public AnimationCurve c, fov;
    public bool holdToRotate = false;
    public GameObject healthBar;
    public GameObject TimeBar;
    public Image expBar;
    public GameObject itemReview;

    [SerializeField]
    private GameObject Menu;

    [SerializeField]
    private bool magnets = true;

    internal GameObject getFarmUI()
    {
        return farmingInventory;
    }

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
    private float fieldOfView;
    void Start () {
        fieldOfView = GetComponent<Camera>().fieldOfView;
        s = smeltingUI.GetComponent<Smelting>();
        player = GetComponentInParent<Character>();
        InventoryWindow.SetActive(false);
        Menu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void setBuildingObject(GameObject obj)
    {
        if(BuildingObject != null)
        {
            Destroy(BuildingObject);
        }
        BuildingObject = obj;
        Build = BuildingObject;
        BuildingObject = GameObject.Instantiate(BuildingObject);
        foreach(Collider c in BuildingObject.GetComponentsInChildren<Collider>())
        {
            Destroy(c);
        }
        for (int i = 0; i < BuildingObject.transform.childCount; i++)
        {
            if(BuildingObject.transform.GetChild(i).tag.Equals("MagnetParent"))
            {
                Destroy(BuildingObject.transform.GetChild(i).gameObject);
            }
        }
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
    private bool mouseEnabled = false;
    // Update is called once per frame
    private bool busy = false;
    private bool chatting = false;

    public void setBuildable(bool cb)
    {
        CanBuildHere = cb;
    }

    public GameObject buildingMenu;
    private RaycastHit dof;

    private Vector3 buildOrigin;
    private float distance;
    public void setBuildOrigin(Vector3 origin, float area)
    {
        this.buildOrigin = origin;
        this.distance = area;
    }

    private bool canBePlaced = false;
	void Update () {
        if(Input.GetButtonUp("Click"))
        {
            castFireBall();
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
                }
                else if(forge.activeSelf)
                {
                    player.closeUI();
                    forge.SetActive(false);
                }
                else if(farmingInventory.activeSelf)
                {
                    player.closeUI();
                    farmingInventory.SetActive(false);
                } 
                else if(buildingMenu.activeSelf)
                {
                    buildingMenu.SetActive(false);
                }
                else if(crushingMenu.activeSelf)
                {
                    crushingMenu.SetActive(false);
                }
                else if(buildmode)//MUST BE LAST (so that you don't disable building when trying to close the menu)
                {
                    buildmode = false;
                    buildingMenu.SetActive(false);
                    Destroy(BuildingObject);
                    disableMagnets();
                    player.enableDash();
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
                FindObjectOfType<Character>().setControl(false);
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


        if (Input.GetButtonUp("HudMenu"))
        {
            if (mouseEnabled)
            {
                if (!busy)
                {
                    player.closeUI();
                    mouseEnabled = false;
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
                mouseEnabled = true;
                player.openUI();
            }
        }






        if(CanBuildHere)
        {
            if (Input.GetButtonUp("Build"))
            {
                if(buildingMenu.activeSelf)
                {
                    buildingMenu.SetActive(false);
                    player.closeUI();
                }
                else if(!buildingMenu.activeSelf || !buildmode)
                {
                    enableMagnets();
                    buildingMenu.SetActive(true);
                    player.openUI();
                    buildmode = true;
                    player.disableDash();
                }
            }
        }







        //then game feature
        if (buildmode)
        {
            checkBuild();
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
                
                timer += Time.deltaTime;

                if (Physics.Raycast(transform.position, transform.forward, out hit, 10))
                {
                    if(Vector3.Distance(hit.point, buildOrigin) < distance)
                    {
                        BuildingObject.SetActive(true);
                        BuildingObject.transform.position = hit.point;
                        canBePlaced = true;
                    }
                    else if(Vector3.Distance(hit.point, buildOrigin) > distance)
                    {
                        BuildingObject.SetActive(true);
                        BuildingObject.transform.position = hit.point;
                        canBePlaced = false;
                    }
                    else
                    {
                        canBePlaced = false;
                    }
                }
                else
                {
                    BuildingObject.SetActive(false);
                }

                if(magnets)
                {
                    willSnap();
                }

                if(Input.GetButtonDown("Click") && !buildingMenu.activeSelf && canBePlaced)
                {
                    GameObject justBuilt = GameObject.Instantiate(Build);
                    justBuilt.transform.position = BuildingObject.transform.position;
                    justBuilt.transform.localRotation = BuildingObject.transform.localRotation;
                }
            }
        }
        else
        {

        }
	}


    public GameObject tempMagicSpell;
    private void castFireBall()
    {
        GameObject Spell = GameObject.Instantiate(tempMagicSpell);
        Vector3 spawn = FindObjectOfType<Character>().gameObject.transform.position + transform.forward + new Vector3(0,1,0);
        Spell.transform.position = spawn;
        Spell.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
    }

    public void checkBuild()
    {
        Architecture a = BuildingObject.GetComponent<Architecture>();
        
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
        FindObjectOfType<Inventory>().resetInventory();
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
        if (hit.collider != null)
        {
            if ((hit.collider.tag.Equals("Magnet") && hit.collider.transform.parent.gameObject.name.Equals(tag)))
            {
                Vector3 rotDif = new Vector3(0, (hit.collider.transform.parent.transform.parent.rotation.eulerAngles.y - BuildingObject.transform.rotation.eulerAngles.y), 0);
                BuildingObject.transform.Rotate(rotDif, Space.World);
                BuildingObject.transform.position = hit.collider.transform.position;
            }
            else
            {
                BuildingObject.transform.position = hit.point;
            }
        }
    }

    internal void refreshXp(float maxXp, float xp)
    {
        expBar.fillAmount = xp / maxXp;
    }

    public GameObject containerUI;
    public void openContainer(List<Item> Items, Container c)
    {
        containerUI.SetActive(true);
        ContainerInventory containerInventory = containerUI.GetComponent<UIWindow>().content.gameObject.GetComponent<ContainerInventory>();
        containerInventory.setContainer(c.GetComponent<Container>());
        containerInventory.loadInventory(c.containerItems);
        openInventory(true, null);
    }


    public Inventory Inventory;
    public ContainerInventory conInventory;
    public void openInventory(bool container, Type filter)
    {
        if(filter != null)
        {
            mouseEnabled = true;
            Inventory.setFilter(filter);
            InventoryWindow.SetActive(true);
        } 
        else
        {
            mouseEnabled = true;
            InventoryWindow.SetActive(true);
        }
    }

    public void openInventory()
    {
        mouseEnabled = true;
        InventoryWindow.SetActive(true);
    }

    public void closeInventory()
    {
        mouseEnabled = false;
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
        foreach (FarmingUI item in farmingInventory.GetComponent<UIWindow>().content.GetComponentsInChildren<FarmingUI>())
        {
            item.setFarm(Farm.GetComponent<FarmPlot>());
        }
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

    public GameObject crushingMenu;
    public void openCrushingMenu(GeodeSplitter splitter)
    {
        crushingMenu.SetActive(true);
        openInventory(false, typeof(Geode));
    }

    public PostProcessingProfile ppe;
    public void setFOV(float curveLocation, bool dash)
    {
        ChromaticAberrationModel.Settings ca = ppe.chromaticAberration.settings;
        GetComponent<Camera>().fieldOfView = fieldOfView * fov.Evaluate(curveLocation);
        if(dash)
        {
            if(curveLocation == 0)
            {
                ca.intensity = 0.1f;
                ppe.chromaticAberration.settings = ca;
            }
            else
            {
                ca.intensity = 1f;
                ppe.chromaticAberration.settings = ca;
            }
        }
    }

    public void reviewItem(string name, string amountOrAction, Sprite icon)
    {
        itemReview.SetActive(true);
        itemReview.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = icon;
        itemReview.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = name;
        itemReview.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = amountOrAction;
    }

    public void hideItem()
    {
        itemReview.SetActive(false);
    }


    public void updateExp(float percent)
    {
        expBar.fillAmount = percent;
    }

    
    public void AntiAliasing(bool enabled)
    {
        ppe.antialiasing.enabled = enabled;
    }

    public void DepthOFFeild(bool enabled)
    {
        GetComponent<DepthOfField>().enabled = enabled;
    }

    public void MotionBlur(bool enabled)
    {
        ppe.motionBlur.enabled = enabled;
    }

    public void Bloom(bool enabled)
    {
        ppe.bloom.enabled = enabled;
    }


}
