using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlantCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image dragPreview;

    [SerializeField] private Image cardImage; // Inspector에 뜸

    void Start()
    {
        dragPreview = GameObject.Find("DragPreview").GetComponent<Image>();
        dragPreview.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragPreview.sprite = cardImage.sprite;
        dragPreview.SetNativeSize();
        dragPreview.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragPreview.canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        dragPreview.rectTransform.localPosition = localPoint;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        dragPreview.gameObject.SetActive(false);
    }
}