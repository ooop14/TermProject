using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    public float speed = 5f;
    public float jumpPower = 10f; // 점프 힘 값을 인스펙터에서 조절할 수 있도록 public으로
    Rigidbody2D rigid;
    Vector2 moveInput;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // 'Jump' 액션이 감지되면 Player Input 컴포넌트가 이 함수를 호출합니다.
    void OnJump()
    {
        // 기존 Update 함수에 있던 점프 코드를 이곳으로 옮깁니다.
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        // velocity와 linearVelocity는 같은 기능입니다. 둘 다 사용 가능합니다.
        rigid.linearVelocity = new Vector2(moveInput.x * speed, rigid.linearVelocity.y);
    }
}