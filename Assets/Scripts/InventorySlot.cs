using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI countText;

    private ItemData currentItem;
    private int itemCount;

    public void SetItem(ItemData item, int count = 1)
    {
        currentItem = item;
        itemCount = count;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        countText.text = count > 1 ? count.ToString() : "";
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;
        iconImage.sprite = null;
        iconImage.enabled = false;
        countText.text = "";
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public bool IsSameItem(ItemData item)
    {
        return currentItem == item;
    }

    public bool CanStack()
    {
        return currentItem != null && itemCount < currentItem.maxStack;
    }

    public void AddOne()
    {
        itemCount++;
        countText.text = itemCount > 1 ? itemCount.ToString() : "";
    }
}
