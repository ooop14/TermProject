using System.Collections;
using UnityEngine;
using UnityEngine.UI; // 1. UI (Slider)를 사용하기 위해 꼭 필요합니다!

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // 최대 체력
    public int currentHealth;   // 현재 체력

    [Header("UI 연결")]
    public Slider healthSlider; // 2. 붉은색 체력바 UI (Slider)
    
    [Header("피격 설정")]
    public float knockbackForce = 5f;    // 밀려나는 힘
    public float invincibilityDuration = 1f; // 무적 시간 (초)
    public Color hitColor = new Color(1f, 0.6f, 0.6f, 0.7f); // 피격 시 변할 색 (반투명 빨강)

    private bool isInvincible = false;   // 무적 상태인지 확인
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth; // 게임 시작 시 체력을 100%로 설정
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHealthUI();
    }

    /// <summary>
    /// 플레이어가 데미지를 입었을 때 호출할 함수
    /// </summary>
    public void TakeDamage(int damage,Transform attacker)
    {
      // 1. 무적 상태라면 데미지 무시
        if (isInvincible) return;

        // 2. 체력 감소
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // 3. 피격 효과 코루틴 실행 (넉백 + 무적 + 색변화)
        StartCoroutine(OnHitRoutine(attacker));
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        // 최대 체력을 넘지 않도록 조정
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    IEnumerator OnHitRoutine(Transform attacker)
    {
        isInvincible = true;

        // --- 1. 넉백 (밀려나기) ---
        // (내 위치 - 공격자 위치) = 밀려날 방향
        Vector2 knockbackDir = (transform.position - attacker.position).normalized;
        rb.linearVelocity = Vector2.zero; // 기존 속도 초기화 (더 확실하게 밀리기 위함)
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // --- 2. 시각적 피드백 (깜빡임) ---
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = hitColor; // 피격 색으로 변경

        // 무적 시간 동안 깜빡거림
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(invincibilityDuration / 10f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(invincibilityDuration / 10f);
        }

        // --- 3. 복구 ---
        spriteRenderer.color = originalColor; // 원래 색 복구
        isInvincible = false;
    }

    

    /// <summary>
    /// 체력 UI(Slider)를 업데이트합니다.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            // Slider의 값은 0과 1 사이이므로, (현재체력 / 최대체력) 비율로 계산합니다.
            healthSlider.value = (float)currentHealth / (float)maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        
        // ✨ GameManager에게 게임오버 처리를 요청합니다.
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
        
        // 플레이어 오브젝트를 끄거나 삭제
        gameObject.SetActive(false);
    }
}