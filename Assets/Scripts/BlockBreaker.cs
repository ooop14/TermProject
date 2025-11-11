using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BlockBreaker : MonoBehaviour
{
    [Header("연결 필수 (타일맵)")]
    public Tilemap groundTilemap;
    public Tilemap backgroundTilemap;

    [Header("연결 필수 (아이템)")]
    public GameObject dirtItemPrefab;
    public GameObject woodItemPrefab;

    // ✨ --- 사거리 설정 변수 --- ✨
    [Header("사거리 설정")]
    [Tooltip("플레이어 오브젝트의 Transform을 연결하세요.")]
    public Transform playerTransform; 
    [Tooltip("블록을 부술 수 있는 최대 거리 (유닛 단위)")]
    public float breakRange = 5f; 
    // ✨ -------------------------- ✨

    [Header("파괴 설정")]
    [Tooltip("블록을 부수는 데 걸리는 시간(초)")]
    public float requiredBreakTime = 1.0f;

    [Header("파괴 애니메이션")]
    [Tooltip("파괴 효과를 보여줄 게임 오브젝트")]
    public GameObject breakIndicatorObject;
    [Tooltip("파괴 진행도에 따른 스프라이트 배열")]
    public Sprite[] breakCrackSprites;

    [Header("연쇄 파괴 설정 (나무)")]
    [Tooltip("연쇄 반응에 포함될 '모든' 나무 타일 (밑동, 줄기, 나뭇잎)")]
    public TileBase[] treeTiles;
    [Tooltip("파괴를 '시작'할 수 있는 타일 (밑동, 줄기만)")]
    public TileBase[] breakableTreeTrunks;

    // --- 비공개 변수들 ---
    private Camera mainCamera;
    private float currentBreakTime = 0f;
    private Vector3Int currentBreakingBlockPos;
    private bool isBreaking = false;
    private SpriteRenderer breakIndicatorRenderer;
    private HashSet<TileBase> treeTileSet;
    private HashSet<TileBase> breakableTrunkSet;

    #region --- 유니티 기본 함수 (Start, Update) ---

    void Start()
    {
        mainCamera = Camera.main;
        treeTileSet = new HashSet<TileBase>(treeTiles);
        breakableTrunkSet = new HashSet<TileBase>(breakableTreeTrunks);

        if (breakIndicatorObject != null)
        {
            breakIndicatorRenderer = breakIndicatorObject.GetComponent<SpriteRenderer>();
            ShowBreakIndicator(false); // 시작할 때 숨기기
        }
    }

    // Update 함수는 이 한 줄이 끝입니다.
    void Update()
    {
        HandleBreakingInput();
    }

    #endregion

    #region --- 입력 처리 함수 (로직 분리) ---

    // ✨ --- [여기가 바로 그 함수입니다!] --- ✨
    /// <summary>
    /// 마우스 입력을 받아 파괴를 시작/진행/중지할지 결정합니다.
    /// </summary>
    private void HandleBreakingInput()
    {
        // 1. 마우스를 누르고 있을 때
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3Int cellPosition = GetMouseCellPosition();

            // ✨ --- 사거리 체크 로직 --- ✨
            if (playerTransform != null) 
            {
                Vector3 cellCenterPos = groundTilemap.GetCellCenterWorld(cellPosition);
                float distance = Vector3.Distance(playerTransform.position, cellCenterPos);

                if (distance > breakRange)
                {
                    StopBreaking(); // 사거리 밖이면 파괴 중지
                    return;         // 함수 종료
                }
            }
            // ✨ -------------------------- ✨

            TileBase clickedTile = GetTileAt(cellPosition);

            // 2. 파괴를 시작할 수 있는 타일인지 확인
            if (CanStartBreaking(clickedTile))
            {
                // 3. 파괴 시작 또는 진행
                if (!isBreaking || cellPosition != currentBreakingBlockPos)
                {
                    StartBreaking(cellPosition); // 파괴 시작
                }
                else
                {
                    ContinueBreaking(); // 파괴 진행
                }
            }
            // 4. 파괴할 수 없는 타일(나뭇잎, 빈 공간)을 클릭한 경우
            else
            {
                StopBreaking(); // 파괴 중지
            }
        }
        // 5. 마우스를 뗐을 때
        else
        {
            StopBreaking(); // 파괴 중지
        }
    }

    /// <summary>
    /// 새로운 블록 파괴를 시작합니다.
    /// </summary>
    private void StartBreaking(Vector3Int cellPosition)
    {
        currentBreakingBlockPos = cellPosition;
        currentBreakTime = 0f;
        isBreaking = true;
        ShowBreakIndicator(true);
        UpdateCrackAnimation(); // 0% 상태의 첫 이미지 표시
    }

    /// <summary>
    /// 기존 블록을 계속 파괴합니다. (타이머 증가 및 파괴 완료 처리)
    /// </summary>
    private void ContinueBreaking()
    {
        currentBreakTime += Time.deltaTime;
        UpdateCrackAnimation(); // 금 가는 애니메이션 업데이트

        if (currentBreakTime >= requiredBreakTime)
        {
            StartCoroutine(BreakBlockRoutine(currentBreakingBlockPos));
            currentBreakTime = 0f; // 타이머 리셋
        }
    }

    /// <summary>
    /// 파괴를 중지하고 모든 상태를 리셋합니다. (중복 로직 통합)
    /// </summary>
    private void StopBreaking()
    {
        if (!isBreaking) return; // 이미 중지 상태면 아무것도 안 함

        currentBreakTime = 0f;
        isBreaking = false;
        ShowBreakIndicator(false);
    }

    #endregion

    #region --- 파괴 애니메이션 함수 ---

    /// <summary>
    /// 파괴 표시기(BreakIndicator)를 켜거나 끕니다.
    /// </summary>
    private void ShowBreakIndicator(bool show)
    {
        if (breakIndicatorObject == null) return;

        breakIndicatorObject.SetActive(show);

        // 켤 때는 위치도 함께 설정
        if (show)
        {
            breakIndicatorObject.transform.position = groundTilemap.GetCellCenterWorld(currentBreakingBlockPos);
        }
    }

    /// <summary>
    /// 파괴 진행도(%)에 맞춰 금 가는 스프라이트를 업데이트합니다.
    /// </summary>
    private void UpdateCrackAnimation()
    {
        if (breakIndicatorRenderer == null || breakCrackSprites.Length == 0) return;

        float progress = currentBreakTime / requiredBreakTime;
        int crackIndex = Mathf.FloorToInt(progress * breakCrackSprites.Length);
        crackIndex = Mathf.Clamp(crackIndex, 0, breakCrackSprites.Length - 1);
        
        breakIndicatorRenderer.sprite = breakCrackSprites[crackIndex];
    }

    #endregion

    #region --- 블록 파괴 코루틴 ---

    /// <summary>
    /// 파괴할 블록을 판별하고, 흙인지 나무인지에 따라 다른 코루틴을 실행합니다. (라우터 역할)
    /// </summary>
    private IEnumerator BreakBlockRoutine(Vector3Int cellPosition)
    {
        Tilemap tilemapToBreak = GetTilemapAt(cellPosition);
        if (tilemapToBreak == null) yield break;

        TileBase brokenTile = tilemapToBreak.GetTile(cellPosition);
        
        if (IsTreeTile(brokenTile))
        {
            yield return ChainBreakTreeRoutine(tilemapToBreak, cellPosition);
        }
        else
        {
            yield return BreakSingleBlockRoutine(tilemapToBreak, cellPosition);
        }
    }

    /// <summary>
    /// 단일 블록(흙)을 파괴하고 흙 아이템을 생성합니다.
    /// </summary>
    private IEnumerator BreakSingleBlockRoutine(Tilemap tilemap, Vector3Int cellPosition)
    {
        tilemap.SetTile(cellPosition, null);
        yield return RefreshCollider(tilemap); // 콜라이더 새로고침

        if (dirtItemPrefab != null)
        {
            Instantiate(dirtItemPrefab, tilemap.GetCellCenterWorld(cellPosition), Quaternion.identity);
        }
    }

    /// <summary>
    /// 나무를 연쇄 파괴하고 나무 아이템을 생성합니다.
    /// </summary>
    private IEnumerator ChainBreakTreeRoutine(Tilemap tilemap, Vector3Int startPosition)
    {
        Queue<Vector3Int> tilesToBreak = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        tilesToBreak.Enqueue(startPosition);
        visited.Add(startPosition);

        Vector3Int[] neighbors = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        while (tilesToBreak.Count > 0)
        {
            Vector3Int currentPos = tilesToBreak.Dequeue();
            TileBase currentTile = tilemap.GetTile(currentPos);
            tilemap.SetTile(currentPos, null);

            // '부술 수 있는 밑동/줄기'일 때만 나무 아이템 생성
            if (woodItemPrefab != null && IsBreakableTrunk(currentTile))
            {
                Instantiate(woodItemPrefab, tilemap.GetCellCenterWorld(currentPos), Quaternion.identity);
            }

            foreach (var offset in neighbors)
            {
                Vector3Int neighborPos = currentPos + offset;
                if (visited.Contains(neighborPos)) continue;
                if (IsTreeTile(tilemap.GetTile(neighborPos)))
                {
                    tilesToBreak.Enqueue(neighborPos);
                    visited.Add(neighborPos);
                }
            }
        }

        yield return RefreshCollider(tilemap); // 모든 나무를 부순 후, 콜라이더 1번만 새로고침
    }

    /// <summary>
    /// 타일맵 콜라이더를 껐다가 켜서 강제로 새로고침합니다. (중복 로직 통합)
    /// </summary>
    private IEnumerator RefreshCollider(Tilemap tilemap)
    {
        var collider = tilemap.GetComponent<TilemapCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            yield return null;
            collider.enabled = true;
        }
    }

    #endregion

    #region --- 헬퍼 함수 (타일 판별) ---

    private Vector3Int GetMouseCellPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;
        return groundTilemap.WorldToCell(mouseWorldPos);
    }

    private Tilemap GetTilemapAt(Vector3Int cellPosition)
    {
        if (groundTilemap.GetTile(cellPosition) != null)
            return groundTilemap;
        if (backgroundTilemap != null && backgroundTilemap.GetTile(cellPosition) != null)
            return backgroundTilemap;
        return null;
    }

    private TileBase GetTileAt(Vector3Int cellPosition)
    {
        Tilemap tilemap = GetTilemapAt(cellPosition);
        return tilemap != null ? tilemap.GetTile(cellPosition) : null;
    }

    private bool IsBreakableTrunk(TileBase tile)
    {
        return tile != null && breakableTrunkSet.Contains(tile);
    }

    private bool IsTreeTile(TileBase tile)
    {
        return tile != null && treeTileSet.Contains(tile);
    }

    private bool CanStartBreaking(TileBase tile)
    {
        if (tile == null) return false; // 빈 공간
        if (IsTreeTile(tile) && !IsBreakableTrunk(tile)) return false; // 나뭇잎
        return true; // 흙, 나무 밑동/줄기
    }

    #endregion
}