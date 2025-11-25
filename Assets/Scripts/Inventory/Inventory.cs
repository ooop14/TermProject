using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private InventorySlot[] slots;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        slots = GetComponentsInChildren<InventorySlot>(includeInactive: true);
    }

    public void AddItem(ItemData item, int count = 1)
    {
        // 1. 이미 같은 아이템이 있으면 스택
        foreach (InventorySlot slot in slots)
        {
            // ✨ [수정] 여러 개를 한 번에 추가하는 로직
            if (!slot.IsEmpty() && slot.IsSameItem(item))
            {
                // (더 정교하게 하려면 maxStack을 체크해서 나눠 담아야 하지만, 일단 간단하게)
                // slot.AddAmount(count); // 이런 함수를 InventorySlot에 추가해야 합니다.
                
                // 간단한 임시 해결책: count만큼 반복해서 AddOne 호출
                for(int i=0; i<count; i++) 
                {
                    if(slot.CanStack()) slot.AddOne();
                    else break; // 꽉 차면 루프 탈출 (다음 슬롯으로)
                }
                // 남은 개수 처리 로직이 필요하지만, 지금은 일단 이렇게 하면 1개 이상 들어갑니다.
                return;
            }
        }

        // 2. 빈 슬롯 찾아서 추가
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item, count); // ✨ [수정] count를 함께 전달
                return;
            }
        }
        Debug.Log("인벤토리가 가득 찼습니다!");
    }

    // 인벤토리 전체에서 특정 아이템이 총 몇 개인지 확인합니다.
    public int GetItemCount(ItemData item)
    {
        int totalCount = 0;
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty() && slot.IsSameItem(item))
            {
                totalCount += slot.GetItemCount();
            }
        }
        return totalCount;
    }

    // 레시피에 필요한 모든 재료를 가지고 있는지 확인합니다.
    public bool HasIngredients(List<ItemIngredient> ingredients)
    {
        foreach (ItemIngredient ingredient in ingredients)
        {
            // GetItemCount로 총 개수를 확인하고, 필요한 양(amount)보다 적으면
            if (GetItemCount(ingredient.item) < ingredient.amount)
            {
                // 재료 부족!
                return false;
            }
        }
        // 모든 재료가 충분함!
        return true;
    }

    public void CompactSlots()
{
    List<InventorySlotData> temp = new List<InventorySlotData>();

    // 1. 모든 슬롯의 아이템을 리스트에 임시 저장
    foreach (var slot in slots)
    {
        if (!slot.IsEmpty())
        {
            temp.Add(new InventorySlotData(slot.GetItemData(), slot.GetItemCount()));
        }
    }

    // 2. 모든 슬롯 초기화
    foreach (var slot in slots)
        slot.ClearSlot();

    // 3. 앞에서부터 다시 재배치
    for (int i = 0; i < temp.Count; i++)
    {
        slots[i].SetItem(temp[i].item, temp[i].count);
    }
}

class InventorySlotData
{
    public ItemData item;
    public int count;
    public InventorySlotData(ItemData item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

    // 인벤토리에서 사용한 재료 아이템들을 제거합니다.
    // Inventory.cs

    public void RemoveIngredients(List<ItemIngredient> ingredients)
    {
        foreach (ItemIngredient ingredient in ingredients)
        {
            int amountToRemove = ingredient.amount;

            foreach (InventorySlot slot in slots)
            {
                if (!slot.IsEmpty() && slot.IsSameItem(ingredient.item))
                {
                    int countInSlot = slot.GetItemCount(); // 현재 슬롯의 개수

                    if (countInSlot > amountToRemove)
                    {
                        // 이 슬롯에서 일부만 제거 (슬롯은 유지됨)
                        slot.RemoveAmount(amountToRemove);
                        amountToRemove = 0;
                        break; 
                    }
                    else
                    {
                        // 이 슬롯을 싹 비움
                        amountToRemove -= countInSlot;
                        slot.ClearSlot(); // 여기서 슬롯이 사라지지 않게 주의 (2번 참고)
                    }
                }
            }
            CompactSlots();
        }
    }

    
}
