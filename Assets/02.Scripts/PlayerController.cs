using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;      // Walk 속도
    public float runSpeed = 6f;       // Run 속도
    public float rotationSpeed = 10f;
    public bool canRun = true;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [Header("Item Magnet Settings")]
    public bool useMagnetEffect = true;
    public float magnetRange = 5f;
    public float magnetStrength = 10f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;

    // 애니메이션 파라미터 해시
    private int animSpeedHash;
    private int animGroundedHash;
    private int animJumpHash;
    private int animHorizontalHash;
    private int animVerticalHash;
    private int animIsRunningHash;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animSpeedHash = Animator.StringToHash("Speed");
            animGroundedHash = Animator.StringToHash("IsGrounded");
            animJumpHash = Animator.StringToHash("Jump");
            animHorizontalHash = Animator.StringToHash("Horizontal");
            animVerticalHash = Animator.StringToHash("Vertical");
            animIsRunningHash = Animator.StringToHash("IsRunning");
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            return;

        GroundCheck();
        Move();
        Jump();
        ApplyGravity();

        if (useMagnetEffect)
        {
            ApplyMagnetEffect();
        }
    }

    void GroundCheck()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            isGrounded = controller.isGrounded;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (animator != null)
        {
            animator.SetBool(animGroundedHash, isGrounded);
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 카메라 기준 방향 계산
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;

        if (moveDirection.magnitude >= 0.1f)
        {
            // 캐릭터 회전
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            // 시프트 키로 달리기 결정
            bool isRunning = canRun && Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            // 캐릭터 이동
            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);

            // 애니메이션 파라미터 설정
            if (animator != null)
            {
                // 로컬 좌표계 기준으로 방향 변환
                Vector3 localMove = transform.InverseTransformDirection(moveDirection);

                animator.SetFloat(animHorizontalHash, localMove.x);
                animator.SetFloat(animVerticalHash, localMove.z);
                animator.SetFloat(animSpeedHash, isRunning ? 1f : 0.5f);
                animator.SetBool(animIsRunningHash, isRunning);
            }
        }
        else
        {
            // 정지 상태
            if (animator != null)
            {
                animator.SetFloat(animHorizontalHash, 0f);
                animator.SetFloat(animVerticalHash, 0f);
                animator.SetFloat(animSpeedHash, 0f);
                animator.SetBool(animIsRunningHash, false);
            }
        }
    }

    void Jump()
    {
        // 디버그 추가
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Space 키 눌림 - IsGrounded: " + isGrounded);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("점프 실행!");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger(animJumpHash);
                Debug.Log("Jump 트리거 발동");
            }
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ApplyMagnetEffect()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, magnetRange);

        foreach (Collider col in items)
        {
            if (col.CompareTag("Item"))
            {
                Vector3 direction = transform.position - col.transform.position;
                float distance = direction.magnitude;

                if (distance > 0.5f)
                {
                    col.transform.position += direction.normalized * magnetStrength * Time.deltaTime;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        if (useMagnetEffect)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, magnetRange);
        }
    }
}