using UnityEngine;
using System.Collections.Generic; // List 사용을 위해 필수!
using UnityEngine.InputSystem;// 키보드 입력을 위해 추가
using System; 

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // 싱글톤
    private InventorySlot[] slots;
    public SpriteRenderer handRenderer;
    private int selectedSlotIndex = 0; // 현재 선택된 슬롯 번호 (0 ~ 7)

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 자식에 있는 모든 InventorySlot 컴포넌트를 가져옵니다.
        slots = GetComponentsInChildren<InventorySlot>(includeInactive: true);
        SelectSlot(0);
    }

    void Update()
    {
        // ✨ --- [추가] 숫자키 1~8로 슬롯 선택 --- ✨
        // (New Input System에서도 Keyboard.current는 바로 쓸 수 있습니다)
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectSlot(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectSlot(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SelectSlot(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SelectSlot(7);
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        selectedSlotIndex = index;

        // ✨ --- [여기가 수정된 부분입니다!] --- ✨
        // 모든 슬롯을 하나씩 확인합니다.
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == index)
            {
                // 선택된 번호의 슬롯은 주황색으로!
                slots[i].SetSelected(true);
            }
            else
            {
                // 나머지는 모두 흰색으로!
                slots[i].SetSelected(false);
            }
        }
        // ✨ ---------------------------------- ✨

        UpdateHandVisual();
    }

    // ✨ --- [추가] 손에 든 아이템 이미지 업데이트 --- ✨
    public void UpdateHandVisual()
    {
        if (handRenderer == null) return;

        InventorySlot currentSlot = slots[selectedSlotIndex];
        ItemData item = currentSlot.GetItemData();

        if (item != null)
        {
            handRenderer.sprite = item.icon; // 아이템이 있으면 그 이미지를 보여줌
            handRenderer.enabled = true;
        }
        else
        {
            handRenderer.sprite = null;      // 없으면 숨김
            handRenderer.enabled = false;
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가합니다. (개수 지정 가능)
    /// </summary>
    public void AddItem(ItemData item, int count = 1)
    {
        int remainingAmount = count;

        // 1. 이미 같은 아이템이 있는 슬롯에 쌓기 (Stack)
        foreach (InventorySlot slot in slots)
        {
            if (remainingAmount <= 0) break; // 다 넣었으면 종료

            // 같은 아이템이 있고, 아직 꽉 차지 않은 슬롯을 찾음
            if (!slot.IsEmpty() && slot.IsSameItem(item) && !slot.IsFull())
            {
                int currentCount = slot.GetItemCount();
                int spaceLeft = item.maxStack - currentCount; // 더 넣을 수 있는 공간

                int amountToAdd = Mathf.Min(remainingAmount, spaceLeft); // 넣을 양 결정

                slot.AddAmount(amountToAdd);
                remainingAmount -= amountToAdd;
            }
        }

        // 2. 남은 아이템을 빈 슬롯에 넣기
        if (remainingAmount > 0)
        {
            foreach (InventorySlot slot in slots)
            {
                if (remainingAmount <= 0) break;

                if (slot.IsEmpty())
                {
                    int amountToAdd = Mathf.Min(remainingAmount, item.maxStack);
                    
                    slot.SetItem(item, amountToAdd);
                    remainingAmount -= amountToAdd;
                }
            }
        }

        if (remainingAmount > 0)
        {
            Debug.Log($"인벤토리가 꽉 차서 아이템 {remainingAmount}개를 줍지 못했습니다.");
            // (여기서 남은 아이템을 바닥에 다시 떨어뜨리는 로직을 추가할 수도 있습니다)
        }
    }

    // --- 제작(Crafting)을 위한 함수들 ---

    /// <summary>
    /// 특정 아이템을 총 몇 개 가지고 있는지 확인
    /// </summary>
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

    /// <summary>
    /// 레시피 재료가 충분한지 확인
    /// </summary>
    public bool HasIngredients(List<ItemIngredient> ingredients)
    {
        foreach (ItemIngredient ingredient in ingredients)
        {
            if (GetItemCount(ingredient.item) < ingredient.amount)
            {
                return false; // 재료 부족
            }
        }
        return true;
    }

    /// <summary>
    /// 재료 아이템들을 인벤토리에서 제거 (제작 시 사용)
    /// </summary>
    public void RemoveIngredients(List<ItemIngredient> ingredients)
    {
        foreach (ItemIngredient ingredient in ingredients)
        {
            int amountToRemove = ingredient.amount;

            foreach (InventorySlot slot in slots)
            {
                if (amountToRemove <= 0) break;

                if (!slot.IsEmpty() && slot.IsSameItem(ingredient.item))
                {
                    int countInSlot = slot.GetItemCount();

                    if (countInSlot > amountToRemove)
                    {
                        // 이 슬롯에서 필요한 만큼만 뺌
                        slot.RemoveAmount(amountToRemove);
                        amountToRemove = 0;
                    }
                    else
                    {
                        // 이 슬롯을 싹 비우고, 남은 양은 다음 슬롯에서 뺌
                        amountToRemove -= countInSlot;
                        slot.ClearSlot();
                    }
                }
            }
        }
    }
}