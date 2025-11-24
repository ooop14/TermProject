using UnityEngine;
using UnityEngine.UI; // 1. UI (Slider)를 사용하기 위해 꼭 필요합니다!

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100; // 최대 체력
    public int currentHealth;   // 현재 체력

    [Header("UI 연결")]
    public Slider healthSlider; // 2. 붉은색 체력바 UI (Slider)

    void Start()
    {
        currentHealth = maxHealth; // 게임 시작 시 체력을 100%로 설정
        UpdateHealthUI();
    }

    /// <summary>
    /// 플레이어가 데미지를 입었을 때 호출할 함수
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 3. 체력 감소
        currentHealth -= damage;
        Debug.Log("슬라임에게 데미지 입힘");
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // 4. 체력이 0 이하면 사망
        if (currentHealth <= 0)
        {
            Die();
        }
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
       Destroy(gameObject);
    }
}