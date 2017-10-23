using UnityEngine;
using UnityEngine.UI;

public class SeedBox : MonoBehaviour {
    public int index;
    private CropPlot cropPlot;

    void Start()
    {
        if(cropPlot != null)
        {
            if (cropPlot.doesHaveCrop())
            {
                transform.GetChild(0).GetComponent<Image>().sprite = cropPlot.getCrop().icon;
            }
        }
    }

    public void setCrop()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = cropPlot.getChar().gameObject.GetComponent<Inventory>().seed.icon;
        cropPlot.setCrop(cropPlot.getChar().gameObject.GetComponent<Inventory>().seed);
    }    

    public void setCP(GameObject CroppedPlot)
    {
        this.cropPlot = CroppedPlot.GetComponent<CropPlot>();
    }

    void Update()
    {
        GetComponentInChildren<Slider>().value = cropPlot.getProgress();
    }
}
