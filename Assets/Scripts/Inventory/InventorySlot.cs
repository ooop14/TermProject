using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public Image iconImage;       // 아이템 아이콘 (자식 오브젝트)
    public TextMeshProUGUI countText; // 수량 텍스트

    private ItemData currentItem; // 현재 담긴 아이템 정보
    private int itemCount;        // 현재 담긴 개수

    /// <summary>
    /// 슬롯에 아이템을 설정합니다. (처음 넣을 때)
    /// </summary>
    public void SetItem(ItemData item, int count = 1)
    {
        currentItem = item;
        itemCount = count;

        // UI 업데이트
        iconImage.sprite = item.icon;
        iconImage.enabled = true; // 아이콘을 켭니다.
        UpdateCountText();
    }

    

    /// <summary>
    /// 아이템 개수를 추가합니다.
    /// </summary>
    public void AddAmount(int amount)
    {
        itemCount += amount;
        UpdateCountText();
    }

    /// <summary>
    /// 아이템 개수를 줄입니다. (0 이하가 되면 슬롯을 비움)
    /// </summary>
    public void RemoveAmount(int amount)
    {
        itemCount -= amount;

        if (itemCount <= 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateCountText();
        }
    }

    /// <summary>
    /// 텍스트 UI를 갱신합니다. (1개일 땐 숫자 안 보임)
    /// </summary>
    private void UpdateCountText()
    {
        if (itemCount > 1)
            countText.text = itemCount.ToString();
        else
            countText.text = "";
    }

    public void SetSelected(bool isSelected)
    {
        // 이 스크립트가 붙어있는 오브젝트(Slot)의 Image 컴포넌트를 가져옵니다.
        Image slotBg = GetComponent<Image>();
        
        if (slotBg != null)
        {
            if (isSelected)
            {
                // 선택됨: 주황색 (RGB: 1, 0.6, 0)
                slotBg.color = new Color(1f, 0.6f, 0f); 
            }
            else
            {
                // 선택 안 됨: 흰색 (원래 색)
                slotBg.color = Color.white;
            }
        }
    }

    // --- 헬퍼 함수들 ---

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public bool IsSameItem(ItemData item)
    {
        return currentItem == item;
    }

    public bool IsFull()
    {
        // 아이템이 있고, 개수가 최대 중첩 수 이상이면 꽉 찬 것
        return currentItem != null && itemCount >= currentItem.maxStack;
    }
    
    public ItemData GetItemData()
    {
        return currentItem;
    }

    public int GetItemCount()
    {
        return itemCount;
    }
    
    /// <summary>
    /// 슬롯을 비웁니다. (배경은 남기고 내용물만 제거)
    /// </summary>
    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;

        // UI 초기화
        iconImage.sprite = null;
        iconImage.enabled = false; // 아이콘만 끕니다! (슬롯 자체는 켜져 있음)
        countText.text = "";
    }

}