using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Create New Item")]
public class ItemData : ScriptableObject
{
    public string itemName;       // 아이템 이름
    public Sprite icon;           // 아이템 이미지
    public int maxStack = 99;     // 최대 중첩 개수
}
