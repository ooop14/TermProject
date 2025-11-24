using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 꼭 필요합니다!

// ItemData처럼, Assets 메뉴에서 '레시피'를 만들 수 있게 해줍니다.
[CreateAssetMenu(fileName = "NewRecipe", menuName = "Item/Create New Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("조합에 필요한 재료")]
    public List<ItemIngredient> ingredients;

    [Header("조합 결과물")]
    public ItemData resultItem;
    public int resultAmount = 1; // (예: 횃불 4개)
}

// ✨ 레시피의 '재료' 한 줄을 정의하는 도우미 클래스입니다.
// [System.Serializable]이 있어야 인스펙터 창에 보입니다.
[System.Serializable]
public class ItemIngredient
{
    public ItemData item; // 필요한 아이템 (예: 흙)
    public int amount;    // 필요한 개수 (예: 5개)
}