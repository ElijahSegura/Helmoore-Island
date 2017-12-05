using UnityEngine;

public class CropPlot : Item {

    Crop c;
    public int index;
    private bool hasCrop = false;
    private float cropProgress = 0f;
    public GameObject smallcrop, midcrop;
    private GameObject crop;
    private float growTime = 0f;
    private Seed seed;
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
            Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);
            crop.transform.Rotate(rotation);
            crop.transform.SetParent(transform);
            hasCrop = true;
            FindObjectOfType<Character>().removeFromInventory(s);
            FindObjectOfType<PlayerCamera>().resetInventory();
            cropProgress = 0f;
            growTime = s.GrownCrop.growTime;
            this.seed = s;
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
	}

    public bool doesHaveCrop()
    {
        return hasCrop;
    }

    private float time = 0f;
    private bool mid = false;
    private bool full = false;
    public void grow()
    {
        if(hasCrop && !full)
        {
            time += Time.deltaTime;
            cropProgress = time / (growTime * 60);
            if(cropProgress >= .5 && !mid)
            {
                Destroy(crop);
                crop = GameObject.Instantiate(midcrop);
                crop.transform.position = transform.position;
                mid = true;
            }
            else if (cropProgress >= 1f && !full)
            {
                Destroy(crop);
                crop = GameObject.Instantiate(seed.GrownCrop.getObject());
                crop.transform.position = transform.position;
                full = true;
                Destroy(crop.GetComponent<Rigidbody>());
            }
        }
    }

    public float getProgress()
    {
        return cropProgress;
    }
}
