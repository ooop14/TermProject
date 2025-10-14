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
    private BlockBreaker blockBreaker; // BlockBreaker 스크립트를 담을 변수

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        blockBreaker = FindFirstObjectByType<BlockBreaker>();  
    }
    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(moveInput.x * speed, rigid.linearVelocity.y);

        //방향 전환
        if (moveInput.x != 0) // 움직임이 있을 때만 방향을 바꿉니다.
        {
            spriteRenderer.flipX = moveInput.x < 0;
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

    //블럭 부수기 기능
    void OnBreak()
    {
        // blockBreaker가 있는지 확인하고, 있다면 블록 파괴 함수를 호출합니다.
        if (blockBreaker != null)
        {
            blockBreaker.BreakBlockAtMousePosition();
        }
    }


}