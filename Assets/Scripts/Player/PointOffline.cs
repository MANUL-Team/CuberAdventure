using UnityEngine;

public class PointOffline : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isGrounded;
    private Rigidbody2D rb;
    private bool hasDrive;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            return;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == "Drive")
            {
                hasDrive = true;
                break;
            }
        }
    }
    void Update()
    {
        if (rb == null || animator == null)
            return;
        float vertical = rb.linearVelocity.y;
        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.4f;
        bool rising = vertical > 1.5f;
        if (isGrounded && !rising)
        {
            animator.SetBool("JumpUp", false);
            animator.SetBool("JumpDown", false);
            if (hasDrive)
                animator.SetBool("Drive", moving);
            return;
        }
        if (hasDrive)
            animator.SetBool("Drive", false);
        bool falling = vertical < 0f;
        animator.SetBool("JumpUp", !falling);
        animator.SetBool("JumpDown", falling);
    }
    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    }
}
