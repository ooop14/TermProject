using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;

    public float gridSize = 1f; // 격자 한 칸 크기
    private void Awake()
    {
        instance = this;
    }

    public void TryPlacePlant(GameObject plantPrefab, Vector3 worldPos)
    {
        // 스냅된 위치 계산
        float x = Mathf.Round(worldPos.x / gridSize) * gridSize;
        float y = Mathf.Round(worldPos.y / gridSize) * gridSize;
        Vector3 gridPos = new Vector3(x, y, 0);

        // 이미 식물이 있는지 검사 (중복 방지)
        Collider2D hit = Physics2D.OverlapPoint(gridPos);
        if (hit == null)
        {
            Instantiate(plantPrefab, gridPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("이미 식물이 있습니다!");
        }
    }
}