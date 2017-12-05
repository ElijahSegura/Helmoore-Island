using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour {

	public void switchEnabled(GameObject item)
    {
        if(item.activeSelf)
        {
            item.SetActive(false);
        }
        else
        {
            item.SetActive(true);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
