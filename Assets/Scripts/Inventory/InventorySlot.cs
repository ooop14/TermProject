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

        // ✨ [중요] 아이콘 이미지만 끄고, 슬롯 배경은 그대로 둡니다.
        iconImage.sprite = null;
        iconImage.enabled = false; 
        
        countText.text = "";
    }

    // 이 슬롯의 아이템 데이터를 반환합니다.
    public ItemData GetItemData()
    {
        return currentItem;
    }

    // 이 슬롯의 아이템 개수를 반환합니다.
    public int GetItemCount()
    {
        return itemCount;
    }

    // 이 슬롯에서 지정된 양만큼 아이템을 제거합니다.
    public void RemoveAmount(int amount)
    {
        itemCount -= amount;

        // 아이템을 다 썼으면 슬롯을 비웁니다.
        if (itemCount <= 0)
        {
            ClearSlot();
        }
        else // 아직 남았으면 개수만 업데이트
        {
            countText.text = itemCount.ToString();
        }
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
