using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("설정")]
    public GameObject monsterPrefab; // 소환할 몬스터 프리팹 (Slime)
    public Transform player;         // 플레이어 위치
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 7f;
    
    [Header("범위")]
    public float minSpawnDistance = 10f; // 플레이어로부터 최소 거리 (화면 밖)
    public float maxSpawnDistance = 20f; // 최대 거리

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float randomTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            // 주기만큼 기다림
            yield return new WaitForSeconds(randomTime);

            if (player != null && monsterPrefab != null)
            {
                SpawnMonster();
            }
        }
    }

    void SpawnMonster()
    {
        // 1. 플레이어 기준으로 왼쪽(-1) 또는 오른쪽(1) 랜덤 결정
        float direction = Random.value > 0.5f ? 1f : -1f;
        
        // 2. 최소~최대 거리 사이의 랜덤 거리 계산
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        // 3. 스폰 위치 계산 (X축만)
        Vector3 spawnPos = player.position;
        spawnPos.x += direction * distance;
        
        // 4. Y축 위치 잡기 (가장 중요!)
        // 하늘 높은 곳(Y=10)에서 아래로 레이저를 쏴서 땅을 찾습니다.
        // 땅 위에 스폰해야 하니까요.
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(spawnPos.y, 2), Vector2.down, 20f, LayerMask.GetMask("Platform"));

        if (hit.collider != null)
        {
            // 땅을 찾았으면 그 위에 스폰 (슬라임 높이 고려해서 +1)
            spawnPos.y = hit.point.y + 1f;
            Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        }
    }
}