using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private float health;
    private bool isAttacking;
    private bool isInHitStun;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private GameObject player1;
    private GameObject player2;
    
    private float lockedZ;
    private bool playerSide = false; 

    void Start() 
    {
        lockedZ = transform.position.z;

        player1 = GameObject.FindWithTag("Player1");
        player2 = GameObject.FindWithTag("Player2");
    }

    void Update()
    {
        if (health <= 0) {
            Debug.Log("MF DEAAAAAD");
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, (int)whatIsGround);

        lockZAxis();

        if (isGrounded) {
            flipPlayer();
        }

        if (!isInHitStun) {
            // can do anything
            MovementInputHandler.playerMovement(); // sort the reference problem here
            if (isGrounded) {
                // ground attacks
            } else {
                // air attacks
            }
        } else {
            // can do nothing
        }

    }

    private void lockZAxis() {
        Vector3 pos = transform.position;
        pos.z = lockedZ;
        transform.position = pos;
    }

    private void flipPlayer() {
        Vector3 p1 = player1.transform.position;
        Vector3 p2 = player2.transform.position;

        float distanceN = p1.x - p2.x;
        //Debug.Log(distanceN);

        if ((distanceN < 0 && playerSide) || (distanceN > 0 && !playerSide)) {
            Vector3 rot = transform.eulerAngles;
            rot.y += 180f;    
            transform.eulerAngles = rot;
            playerSide = !playerSide;
        }
    }
}
