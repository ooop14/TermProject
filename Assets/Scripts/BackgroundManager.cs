using UnityEngine;

/// <summary>
/// 씬의 배경 오브젝트들을 자동으로 감지하고 패럴랙스 효과를 적용하는 매니저.
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    [Header("🎥 카메라 설정")]
    public Transform cameraTransform;  // 주로 Main Camera의 Transform

    [Header("🌄 패럴랙스 레이어 설정")]
    [Tooltip("가까운 레이어일수록 값이 1에 가까워야 함 (0 ~ 1)")]
    public float[] parallaxLayers = { 0.1f, 0.3f, 0.6f, 0.9f };

    [Header("🔍 자동 검색 옵션")]
    [Tooltip("BackgroundManager의 자식 오브젝트들을 자동으로 패럴랙스 처리할지 여부")]
    public bool autoDetectChildren = true;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (autoDetectChildren)
        {
            SetupParallaxForChildren();
        }
    }

    /// <summary>
    /// 자식 오브젝트들을 자동으로 패럴랙스 레이어에 배정합니다.
    /// </summary>
    private void SetupParallaxForChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("BackgroundManager: 자식 오브젝트가 없습니다!");
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // 기존에 ParallaxBackground가 없으면 자동 추가
            ParallaxBackground pb = child.GetComponent<ParallaxBackground>();
            if (pb == null)
                pb = child.gameObject.AddComponent<ParallaxBackground>();

            pb.cameraTransform = cameraTransform;

            // 배열 길이를 벗어나면 마지막 값으로 통일
            float effect = (i < parallaxLayers.Length) ? parallaxLayers[i] : parallaxLayers[parallaxLayers.Length - 1];
            pb.parallaxEffect = effect;

            Debug.Log($"✅ {child.name}에 ParallaxBackground 추가 (속도 {effect})");
        }
    }
}
