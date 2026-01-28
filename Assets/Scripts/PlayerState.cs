using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum State { 
        Idle, 
        Attack1,
        Attack2,
        Attack3,
        AttackA,
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
    [SerializeField] float knockbackForce = 6f;
    [SerializeField] float launchForce = 10f;
    [SerializeField] GameObject attackHitboxPrefab;
    [SerializeField] Transform attackSpawnPoint;

    [Header("Facing")]
    [SerializeField] Transform graphics;
    [SerializeField] Transform visor;
    [SerializeField] Transform opponent;

    [Header("Combo")]
    [SerializeField] float comboInputWindow = 0.2f;
    [SerializeField] float comboResetDelay = 0.5f;

    bool comboBuffered;
    bool comboLocked;

    // Back = -FacingDirection
    // Forward = +FacingDirection
    public int FacingDirection { get; private set; }
    public State currentState { get; private set; } = State.Idle;

    PlayerInput input;
    PlayerMovement movement;
    PlayerBlocking blocking;
    Character character;
    Animator animator;

    GameObject activeHitbox;

    void Awake() {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        blocking = GetComponent<PlayerBlocking>();
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
    }

    void Start() {
        FacingDirection = transform.localScale.x >= 0 ? 1 : -1;
    }


    void Update() {
        UpdateFacing();
        animator.SetInteger("State", (int)currentState);

        if (currentState == State.Hitstun || currentState == State.BlockStun)
            return;

        HandleBlocking();
        HandleMovement();
        HandleInputs();
    }

    void HandleInputs() {
        if (input.AttackPressed)
            HandleAttackInput();

        if (input.SpecialPressed)
            HandleSpecialInput();
    }

    void HandleMovement() {
        movement.Move(input.Horizontal);

        if (input.JumpPressed)
            movement.Jump(input.Horizontal);
    }

    void HandleBlocking() {
        if (currentState == State.Idle && blocking.WantsToBlock)
            SetState(State.HoldingBack);
        else if (currentState == State.HoldingBack && !blocking.WantsToBlock)
            SetState(State.Idle);
    }

    void HandleSpecialInput() {
        if (currentState != State.Idle && currentState != State.HoldingBack)
            return;

        bool airborne = !movement.isGrounded;

        if (airborne) {
            SetState(State.ASpecial);
            character.AirSpecial();
        }
        else if (blocking.WantsToBlock) {
            SetState(State.BSpecial);
            character.BackSpecial();
        }
        else if (input.Horizontal * FacingDirection > 0) {
            SetState(State.FSpecial);
            character.ForwardSpecial();
        }
        else {
            SetState(State.NSpecial);
            character.NeutralSpecial();
        }

        animator.SetTrigger("Special");
    }

    void HandleAttackInput() {
        if (comboLocked)
            return;

        if (currentState == State.Idle) {
            StartAttack1();
            return;
        }

        if (currentState == State.Attack1 || currentState == State.Attack2) {
            comboBuffered = true;
        }
    }

    void StartAttack1() {
        SetState(State.Attack1);
        comboBuffered = false;
        character.OnAttack1();
        animator.SetTrigger("Attack");
    }

    void StartAttack2() {
        SetState(State.Attack2);
        comboBuffered = false;
        character.OnAttack2();
        animator.SetTrigger("Attack");
    }

    void StartAttack3() {
        SetState(State.Attack3);
        comboBuffered = false;
        comboLocked = true;
        character.OnAttack3();
        animator.SetTrigger("Attack");
    }

    public void Anim_SpawnHitbox() {
        activeHitbox = Instantiate(
            attackHitboxPrefab,
            attackSpawnPoint.position,
            attackSpawnPoint.rotation
        );
        activeHitbox.GetComponent<PlayerAttack>().Init(this);
    }

    public void Anim_EndHitbox() {
        if (activeHitbox != null)
            Destroy(activeHitbox);
    }

    public void Anim_OpenComboWindow() {
        comboBuffered = false;
    }

    public void Anim_CheckCombo() {
        if (!comboBuffered)
            return;

        if (currentState == State.Attack1)
            StartAttack2();
        else if (currentState == State.Attack2)
            StartAttack3();
    }

    public void Anim_EndAttack() {
        SetState(State.Idle);
    }

    public void Anim_UnlockCombo() {
        comboLocked = false;
    }

    public void TakeHit(Vector3 hitDirection) {
        if (currentState == State.Hitstun || currentState == State.BlockStun)
            return;

        CancelAttack();

        if (currentState == State.HoldingBack) {
            TakeBlockHit(hitDirection);
            return;
        }

        TakeNormalHit(hitDirection);
    }

    void TakeNormalHit(Vector3 hitDir) {
        SetState(State.Hitstun);
        movement.ResetVelocity();
        movement.ApplyKnockback(hitDir.x * knockbackForce, launchForce);
        animator.SetTrigger("Hit");
    }

    void TakeBlockHit(Vector3 hitDir) {
        SetState(State.BlockStun);
        movement.ResetVelocity();
        movement.ApplyKnockback(hitDir.x * knockbackForce * 0.3f, 0f);
        animator.SetTrigger("BlockHit");
    }

    public void Anim_EndHitstun() {
        SetState(State.Idle);
    }

    public void Anim_EndBlockstun() {
        SetState(State.Idle);
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

    void CancelAttack() {
        comboBuffered = false;
        comboLocked = false;
        Anim_EndHitbox();
    }

    void SetState(State newState) {
        currentState = newState;
    }

    void UpdateFacing() {
        if (opponent == null || !movement.isGrounded)
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
        graphics.localScale = new Vector3(
            Mathf.Abs(graphics.localScale.x) * FacingDirection,
            graphics.localScale.y,
            graphics.localScale.z
        );

        visor.localScale = new Vector3(
            Mathf.Abs(visor.localScale.x) * FacingDirection,
            visor.localScale.y,
            visor.localScale.z
        );

        attackSpawnPoint.localPosition = new Vector3(
            Mathf.Abs(attackSpawnPoint.localPosition.x) * FacingDirection,
            attackSpawnPoint.localPosition.y,
            attackSpawnPoint.localPosition.z
        );
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