using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Create New Item")]
public class ItemData : ScriptableObject
{
    public string itemName;       // 아이템 이름
    public Sprite icon;           // 아이템 이미지
    public int maxStack = 99;     // 최대 중첩 개수

    public GameObject itemPrefab; // 바닥에 버렸을 때 생성될 실제 게임 오브젝트 변수입니다.
}
