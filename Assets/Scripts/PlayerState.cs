using UnityEngine;

public class PlayerState : MonoBehaviour
{
    enum State { 
        Idle, 
        Attacking, 
        Hitstun, 
        Blocking,
        BlockStun
    }

    [Header("Combat")]
    [SerializeField] float attackDuration = 0.4f;
    [SerializeField] float hitstunDuration = 0.5f;
    [SerializeField] float blockStunDuration = 0.15f;
    [SerializeField] float knockbackForce = 6f;
    [SerializeField] float launchForce = 10f;
    [SerializeField] GameObject attackHitboxPrefab;
    [SerializeField] Transform attackSpawnPoint;

    State currentState = State.Idle;

    PlayerInput input;
    PlayerMovement movement;

    GameObject activeHitbox;

    void Awake() {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if (currentState == State.Hitstun) {
            return;
        }

        movement.Move(input.Horizontal);

        if (input.JumpPressed) {
            movement.Jump(input.Horizontal);
        }

        if (input.AttackPressed && currentState == State.Idle) {
            StartAttack();
        }
    }

    private void StartAttack() {
        currentState = State.Attacking;

        SpawnHitbox();

        Invoke(nameof(EndAttack), 0.4f); // maybe add attackDuration

        Debug.Log($"{name} attacking");
    }
    
    private void EndAttack() {
        currentState = State.Idle;
        
        if(activeHitbox != null) {
            Destroy(activeHitbox);
        }
    }

    public void TakeHit(Vector3 hitDirection) {
        if (currentState == State.Hitstun) return;
        if (currentState == State.Blocking || currentState == State.BlockStun) {
            TakeBlockHit(hitDirection);
            return;
        }

        TakeNormalHit(hitDirection);
    }

    private void TakeNormalHit(Vector3 hitDirection) {
        currentState = State.Hitstun; 

        movement.ResetVelocity();
        
        movement.ApplyKnockback(hitDirection.x * knockbackForce, launchForce);

        Invoke(nameof(EndHitStun), hitstunDuration);
    }

    private void TakeBlockHit(Vector3 hitDirection) {
        currentState = State.BlockStun;

        movement.ResetVelocity();

        movement.ApplyKnockback(hitDirection.x * knockbackForce * 0.3f, 0f);

        Invoke(nameof(EndBlockStun), blockStunDuration);

        Debug.Log($"{name} blocked");
    }

    private void EndHitStun() {
        currentState = State.Idle;
    }

    private void EndBlockStun() {
        currentState = State.Idle;
    }

    private void SpawnHitbox() {
        activeHitbox = Instantiate(
            attackHitboxPrefab,
            attackSpawnPoint.position,
            attackSpawnPoint.rotation
        );

        activeHitbox.GetComponent<PlayerAttack>().Init(this);
    }


    /*
    [SerializeField] private float health = 100f;
    private bool isAttacking;
    private bool isInHitStun;
    private bool isGrounded;
    private bool isBlocking;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask whatIsGround;

    private GameObject player1;
    private GameObject player2;
    
    private float lockedZ;
    private bool playerSide = false; 

    private MovementInputHandler movement;

    public KeyCode attackButton;
    public KeyCode specialButton;

    [SerializeField] private GameObject attackHitboxPrefab;
    [SerializeField] private Transform attackSpawnPoint;
    [SerializeField] private float attackDuration = 0.4f;

    private GameObject activeHitbox;

    [SerializeField] private float knockbackForce = 0.3f;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float hitstunDuration = 0.5f;

    [SerializeField] private float blockStunDuration = 0.15f;

    private Rigidbody rb;

    void Start() 
    {
        movement = GetComponent<MovementInputHandler>();
        rb = GetComponent<Rigidbody>();

        lockedZ = transform.position.z;

        player1 = GameObject.FindWithTag("Player1");
        player2 = GameObject.FindWithTag("Player2");
    }

    void Update()
    {
        //if (health <= 0) {
        //    Debug.Log("MF DEAAAAAD");
        //}

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, (int)whatIsGround);

        isBlocking = isGrounded && !isAttacking && IsHoldingBack();

        lockZAxis();

        if (isGrounded) {
            flipPlayer();
        }

        if (!isInHitStun && !isAttacking) {

            movement.playerMovement(); 

            if (isGrounded) {
                // ground attacks
                if (Input.GetKeyDown(attackButton)) {
                    StartAttack();
                }
                if (Input.GetKeyDown(specialButton)) {
                    StartSpecial();
                }
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


    private void StartSpecial() {
        isAttacking = true;
        movement.enabled = false;

        // play special animation here
        SpawnHitbox();

        Invoke(nameof(EndAttack), attackDuration);
    }


    private void SpawnHitbox() {
        activeHitbox = Instantiate(
            attackHitboxPrefab,
            attackSpawnPoint.position,
            attackSpawnPoint.rotation
        );

        activeHitbox.GetComponent<PlayerAttack>().Init(this);
    }


    private void TakeNormalHit(Vector3 hitDirection) {

        isInHitStun = true;
        isAttacking = false;

        movement.enabled = false;
        rb.linearVelocity = Vector3.zero;

        Vector3 force = hitDirection.normalized * knockbackForce + Vector3.up * launchForce;

        rb.AddForce(force, ForceMode.Impulse);

        Invoke(nameof(EndHitStun), hitstunDuration);
    }

    private void TakeBlockHit(Vector3 hitDirection) {
        isInHitStun = true;
        movement.enabled = false;

        rb.linearVelocity = Vector3.zero;

        Vector3 blockForce = hitDirection.normalized * (knockbackForce * 0.3f);

        rb.AddForce(blockForce, ForceMode.Impulse);

        Invoke(nameof(EndBlockStun), blockStunDuration);

        Debug.Log($"{name} blocked");
    }

    private void EndBlockStun() {
        isInHitStun = false;
        movement.enabled = true;
    }

    private void EndHitStun() {
        isInHitStun = false;
        movement.enabled = true;
    }

    private bool IsHoldingBack() {
        if (player1 == null || player2 == null) {
            return false;
        }

        Transform opponent = gameObject == player1 ? player2.transform : player1.transform;

        float directionToOpponent = opponent.position.x - transform.position.x;

        if (directionToOpponent > 0) {
            return Input.GetAxisRaw("Horizontal") < 0;
        }

        return Input.GetAxisRaw("Horizontal") > 0;
    }
    */
}
