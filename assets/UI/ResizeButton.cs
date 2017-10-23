using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeButton : MonoBehaviour {
    public void resize()
    {
        this.GetComponentInParent<UIWindow>().resize();
    }

    public void startResize()
    {
        GetComponentInParent<UIWindow>().startResize();
    }
}
