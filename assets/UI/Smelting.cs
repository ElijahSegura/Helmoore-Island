using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Smelting : MonoBehaviour {

    public GameObject barName;
    public GameObject image;
    public GameObject max;
    public GameObject Slider;
    public GameObject Timer;
    public GameObject StartButton;
    public GameObject Disabler;


    public void changeValue()
    {
        max.GetComponent<Text>().text = Slider.GetComponent<Slider>().value + "";
    }

    public void updateTimer(float percent)
    {
        Timer.GetComponent<Slider>().value = percent;
    }

    public void startSmelting()
    {
        this.Disabler.SetActive(true);
    }

    public void stopSmelting()
    {
        this.Disabler.SetActive(false);
    }
    
}
