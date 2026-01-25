using UnityEngine;

public class PlayerInput: MonoBehaviour
{
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode jumpButton;
    public KeyCode attack;
    public KeyCode special;

    public float Horizontal { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool SpecialPressed { get; private set; }

    void Update() {
        Horizontal = 0f;
        if (Input.GetKey(moveLeft)) Horizontal -= 1f;
        if (Input.GetKey(moveRight)) Horizontal += 1f;

        JumpPressed = Input.GetKeyDown(jumpButton);
        AttackPressed = Input.GetKeyDown(attack);
        SpecialPressed = Input.GetKeyDown(special);
    }


    /*
    public float speed = 5f;
    public float jumpHeight = 5f;
    public float jumpSpeed = 3f;
    private bool leftPressed = false;
    private bool rightPressed = false;

    private CharacterController controller;
    private float MovementX = 0f;
    private Vector3 playerVelocity;
    private float gravityValue = -11f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    */

    /*
    void Start() 
    {
        controller = GetComponent<CharacterController>();
    }

    public void playerMovement() {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, (int)whatIsGround);

        if (Input.GetKeyDown(moveLeft)) {
            leftPressed = true;
            Debug.Log("leftPressed");
        }
        if (Input.GetKeyDown(moveRight)) {
            rightPressed = true;
            Debug.Log("rightPressed");
        }

        if (isGrounded) {
            if (leftPressed) {
                MovementX = -1f;
            }
            if (rightPressed) {
                MovementX = 1f;
            }
        } 

        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        if (Input.GetKeyDown(jumpButton) && isGrounded) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        if (Input.GetKeyUp(moveLeft)) {
            leftPressed = false;
            if (rightPressed == true) {
                MovementX = 1f;
            }
        }
        if (Input.GetKeyUp(moveRight)) {
            rightPressed = false;
            if (leftPressed == true) {
                MovementX = -1f;
            }
        }
                
        if (leftPressed == false && rightPressed == false || leftPressed == true && rightPressed == true) {
            if (isGrounded) {
                MovementX = 0f;
            }
        }
    
        Vector3 movement = new Vector3(MovementX, 0f, 0f).normalized;

        if (!isGrounded) {
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move((movement * speed + playerVelocity) * Time.deltaTime);
        }
        
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move((movement * speed + playerVelocity) * Time.deltaTime);
    }
    */
}
