using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    [Header("피격 설정")]
    public float knockbackForce = 3f;
    public Color hitColor = Color.red;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 만약 같은 오브젝트에 없으면 자식들 중에서라도 찾아봅니다.
        if (spriteRenderer == null) 
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 슬라이더 초기화
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        currentHealth -= damage;
        
        // UI 업데이트 (안전장치 추가)
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(OnHitRoutine(attacker));
        }
    }

    IEnumerator OnHitRoutine(Transform attacker)
    {
        // 1. 넉백 (Rigidbody가 있을 때만)
        if (rb != null && attacker != null)
        {
            Vector2 knockbackDir = (transform.position - attacker.position).normalized;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        // 2. 빨간색 번쩍임 (SpriteRenderer가 있을 때만)
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
        else
        {
            // 렌더러가 없으면 그냥 시간만 끕니다 (오류 방지)
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        
        // (선택 사항) 아이템 드랍 로직 등을 여기에 추가
    }
}