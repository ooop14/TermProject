using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // 1. InputSystem 사용을 위해 추가!
using UnityEngine.Tilemaps;

public class BlockBreaker : MonoBehaviour
{
    // --- 기존 변수들 ---
    public Tilemap groundTilemap;
    public Tilemap backgroundTilemap;
    public GameObject dirtItemPrefab;
    private Camera mainCamera;

    // --- 새로 추가된 변수들 ---
    [Tooltip("블록을 부수는 데 걸리는 시간(초)")]
    public float requiredBreakTime = 1.0f; // 1초
    private float currentBreakTime = 0f;     // 현재 누르고 있는 시간 (타이머)
    private Vector3Int currentBreakingBlockPos; // 현재 부수고 있는 블록의 좌표
    private bool isBreaking = false;         // 현재 무언가를 부수고 있는지 여부

    void Start()
    {
        mainCamera = Camera.main;
    }

    // 2. 마우스 입력을 매 프레임 확인하기 위해 Update() 함수 추가
    void Update()
    {
        // 3. 마우스 왼쪽 버튼이 '눌려있는지' 확인 (새로운 Input System 방식)
        if (Mouse.current.leftButton.isPressed)
        {
            // 4. 마우스 위치의 타일 좌표 가져오기
            Vector3Int cellPosition = GetMouseCellPosition();

            // 5. 처음 클릭했거나, 마우스를 다른 블록으로 옮겼는지 확인
            if (!isBreaking || cellPosition != currentBreakingBlockPos)
            {
                // 새로운 블록을 부수기 시작 -> 타이머 리셋
                currentBreakingBlockPos = cellPosition;
                currentBreakTime = 0f;
                isBreaking = true;
            }
            else // 6. 같은 블록을 계속 누르고 있음
            {
                // 타이머 증가
                currentBreakTime += Time.deltaTime;

                // (여기에 블록 금 가는 효과를 넣으면 좋습니다)

                // 7. 시간이 다 되었는지 확인
                if (currentBreakTime >= requiredBreakTime)
                {
                    // 블록 파괴 코루틴 실행!
                    StartCoroutine(BreakBlockRoutine(currentBreakingBlockPos));
                    
                    // 타이머와 상태 리셋
                    currentBreakTime = 0f;
                    isBreaking = false;
                }
            }
        }
        else // 8. 마우스를 뗐음
        {
            // 모든 진행 상황 초기화
            if (isBreaking)
            {
                currentBreakTime = 0f;
                isBreaking = false;
                // (여기서 블록 금 간 효과를 리셋합니다)
            }
        }
    }

    // 9. 마우스 위치를 타일 좌표로 변환하는 헬퍼 함수
    private Vector3Int GetMouseCellPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0; // Z축 보정
        return groundTilemap.WorldToCell(mouseWorldPos);
    }

    // 10. 기존의 블록 파괴 코루틴 (이전과 동일)
    private IEnumerator BreakBlockRoutine(Vector3Int cellPosition)
    {
        bool createItem = false;
        Tilemap tilemapToBreak = null;

        // 앞쪽 땅(Ground)을 먼저 확인
        if (groundTilemap.GetTile(cellPosition) != null)
        {
            tilemapToBreak = groundTilemap;
            createItem = true; // 땅을 부술 때만 아이템 생성
        }
        // 만약 앞쪽 땅이 비어있다면, 뒤쪽 배경벽(BackgroundWall)을 확인
        else if (backgroundTilemap != null && backgroundTilemap.GetTile(cellPosition) != null)
        {
            tilemapToBreak = backgroundTilemap;
            createItem = true;
        }

        // 부술 타일맵을 찾았다면
        if (tilemapToBreak != null)
        {
            // 블록 파괴 (벽지 뜯기)
            tilemapToBreak.SetTile(cellPosition, null);

            // 콜라이더 새로고침
            var collider = tilemapToBreak.GetComponent<TilemapCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
                yield return null;
                collider.enabled = true;
            }

            // 아이템 생성 (필요한 경우)
            if (createItem)
            {
                Vector3 itemSpawnPos = tilemapToBreak.GetCellCenterWorld(cellPosition);
                Instantiate(dirtItemPrefab, itemSpawnPos, Quaternion.identity);
            }
        }
    }
}