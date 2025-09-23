using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlantCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // [Header("UI References")] // Inspector에서 가독성을 높이기 위한 헤더
    [SerializeField] private Image cardImage; // 각 카드마다 Inspector에서 할당
    [SerializeField] private Image dragPreview; // 씬에 있는 단 하나의 미리보기 이미지를 Inspector에서 할당

    void Awake()
    {
        // 만약 Inspector에서 cardImage를 할당하는 것을 잊었다면,
        // 이 오브젝트의 자식 컴포넌트에서 자동으로 찾아오도록 시도합니다.
        if (cardImage == null)
        {
            cardImage = GetComponentInChildren<Image>();
        }

        // dragPreview가 할당되었는지 확인
        if (dragPreview == null)
        {
            Debug.LogError("DragPreview가 Inspector에 할당되지 않았습니다!", this.gameObject);
        }
    }

    void Start()
    {
        // 시작 시 미리보기는 비활성화
        if (dragPreview != null)
        {
            dragPreview.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cardImage.sprite == null || dragPreview == null) return; // 방어 코드

        // 드래그 시작 시, 이 카드의 스프라이트를 미리보기에 설정
        dragPreview.sprite = cardImage.sprite;
        dragPreview.SetNativeSize();
        dragPreview.gameObject.SetActive(true);

        // 첫 프레임 위치를 즉시 업데이트
        UpdateDragPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragPreview == null) return;
        
        // 드래그 중 위치 업데이트
        UpdateDragPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragPreview == null) return;

        // 드래그가 끝나면 미리보기 비활성화
        dragPreview.gameObject.SetActive(false);
    }

    // 위치 업데이트 로직을 별도 함수로 분리하여 가독성 및 재사용성 향상
    private void UpdateDragPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragPreview.canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        dragPreview.rectTransform.localPosition = localPoint;
    }
}