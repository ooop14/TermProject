using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BlockBreaker : MonoBehaviour
{
    // --- 타일맵 변수 ---
    public Tilemap groundTilemap;
    public Tilemap backgroundTilemap;
    
    [Header("아이템 프리팹")]
    public GameObject dirtItemPrefab;   // 흙 아이템
    public GameObject woodItemPrefab;   // 나무 아이템
    
    private Camera mainCamera;

    // --- 파괴 시간 변수 ---
    [Tooltip("블록을 부수는 데 걸리는 시간(초)")]
    public float requiredBreakTime = 1.0f;
    private float currentBreakTime = 0f;
    private Vector3Int currentBreakingBlockPos;
    private bool isBreaking = false;

    // --- 파괴 애니메이션 변수 ---
    [Header("파괴 애니메이션")]
    public GameObject breakIndicatorObject;
    public Sprite[] breakCrackSprites;
    private SpriteRenderer breakIndicatorRenderer;

    // --- 연쇄 파괴 변수 ---
    [Header("연쇄 파괴 설정")]
    public TileBase[] treeTiles;
    public TileBase[] breakableTreeTrunks;

    private HashSet<TileBase> treeTileSet;
    private HashSet<TileBase> breakableTrunkSet;

    // --- Start, Helper Functions, Update (기존과 동일) ---

    void Start()
    {
        mainCamera = Camera.main;
        treeTileSet = new HashSet<TileBase>(treeTiles);
        breakableTrunkSet = new HashSet<TileBase>(breakableTreeTrunks);
        if (breakIndicatorObject != null)
        {
            breakIndicatorRenderer = breakIndicatorObject.GetComponent<SpriteRenderer>();
            breakIndicatorObject.SetActive(false);
        }
    }

    private bool IsBreakableTrunk(TileBase tile)
    {
        return tile != null && breakableTrunkSet.Contains(tile);
    }
    private bool IsTreeTile(TileBase tile)
    {
        return tile != null && treeTileSet.Contains(tile);
    }
    private Vector3Int GetMouseCellPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;
        return groundTilemap.WorldToCell(mouseWorldPos);
    }

    void Update()
    {
        // (Update 함수 내용은 기존과 완전히 동일합니다)
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3Int cellPosition = GetMouseCellPosition();
            TileBase clickedTile = groundTilemap.GetTile(cellPosition);
            if (clickedTile == null && backgroundTilemap != null)
            {
                clickedTile = backgroundTilemap.GetTile(cellPosition);
            }

            bool canStartBreaking = false;
            if (clickedTile != null)
            {
                if (IsTreeTile(clickedTile))
                {
                    if (IsBreakableTrunk(clickedTile))
                        canStartBreaking = true;
                }
                else
                    canStartBreaking = true;
            }

            if (!canStartBreaking)
            {
                if (isBreaking)
                {
                    currentBreakTime = 0f;
                    isBreaking = false;
                    if (breakIndicatorObject != null)
                        breakIndicatorObject.SetActive(false);
                }
                return;
            }
            
            if (!isBreaking || cellPosition != currentBreakingBlockPos)
            {
                currentBreakingBlockPos = cellPosition;
                currentBreakTime = 0f;
                isBreaking = true;
                if (breakIndicatorObject != null)
                {
                    breakIndicatorObject.SetActive(true);
                    breakIndicatorObject.transform.position = groundTilemap.GetCellCenterWorld(cellPosition);
                    if (breakCrackSprites.Length > 0)
                        breakIndicatorRenderer.sprite = breakCrackSprites[0];
                }
            }
            else
            {
                currentBreakTime += Time.deltaTime;
                if (breakIndicatorRenderer != null && breakCrackSprites.Length > 0)
                {
                    float progress = currentBreakTime / requiredBreakTime;
                    int crackIndex = Mathf.FloorToInt(progress * breakCrackSprites.Length);
                    crackIndex = Mathf.Clamp(crackIndex, 0, breakCrackSprites.Length - 1);
                    breakIndicatorRenderer.sprite = breakCrackSprites[crackIndex];
                }
                
                if (currentBreakTime >= requiredBreakTime)
                {
                    StartCoroutine(BreakBlockRoutine(currentBreakingBlockPos));
                    currentBreakTime = 0f;
                }
            }
        }
        else
        {
            if (isBreaking)
            {
                currentBreakTime = 0f;
                isBreaking = false;
                if (breakIndicatorObject != null)
                    breakIndicatorObject.SetActive(false);
            }
        }
    }

    // --- 블록 파괴 로직 (기존과 동일) ---
    private IEnumerator BreakBlockRoutine(Vector3Int cellPosition)
    {
        Tilemap tilemapToBreak = null;
        bool createItem = true; // (이제 createItem은 항상 true입니다)

        if (groundTilemap.GetTile(cellPosition) != null)
            tilemapToBreak = groundTilemap;
        else if (backgroundTilemap != null && backgroundTilemap.GetTile(cellPosition) != null)
            tilemapToBreak = backgroundTilemap;

        if (tilemapToBreak == null) yield break;

        TileBase brokenTile = tilemapToBreak.GetTile(cellPosition);
        if (IsTreeTile(brokenTile))
        {
            yield return ChainBreakTreeRoutine(tilemapToBreak, cellPosition, createItem);
        }
        else
        {
            yield return BreakSingleBlockRoutine(tilemapToBreak, cellPosition, createItem);
        }
    }

    // --- 단일 블록 파괴 (흙 아이템 드랍) ---
    private IEnumerator BreakSingleBlockRoutine(Tilemap tilemap, Vector3Int cellPosition, bool createItem)
    {
        tilemap.SetTile(cellPosition, null);

        var collider = tilemap.GetComponent<TilemapCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            yield return null;
            collider.enabled = true;
        }

        if (createItem)
        {
            Vector3 itemSpawnPos = tilemap.GetCellCenterWorld(cellPosition);
            // 흙 블록은 dirtItemPrefab을 생성
            Instantiate(dirtItemPrefab, itemSpawnPos, Quaternion.identity);
        }
    }

    // --- ✨ [수정됨!] 나무 연쇄 파괴 (나무 아이템 드랍) ---
    private IEnumerator ChainBreakTreeRoutine(Tilemap tilemap, Vector3Int startPosition, bool createItem)
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

            // [수정] 아이템 생성 로직: 부순 타일이 '밑동/줄기'일 때만 아이템 생성
            if (createItem && IsBreakableTrunk(currentTile)) 
            {
                Vector3 itemSpawnPos = tilemap.GetCellCenterWorld(currentPos);

                // 나무 블록은 woodItemPrefab을 생성
                Instantiate(woodItemPrefab, itemSpawnPos, Quaternion.identity); 
            }

            foreach (var offset in neighbors)
            {
                Vector3Int neighborPos = currentPos + offset;
                if (visited.Contains(neighborPos)) continue;
                TileBase neighborTile = tilemap.GetTile(neighborPos);
                
                if (IsTreeTile(neighborTile))
                {
                    tilesToBreak.Enqueue(neighborPos);
                    visited.Add(neighborPos);
                }
            }
        }

        // 콜라이더 새로고침
        var collider = tilemap.GetComponent<TilemapCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            yield return null;
            collider.enabled = true;
        }
    }
}