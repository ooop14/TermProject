using UnityEngine;

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

    public void AddItem(ItemData item)
    {
        // 1. 이미 같은 아이템이 있으면 스택
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty() && slot.IsSameItem(item) && slot.CanStack())
            {
                slot.AddOne();
                return;
            }
        }

        // 2. 빈 슬롯 찾아서 추가
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item);
                return;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
    }
}
