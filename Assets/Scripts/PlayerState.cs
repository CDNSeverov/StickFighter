using UnityEngine;

public class PlayerState : MonoBehaviour
{
    enum State { 
        Idle, 
        Attacking, 
        Hitstun, 
        HoldingBack,
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

    [Header("Facing")]
    [SerializeField] Transform graphics;
    [SerializeField] Transform visor;

    State currentState = State.Idle;

    PlayerInput input;
    PlayerMovement movement;
    PlayerBlocking blocking;

    GameObject activeHitbox;

    // Back = -FacingDirection
    // Forward = +FacingDirection
    public int FacingDirection { get; private set; }
    
    [SerializeField] Transform opponent;

    void Awake() {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        blocking = GetComponent<PlayerBlocking>();
    }

    void Start() {
        FacingDirection = transform.localScale.x >= 0 ? 1 : -1;
    }


    void Update() {
        //Debug.Log($"State: {currentState}");
        UpdateFacing();

        if (currentState == State.Hitstun || currentState == State.BlockStun) {
            return;
        }

        if (currentState == State.Idle && blocking.WantsToBlock) {
            currentState = State.HoldingBack;
        } else if (currentState == State.HoldingBack && !blocking.WantsToBlock) {
            currentState = State.Idle;
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

        Invoke(nameof(EndAttack), attackDuration);

        Debug.Log($"{name} attacking");
    }
    
    private void EndAttack() {
        currentState = State.Idle;
        
        if (activeHitbox != null) {
            Destroy(activeHitbox);
        }
    }

    public void TakeHit(Vector3 hitDirection) {
        if (currentState == State.Hitstun || currentState == State.BlockStun) return;

        if (currentState == State.HoldingBack) {
            TakeBlockHit(hitDirection);
            return;
        }

        TakeNormalHit(hitDirection);
    }

    private void TakeNormalHit(Vector3 hitDirection) {
        CancelAttack();

        currentState = State.Hitstun; 

        movement.ResetVelocity();
        movement.ApplyKnockback(hitDirection.x * knockbackForce, launchForce);

        Invoke(nameof(EndHitStun), hitstunDuration);
    }

    private void TakeBlockHit(Vector3 hitDirection) {
        CancelAttack();
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

    void UpdateFacing() {
        if (opponent == null)
            return;
        
        //Debug.Log("In UpdateFacing");

        if (!movement.isGrounded)
            return;

        float diff = opponent.position.x - transform.position.x;

        if (Mathf.Abs(diff) < 0.05f)
            return;

        int newFacing = diff > 0 ? 1 : -1;

        if (newFacing == FacingDirection)
            return;

        FacingDirection = newFacing;

        FlipVisuals();
    }

    void FlipVisuals() {
        Debug.Log("In FlipVisuals");

        Vector3 gScale = graphics.localScale;
        gScale.x = Mathf.Abs(gScale.x) * FacingDirection;
        graphics.localScale = gScale;

        Vector3 vScale = visor.localScale;
        vScale.x = Mathf.Abs(vScale.x) * FacingDirection;
        visor.localScale = vScale;

        Vector3 aPos = attackSpawnPoint.localPosition;
        aPos.x = Mathf.Abs(aPos.x) * FacingDirection;
        attackSpawnPoint.localPosition = aPos;
    }

    private void CancelAttack() {
        if (activeHitbox != null) {
            Destroy(activeHitbox);
        }

        CancelInvoke(nameof(EndAttack));
    }

    /*
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
    */
}