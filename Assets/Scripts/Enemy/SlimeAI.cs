using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    [Header("능력치")]
    public int damage = 10;
    public float jumpForce = 5f;  // 수직 점프 힘
    public float moveSpeed = 3f;  // 점프 시 수평 이동 힘

    [Header("AI 설정")]
    public float detectionRadius = 8f; // 플레이어를 감지할 반경
    public LayerMask playerLayer;      // "Player" 레이어
    public float patrolJumpInterval = 3f; // 순찰 시 점프 주기 (초)
    public float chaseJumpInterval = 1f;  // 추격 시 점프 주기 (초)

    [Header("땅 감지 (필수!)")]
    public Transform groundCheck;   // 'GroundCheck' 자식 오브젝트
    public LayerMask groundLayer;   // 땅("Platform") 레이어
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float jumpTimer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpTimer = patrolJumpInterval; // 첫 점프는 순찰 주기로
    }

    // 모든 로직을 FixedUpdate (물리 업데이트)에서 처리합니다.
    void FixedUpdate()
    {
        // 1. 땅에 닿았는지 확인
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. 타이머 시간 감소
        jumpTimer -= Time.fixedDeltaTime;

        // 3. 땅에 있고, 점프할 시간이 되었을 때만 점프!
        if (isGrounded && jumpTimer <= 0)
        {
            // 4. 플레이어 감지
            Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            
            float direction;

            if (player != null) // 플레이어 발견! (추격)
            {
                // 플레이어 방향으로 점프
                direction = (player.transform.position.x - transform.position.x) > 0 ? 1 : -1;
                jumpTimer = chaseJumpInterval; // 다음 점프는 빠르게 (추격용)
            }
            else // 플레이어 없음 (순찰)
            {
                // 랜덤 방향으로 점프
                direction = (Random.value > 0.5f) ? 1 : -1;
                jumpTimer = patrolJumpInterval; // 다음 점프는 느리게 (순찰용)
            }

            // 5. 계산된 방향으로 점프 실행
            rb.AddForce(new Vector2(direction * moveSpeed, jumpForce), ForceMode2D.Impulse);
            spriteRenderer.flipX = (direction == -1); // 방향에 맞게 스프라이트 뒤집기
        }
    }
    
    // 6. 충돌 로직 (플레이어 데미지 - 기존과 동일)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}