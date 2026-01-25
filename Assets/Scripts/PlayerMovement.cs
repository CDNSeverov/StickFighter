using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -11f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius;
    [SerializeField] LayerMask groundLayer;

    CharacterController controller;
    Vector3 velocity;

    float externalXVelocity;
    float jumpXVelocity;

    public bool isGrounded { get; private set; }

    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
        
        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
            jumpXVelocity = 0f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 move = new Vector3(jumpXVelocity + externalXVelocity, velocity.y, 0f);
        controller.Move(move * Time.deltaTime);

        externalXVelocity = Mathf.MoveTowards(externalXVelocity, 0f, 30f * Time.deltaTime);
    }

    public void Move(float xInput) {
        if (!isGrounded) return;

        Vector3 move = new Vector3(xInput, 0, 0);
        controller.Move(move * speed * Time.deltaTime);
    }

    public void Jump(float xInput) {
        if (!isGrounded) return;
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        jumpXVelocity = xInput * speed;
    }

    public void ApplyKnockback(float xForce, float yForce) {
        externalXVelocity = xForce;
        velocity.y = yForce;
    }

    public void ResetVelocity() {
        velocity = Vector3.zero;
        externalXVelocity = 0f;
    }

    private void LateUpdate() {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
