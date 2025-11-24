using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance; // 싱글톤

    [Header("UI 연결")]
    public GameObject craftingPanel;      // 켜고 끌 패널 (CraftingPanel)
    public Transform recipeContent;       // 버튼이 들어갈 부모 (Scroll View -> Content)
    public GameObject recipeButtonPrefab; // 버튼 프리팹

    [Header("데이터")]
    public List<CraftingRecipe> allRecipes; // 모든 레시피 목록

    private bool isOpen = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        craftingPanel.SetActive(false); // 시작할 땐 숨김
        UpdateRecipeUI(); // 미리 목록을 만들어둡니다.
    }

    // E키를 누르면 호출될 함수
    public void ToggleCraftingUI()
    {
        isOpen = !isOpen;
        craftingPanel.SetActive(isOpen);

        if (isOpen)
        {
            UpdateRecipeUI(); // 열 때마다 목록 갱신 (재료 확인 등)
        }
    }

    void UpdateRecipeUI()
    {
        // 기존 버튼들 싹 지우고 (초기화)
        foreach (Transform child in recipeContent)
        {
            Destroy(child.gameObject);
        }

        // 레시피 개수만큼 버튼 생성
        foreach (CraftingRecipe recipe in allRecipes)
        {
            GameObject btnObj = Instantiate(recipeButtonPrefab, recipeContent);
            
            // (심화) 여기서 버튼의 아이콘/텍스트를 recipe 정보로 바꿔주는 코드가 필요합니다.
            Image iconImage = btnObj.transform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = recipe.resultItem.icon;
            
            // 버튼 클릭 기능 연결
            btnObj.GetComponent<Button>().onClick.AddListener(() => TryCraft(recipe));
        }
    }

    void TryCraft(CraftingRecipe recipe)
    {
        if (Inventory.instance.HasIngredients(recipe.ingredients))
        {
            Inventory.instance.RemoveIngredients(recipe.ingredients);
            
            // ✨ [수정] resultAmount를 함께 전달합니다!
            Inventory.instance.AddItem(recipe.resultItem, recipe.resultAmount);
            
            Debug.Log(recipe.resultItem.itemName + " 제작 성공!");
        }
        else
        {
            Debug.Log("재료가 부족합니다.");
        }
    }
}