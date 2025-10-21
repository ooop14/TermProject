using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 1. 플레이어의 Transform 정보를 담을 변수
    public Transform target;
    
    // 2. 카메라가 얼마나 부드럽게 따라갈지 (값이 낮을수록 부드러움)
    public float smoothSpeed = 0.125f;

    // 3. 카메라와 플레이어 사이의 거리(Z축)
    public Vector3 offset = new Vector3(0, 0, -10);

    // Update()가 끝난 후, 즉 플레이어가 움직인 "이후"에 실행됩니다.
    // 카메라가 떨리는 현상을 방지해줍니다.
    void LateUpdate()
    {
        // 1. 목표 위치 계산 (플레이어 위치 + 오프셋)
        Vector3 desiredPosition = target.position + offset;
        
        // 2. 현재 위치에서 목표 위치로 부드럽게 이동 (Lerp 사용)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // 3. 카메라 위치를 부드럽게 이동한 위치로 설정
        transform.position = smoothedPosition;
    }
}