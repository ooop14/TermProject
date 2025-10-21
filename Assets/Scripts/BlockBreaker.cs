using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BlockBreaker : MonoBehaviour
{
    // 1. 타일맵 변수를 두 개로 만듭니다.
    public Tilemap groundTilemap;      // 앞쪽 땅 타일맵
    public Tilemap backgroundTilemap;  // 뒤쪽 배경벽 타일맵

    public GameObject dirtItemPrefab;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void BreakBlockAtMousePosition()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int cellPosition = groundTilemap.WorldToCell(mouseWorldPos); // 기준은 groundTilemap

        // 2. 앞쪽 땅(Ground)을 먼저 확인하고 부숩니다.
        if (groundTilemap.GetTile(cellPosition) != null)
        {
            // 블록 파괴
            groundTilemap.SetTile(cellPosition, null);

            // 아이템 생성
            Vector3 itemSpawnPos = groundTilemap.GetCellCenterWorld(cellPosition);
            Instantiate(dirtItemPrefab, itemSpawnPos, Quaternion.identity);
        }
        // 3. 만약 앞쪽 땅이 비어있다면, 뒤쪽 배경벽(BackgroundWall)을 부숩니다.
        else if (backgroundTilemap != null && backgroundTilemap.GetTile(cellPosition) != null)
        {
            Debug.Log("배경벽을 파괴합니다.");
            backgroundTilemap.SetTile(cellPosition, null);
        }
    }
}