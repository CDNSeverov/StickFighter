using UnityEngine;

public class PlayerState : MonoBehaviour
{
    enum State { 
        Idle, 
        Attack1,
        Attack2,
        Attack3,
        NSpecial,
        FSpecial,
        BSpecial,
        ASpecial,
        Hitstun, 
        KnockedDown,
        Recovery,
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

    [Header("Combo")]
    [SerializeField] float comboInputWindow = 0.2f;
    [SerializeField] float comboResetDelay = 0.5f;

    bool comboBuffered;
    bool comboLocked;

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

        if (input.AttackPressed) {
            HandleAttackInput();
        }
    }

    private void HandleAttackInput() {
        if (comboLocked) {
            return;
        }

        switch (currentState) {
            case State.Idle:
                StartAttack1();
                break;
            case State.Attack1:
            case State.Attack2:
                comboBuffered = true;
                break;
        }
    }

    private void StartAttack1() {
        currentState = State.Attack1;
        comboBuffered = false;

        SpawnHitbox();

        Invoke(nameof(OpenComboWindow), attackDuration - comboInputWindow);
        Invoke(nameof(EndAttack), attackDuration);

        Debug.Log($"{name} attack 1");
    }

    private void StartAttack2() {
        currentState = State.Attack2;
        comboBuffered = false;

        SpawnHitbox();
        
        Invoke(nameof(OpenComboWindow), attackDuration - comboInputWindow);
        Invoke(nameof(EndAttack), attackDuration);

        Debug.Log($"{name} attack 2");
    }

    private void StartAttack3() {
        currentState = State.Attack3;
        comboBuffered = false;
        comboLocked = true;

        SpawnHitbox();

        Invoke(nameof(EndAttack), attackDuration);
        Invoke(nameof(ResetComboLock), comboResetDelay);

        Debug.Log($"{name} attack 3");
    }

    private void OpenComboWindow() {
        if (!comboBuffered) {
            return;
        }

        if (currentState == State.Attack1) {
            CancelInvoke(nameof(EndAttack));
            StartAttack2();
        } else if (currentState == State.Attack2) {
            CancelInvoke(nameof(EndAttack));
            StartAttack3();
        }
    }
    
    private void EndAttack() {
        if (activeHitbox != null) {
            Destroy(activeHitbox);
        }

        if (currentState == State.Attack1 || currentState == State.Attack2) {
            currentState = State.Idle;
        } else if (currentState == State.Attack3) {
            currentState = State.Idle;
        }
    }

    private void ResetComboLock() {
        comboLocked = false;
    }
    
    private void CancelAttack() {
        if (activeHitbox != null) {
            Destroy(activeHitbox);
        }

        comboBuffered = false;
        comboLocked = false;

        CancelInvoke();
    }

    public void TakeHit(Vector3 hitDirection) {
        if (currentState == State.Hitstun || currentState == State.BlockStun) return;

        if (currentState == State.HoldingBack) {
            TakeBlockHit(hitDirection);
            return;
        }

        TakeNormalHit(hitDirection);
    }

    private void TakeSpecialHit() {
        // When player gets hit by a special (that is not a projectile) they get KnockedDown. 
        // This means they need a second to recover to idle stance, during this time they can not be hit. 
        // During the recovery process they are in the Recovery state.
        Debug.Log($"{name} got hit by special");
    }

    private void TakeProjectileHit() {
        // When player gets hit by a projectile (they are usually spawned through special attacks) they just go into HitStun.
        Debug.Log($"{name} got hit by projectile");
    }

    private void TakeNormalHit(Vector3 hitDirection) {
        CancelAttack();

        currentState = State.Hitstun; 

        movement.ResetVelocity();
        movement.ApplyKnockback(hitDirection.x * knockbackForce, launchForce);

        Invoke(nameof(EndHitStun), hitstunDuration);
        
        Debug.Log($"{name} got hit by attack");
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

    // None of this actually works. I will look into it when I start implementing animations and stuff like that.

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
        //Debug.Log("In FlipVisuals");

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