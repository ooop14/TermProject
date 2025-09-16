using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantSlot : MonoBehaviour
{
    public Sprite plantSprite;

    public GameObject plantObject;

    public int price;

    public Image icon;
    public TextMeshProUGUI PriceText;

    private void Oalidate()
    {

        if (plantSprite)
        {
            icon.enabled = true;
            icon.sprite = plantSprite;
            PriceText.text = price.ToString();
        }
        else
        {
            icon.enabled = false;
        }
    }
}
