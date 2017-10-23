using UnityEngine;

public class CropPlot : Item {

    Crop c;
    public int index;
    private bool hasCrop = false;
    private float cropProgress = 0f;
    public GameObject smallcrop, midcrop;
    private GameObject crop;
    public override GameObject getObject()
    {
        return null;
    }

    public Crop getCrop()
    {
        return c;
    }

    public override void Interact()
    {
        getChar().getCamera().openFarming(transform.parent.gameObject);
    }

    public void setCrop(Seed s)
    {
        if(!hasCrop)
        {
            crop = GameObject.Instantiate(smallcrop);
            crop.transform.position = transform.position;
            Vector3 rotation = new Vector3(0, 0, Random.Range(0f, 360f));
            crop.transform.Rotate(rotation);
            crop.transform.SetParent(transform);
            hasCrop = true;
            FindObjectOfType<Character>().removeFromInventory(s);
            FindObjectOfType<PlayerCamera>().resetInventory();
            cropProgress = 0f;
        }
    }
 
    public override void set(Item i)
    {
        
    }

    void Start () {
        setChar();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).tag.Equals("Item"))
            {
                hasCrop = true;
            }
        }
        Destroy(GetComponentInChildren<Rigidbody>());
        GetComponentInChildren<Item>().pickupable = false;
        c = GetComponentInChildren<Crop>();
        addE();
	}

    public bool doesHaveCrop()
    {
        return hasCrop;
    }

    private float time = 0f;
    void Update()
    {
        if(hasCrop)
        {
            time += Time.deltaTime;
            cropProgress = time / (c.growTime * 60);
            if(cropProgress >= .5)
            {
                Destroy(crop);
                crop = GameObject.Instantiate(midcrop);
            }
            else if(cropProgress >= 1f)
            {
                Destroy(crop);
                crop = GameObject.Instantiate(getChar().gameObject.GetComponent<Inventory>().seed.GrownCrop);
                Destroy(crop.GetComponent<Rigidbody>());
            }
        }
    }

    public float getProgress()
    {
        return cropProgress;
    }
}
