using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Resolution reference;
    public bool square;
    public GameObject HUD;
    public GameObject content;
    public bool keepRatio;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        transform.SetParent(HUD.transform, false);
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
        else if(keepRatio)
        {

        }
        else
        {
            t.sizeDelta = new Vector2(ogSize.x - dX, ogSize.x - dX);
            t.localPosition = new Vector3(ogLocation.x - (dX / 2), ogLocation.x + (dX / 2) , ogLocation.z);
        }
        if(content != null)
        {
            float size = content.GetComponent<RectTransform>().rect.width;
            float margin = content.GetComponent<GridLayoutGroup>().padding.left + content.GetComponent<GridLayoutGroup>().padding.right;
            content.GetComponent<GridLayoutGroup>().cellSize = new Vector2((size / 4) - margin, (size / 4) - margin);
        }
    }

    public void startResize()
    {
        ogMousePos = Input.mousePosition;
        t = GetComponent<RectTransform>();
        ogSize = t.sizeDelta;
        ogLocation = t.localPosition;
    }


    private RectTransform size;
    public void OnBeginDrag(PointerEventData eventData)
    {
        size = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + new Vector3(size.sizeDelta.x / 4, -(size.sizeDelta.y / 4), 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
