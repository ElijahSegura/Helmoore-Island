using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GeodeCrusherUI : MonoBehaviour, IDropHandler {
    private Geode geode;
    private bool hasGeode = false;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject geode = eventData.pointerDrag;
        if(geode.GetComponent<UIInventoryItem>() != null)
        {
            this.geode = ((Geode)(geode.GetComponent<UIInventoryItem>().getItem()));
            transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            transform.GetChild(1).GetComponent<Image>().sprite = this.geode.icon;
            hasGeode = true;
        }
    }

    public void crush()
    {
        if(hasGeode)
        {
            Gem gem = geode.split();
            Character c = FindObjectOfType<Character>();
            Debug.Log(gem);
            if (gem != null)
            {
                c.addToInventory(gem);
                foreach (Item item in c.getInventory())
                {
                    if (item.itemName.Equals(geode.itemName))
                    {
                        c.removeFromInventory(item);
                        break;
                    }
                }
            }
            else
            {
                foreach (Item item in c.getInventory())
                {
                    if (item.itemName.Equals(geode.itemName))
                    {
                        c.removeFromInventory(item);
                        break;
                    }
                }
            }
            bool hasAnother = false;
            foreach (Item item in c.getInventory())
            {
                if (item.itemName.Equals(geode.itemName))
                {
                    hasAnother = true;
                    break;
                }
            }
            if (!hasAnother)
            {
                geode = null;
                hasGeode = false;
                transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
            c.getCamera().resetInventory();
        }
    }
}
