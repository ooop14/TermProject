using UnityEngine;

[ExecuteAlways]
public class ParallaxBackground : MonoBehaviour
{
    [Header("🎥 기본 설정")]
    [Tooltip("카메라(또는 플레이어)의 Transform")]
    public Transform cameraTransform;

    [Tooltip("패럴랙스 이동 속도 (0~1, 0에 가까울수록 느림)")]
    [Range(0f, 1f)] public float parallaxEffect = 0.5f;

    [Tooltip("X축 패럴랙스만 적용 (기본 true)")]
    public bool horizontalOnly = true;

    [Header("🌌 지하 연출 설정")]
    [Tooltip("이 높이보다 아래로 내려가면 배경이 Y축으로 천천히 따라옵니다.")]
    public float undergroundThresholdY = -10f;

    [Tooltip("지하에서 Y축 패럴랙스 이동 속도 비율 (기본 0.2)")]
    [Range(0f, 1f)] public float undergroundVerticalEffect = 0.2f;

    [Tooltip("지하에서 어두워질 정도 (0=밝음, 1=완전암흑)")]
    [Range(0f, 1f)] public float undergroundDarkness = 0.5f;

    [Tooltip("지하 진입 시 어두워질 배경의 SpriteRenderer (선택)")]
    public SpriteRenderer backgroundRenderer;

    private Vector3 lastCameraPosition;
    private Color originalColor;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPosition = cameraTransform.position;

        if (backgroundRenderer != null)
            originalColor = backgroundRenderer.color;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        float moveX = deltaMovement.x * parallaxEffect;
        float moveY = 0f;

        // ⚙️ Y축 이동 처리 (지하에서만)
        if (!horizontalOnly)
        {
            if (cameraTransform.position.y < undergroundThresholdY)
            {
                float depthFactor = Mathf.InverseLerp(undergroundThresholdY - 10f, undergroundThresholdY, cameraTransform.position.y);
                moveY = deltaMovement.y * undergroundVerticalEffect * depthFactor;
            }
        }

        transform.position += new Vector3(moveX, moveY, 0);
        lastCameraPosition = cameraTransform.position;

        // 🌒 배경 어두워짐 효과
        if (backgroundRenderer != null)
        {
            float darknessFactor = 0f;

            if (cameraTransform.position.y < undergroundThresholdY)
            {
                darknessFactor = Mathf.InverseLerp(undergroundThresholdY, undergroundThresholdY - 10f, cameraTransform.position.y);
            }

            Color c = originalColor;
            c.a = Mathf.Lerp(originalColor.a, originalColor.a * (1f - undergroundDarkness), darknessFactor);
            backgroundRenderer.color = c;
        }

    }
}
