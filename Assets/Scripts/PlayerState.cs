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
    [SerializeField] float comboBufferTime = 0.3f;

    bool isAttacking;
    bool comboBuffered;
    float comboBufferTimer;
    int comboStep; 

    int hashAttack;
    int hashAttackIndex;

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

    int hashSpeed; 
    int hashIsGrounded; 
    int hashIsHoldingBack;

    void Awake() {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        blocking = GetComponent<PlayerBlocking>();
        character = GetComponent<Character>();
        animator = GetComponentInChildren<Animator>();
        
        hashSpeed = Animator.StringToHash("Speed"); 
        hashIsGrounded = Animator.StringToHash("IsGrounded"); 
        hashIsHoldingBack = Animator.StringToHash("IsHoldingBack");
        hashAttack = Animator.StringToHash("Attack");
        hashAttackIndex = Animator.StringToHash("AttackIndex");
    }

    void Start() {
        FacingDirection = transform.localScale.x >= 0 ? 1 : -1;
    }

    void Update() {
        UpdateFacing();

        if (comboBuffered)
        {
            comboBufferTimer -= Time.deltaTime;
            if (comboBufferTimer <= 0f)
                comboBuffered = false;
        }

        animator.SetFloat(hashSpeed, Mathf.Abs(input.Horizontal)); 
        animator.SetBool(hashIsGrounded, movement.isGrounded); 
        animator.SetBool(hashIsHoldingBack, currentState == State.HoldingBack);

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
            Debug.Log("Special");
            //HandleSpecialInput();
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

    void HandleAttackInput()
    {
        if (CanAttack())
        {
            StartAttack(1);
        }

        else if (isAttacking && comboStep < 3)
        {
            comboBuffered = true;
            comboBufferTimer = comboBufferTime;
        }
    }

    private bool CanAttack() {
        return !isAttacking && currentState == State.Idle;
    }


    void StartAttack(int step)
    {
        isAttacking = true;
        comboStep = step;
        comboBuffered = false;

        switch (step)
        {
            case 1: SetState(State.Attack1); character.OnAttack1(); break;
            case 2: SetState(State.Attack2); character.OnAttack2(); break;
            case 3: SetState(State.Attack3); character.OnAttack3(); break;
        }

        animator.ResetTrigger(hashAttack);

        animator.SetInteger(hashAttackIndex, step);
        animator.SetTrigger(hashAttack);
    }

    public void Anim_OnAttackEnd() {
        Debug.Log($"Attack ended. ComboBuffered: {comboBuffered}, comboStep: {comboStep}");
        if (comboBuffered && comboStep < 3) {
            StartAttack(comboStep + 1);
        } else {
            EndCombo();
        }
    }

    void EndCombo()
    {
        isAttacking = false;
        comboBuffered = false;
        comboStep = 0;
        SetState(State.Idle);

        animator.ResetTrigger(hashAttack);
        animator.SetInteger(hashAttackIndex, 0);
    }

    void CancelAttack()
    {
        isAttacking = false;
        comboBuffered = false;
        comboStep = 0;
        Anim_EndHitbox();
    }


    void SetState(State newState) {
        currentState = newState;
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

    public void Anim_EndHitbox() {
        if (activeHitbox != null)
            Destroy(activeHitbox);
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