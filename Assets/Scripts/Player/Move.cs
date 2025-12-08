using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    Rigidbody2D rigid;
    Vector2 moveInput;
    Animator animator;
    SpriteRenderer spriteRenderer;
    public float speed = 5f;
    public float jumpPower = 10f; // 점프 힘 값을 인스펙터에서 조절할 수 있도록 public으로
    bool isGrounded; // 땅에 닿았는지 확인하는 값
    
    [Header("공격 설정")]
    public Transform attackPoint; // 플레이어 앞에 있는 빈 오브젝트
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;    // 'Enemy' 레이어
    public int attackDamage = 25;

    public float attackRate = 0.5f; // 0.5초에 한 번만 공격 가능
    private float nextAttackTime = 0f; // 다음 공격이 가능한 시간

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    
    //움직임 기능
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // 점프 기능
    void OnJump()
    {
        // 땅에 있을 때만 점프가 가능하도록 조건을 추가합니다.
        if (isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("Jump", true);
        }
    }

   // Move.cs

    void OnAttack()
    {
        if (Time.time >= nextAttackTime){
            animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach(Collider2D enemy in hitEnemies)
            {
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    // ✨ [수정] damage와 함께 'transform(플레이어)'을 넘겨줍니다.
                    health.TakeDamage(attackDamage, transform);
                }
            }
            nextAttackTime = Time.time + attackRate;
        }
    }
void OnCraft(InputValue value)
    {
        // CraftingManager에게 "창 열어/닫아!" 명령
        if (CraftingManager.instance != null)
        {
            CraftingManager.instance.ToggleCraftingUI();
        }
    }
    
    // 'Is Trigger'가 켜진 콜라이더와 부딪혔을 때 자동으로 호출되는 함수
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();

        if (pickup != null)
        {
            Inventory.instance.AddItem(pickup.itemData);
            Debug.Log($"[획득] {pickup.itemData.itemName}");
        }

        Destroy(other.gameObject);
    }
    }


    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(moveInput.x * speed, rigid.linearVelocity.y);

        //방향 전환
        if (moveInput.x != 0) // 움직임이 있을 때만 방향을 바꿉니다.
        {
            // 1. 현재 캐릭터의 원래 크기(절댓값)를 가져옵니다. (예: 4)
            float originalScaleX = Mathf.Abs(transform.localScale.x);
            float originalScaleY = transform.localScale.y;
            float originalScaleZ = transform.localScale.z;

            // 2. 방향(Sign)만 바꾸고, 크기는 원래 값(originalScaleX)을 곱해서 유지합니다.
            // 오른쪽이면 (4, 4, 1), 왼쪽이면 (-4, 4, 1)이 됩니다.
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x) * originalScaleX, originalScaleY, originalScaleZ);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2.5f, LayerMask.GetMask("Platform"));

        // Raycast가 땅에 닿았으면 isGrounded는 true, 아니면 false가 됩니다.
        isGrounded = hit.collider != null;

        //걷기/대기 애니메이션 전환
        if (Mathf.Abs(rigid.linearVelocity.x) == 0)
            animator.SetBool("isRun", false);
        else
            animator.SetBool("isRun", true);

        //착지가 감지되면 jump 애니메이션을 false로
        if (Mathf.Abs(rigid.linearVelocity.y) < 0.2f)
        {
            animator.SetBool("Jump", false);
        }

    }



}