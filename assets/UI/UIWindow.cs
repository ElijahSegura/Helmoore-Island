using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindow : MonoBehaviour {
    private Resolution reference;
    public bool square;
    public GameObject content;
    public bool keepRatio;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        resizeContent();
    }

    public void Drag()
    {
        reference = Screen.currentResolution;
        Vector3 point = Input.mousePosition;
        float xRatio = point.x / (reference.width / 2);
        float yRatio = point.y / (reference.height / 2);
        xRatio -= 1f;
        yRatio -= 1f;
        point.x = (reference.width * xRatio);
        point.y = (reference.height * yRatio);
        point.z = 0;
        transform.localPosition = point;
    }

    public void resizeContent()
    {
        if(content != null)
        {
            content = GetComponentInChildren<GridLayoutGroup>().gameObject;
            t = GetComponent<RectTransform>();
            float size = content.GetComponent<RectTransform>().rect.width;
            content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(size / 4, size / 4);
        }
    }

    Vector3 ogMousePos;
    Vector2 ogSize;
    Vector3 ogLocation;
    RectTransform t;
    public void resize()
    {
        Vector3 mousepos = Input.mousePosition;
        float dX, dY;
        dX = ogMousePos.x - mousepos.x;
        dY = ogMousePos.y - mousepos.y;
        if(!square)
        {
            t.sizeDelta = new Vector2(ogSize.x - dX, ogSize.y + dY);
            t.localPosition = new Vector3(ogLocation.x - (dX / 2), ogLocation.y - (dY / 2), ogLocation.z);
        }
        else
        {
            t.sizeDelta = new Vector2(ogSize.x - dX, ogSize.x - dX);
            t.localPosition = new Vector3(ogLocation.x - (dX / 2), ogLocation.x + (dX / 2) , ogLocation.z);
        }
        if(content != null)
        {
            float size = content.GetComponent<RectTransform>().rect.width;
            content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(size / 4, size / 4);
        }
    }

    public void startResize()
    {
        ogMousePos = Input.mousePosition;
        t = GetComponent<RectTransform>();
        ogSize = t.sizeDelta;
        ogLocation = t.localPosition;
    }
}
