using UnityEngine;
using System.Collections;

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
        BlockStun,
        ComboWindow
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
    [SerializeField] float comboBufferTime = 0.4f;
    [SerializeField] float comboWindowDuration = 0.25f;

    float comboWindowTimer;
    bool isAttacking;
    bool comboBuffered;
    float comboBufferTimer;
    int comboStep; 

    [Header("Hitstun")]
    [SerializeField] float hitstunDuration = 0.25f;
    [SerializeField] float blockstunDuration = 0.18f;

    float hitstunTimer;

    int hashAttack;
    int hashAttackIndex;
    int hashAttackAir;
    int hashHit;
    int hashBlockHit;

    [Header("Knockdown")]
    [SerializeField] float knockdownDuration = 0.6f;
    [SerializeField] float recoveryDuration = 0.6f;

    float knockdownTimer;
    float recoveryTimer;
    int hashKnockDown;

    public bool IsUnhittable => currentState == State.KnockedDown || currentState == State.Recovery;


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
        hashAttackAir = Animator.StringToHash("AttackAir");
        hashHit = Animator.StringToHash("Hit");
        hashKnockDown = Animator.StringToHash("KnockDown");
        hashBlockHit = Animator.StringToHash("BlockHit");
    }

    void Start() {
        FacingDirection = transform.localScale.x >= 0 ? 1 : -1;
    }

    void Update() {
        UpdateFacing();

        //Debug.Log(currentState);

        if (comboBuffered)
        {
            comboBufferTimer -= Time.deltaTime;
            if (comboBufferTimer <= 0f)
                comboBuffered = false;
        }

        if (currentState == State.Hitstun)
        {
            hitstunTimer -= Time.deltaTime;
            if (hitstunTimer <= 0f)
                ExitHitstun();
            return;
        }

        if (currentState == State.KnockedDown)
        {
            knockdownTimer -= Time.deltaTime;
            if (knockdownTimer <= 0f)
                EnterRecovery();
            return;
        }

        if (currentState == State.Recovery)
        {
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0f)
                ExitRecovery();
            return;
        }

        if (currentState == State.BlockStun)
        {
            hitstunTimer -= Time.deltaTime;
            if (hitstunTimer <= 0f)
                ExitBlockstun();
            return;
        }

        animator.SetFloat(hashSpeed, Mathf.Abs(input.Horizontal)); 
        animator.SetBool(hashIsGrounded, movement.isGrounded); 
        animator.SetBool(hashIsHoldingBack, currentState == State.HoldingBack);

        if (currentState == State.Hitstun || currentState == State.BlockStun)
            return;

        if (!isAttacking) {
            HandleMovement();
        }
        HandleBlocking();
        HandleInputs();
        HandleComboWindow();
    }

    private void HandleInputs() {

        if (input.AttackPressed)
            HandleAttackInput();

            
        if (input.SpecialPressed)
            HandleSpecialInput();
    }

    private void HandleMovement() {
        movement.Move(input.Horizontal);

        if (input.JumpPressed)
            movement.Jump(input.Horizontal);
    }

    private void HandleBlocking() {
        if (currentState == State.Idle && blocking.WantsToBlock)
            SetState(State.HoldingBack);
        else if (currentState == State.HoldingBack && !blocking.WantsToBlock)
            SetState(State.Idle);
    }

    private void HandleAttackInput() {
        if (!movement.isGrounded && !isAttacking)
        {
            StartAirAttack();
            return;
        }

        comboBuffered = true;
        comboBufferTimer = comboBufferTime;

        if (currentState == State.Idle)
        {
            StartAttack(1);
        }
    }

    private void HandleSpecialInput() {
        if (!CanSpecial())
            return;

        if (!movement.isGrounded)
        {
            StartAirSpecial();
            return;
        }

        if (currentState == State.HoldingBack)
        {
            StartBackSpecial();
            return;
        }

        if (Mathf.Abs(input.Horizontal) > 0.1f)
        {
            float dir = input.Horizontal * FacingDirection;

            if (dir > 0f)
            {
                StartForwardSpecial();
                return;
            }
        }

        StartNeutralSpecial();
    }

    private bool CanSpecial() {
        return (currentState == State.Idle || currentState == State.HoldingBack) && !isAttacking;
    }


    private bool CanAttack() {
        return !isAttacking && currentState == State.Idle;
    }


    void StartAttack(int step)
    {
        if (isAttacking) {
            return;
        }

        comboBuffered = false;
        comboStep = step;
        isAttacking = true;

        switch (step)
        {
            case 1:
                currentState = State.Attack1;
                animator.CrossFade("Attack1", 0.05f);
                character.OnAttack1();
                break;

            case 2:
                currentState = State.Attack2;
                animator.CrossFade("Attack2", 0.05f);
                character.OnAttack2();
                break;

            case 3:
                currentState = State.Attack3;
                animator.CrossFade("Attack3", 0.05f);
                character.OnAttack3();
                break;
        }
    }

    void StartAirAttack() {
        isAttacking = true;
        comboBuffered = false;
        comboStep = 0;

        currentState = State.AttackA;

        animator.ResetTrigger(hashAttackAir);
        animator.SetTrigger(hashAttackAir);

        character.OnAttackAir();
    }

    void StartNeutralSpecial() {
        isAttacking = true;
        currentState = State.NSpecial;

        animator.CrossFade("NSpecial", 0.05f);
        character.NeutralSpecial();
    }

    void StartForwardSpecial() {
        isAttacking = true;
        currentState = State.FSpecial;

        animator.CrossFade("FSpecial", 0.05f);
        character.ForwardSpecial();
    }

    void StartBackSpecial() {
        isAttacking = true;
        currentState = State.BSpecial;

        animator.CrossFade("BSpecial", 0.05f);
        character.BackSpecial();
    }

    void StartAirSpecial() {
        isAttacking = true;
        currentState = State.ASpecial;

        animator.CrossFade("ASpecial", 0.05f);
        character.AirSpecial();
    }

    void HandleComboWindow()
    {
        if (currentState != State.ComboWindow)
            return;

        comboWindowTimer -= Time.deltaTime;

        if (comboBuffered && comboStep < 3)
        {
            comboBuffered = false;
            StartAttack(comboStep + 1);
            return;
        }

        if (comboWindowTimer <= 0f)
        {
            comboBuffered = false;
            comboStep = 0;
            currentState = State.Idle;
        }
    }


    public void Anim_OnAttackEnd() {
        isAttacking = false;
        currentState = State.ComboWindow;
        comboWindowTimer = comboWindowDuration;
    }

    public void Anim_OnSpecialEnd() {
        isAttacking = false;

        if (movement.isGrounded)
            SetState(State.Idle);
        else
            SetState(State.Idle);
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

        hitstunTimer = hitstunDuration;

        movement.ResetVelocity();
        movement.ApplyKnockback(hitDir.x * knockbackForce, 0f);

        animator.ResetTrigger(hashHit);
        animator.SetTrigger(hashHit);
    }


    void TakeBlockHit(Vector3 hitDir) {
        SetState(State.BlockStun);

        hitstunTimer = blockstunDuration;

        movement.ResetVelocity();
        movement.ApplyKnockback(hitDir.x * knockbackForce * 0.3f, 0f);

        animator.ResetTrigger(hashBlockHit);
        animator.SetTrigger(hashBlockHit);
    }

    void ExitHitstun() {
        hitstunTimer = 0f;
        SetState(State.Idle);
    }

    void ExitBlockstun() {
        hitstunTimer = 0f;
        SetState(State.Idle);
    }


    public void Anim_EndHitbox() {
        if (activeHitbox != null)
            Destroy(activeHitbox);
    }

    public void TakeSpecialHit(Vector3 hitDir) {
        if (currentState == State.KnockedDown || currentState == State.Recovery)
            return;

        CancelAttack();

        SetState(State.KnockedDown);

        knockdownTimer = knockdownDuration;

        movement.ResetVelocity();
        movement.ApplyKnockback(hitDir.x * knockbackForce, 0f);

        animator.ResetTrigger(hashKnockDown);
        animator.SetTrigger(hashKnockDown);
    }


    void EnterRecovery() {
        SetState(State.Recovery);
        recoveryTimer = recoveryDuration;

        animator.CrossFade("Recovery", 0.05f);
    }

    void ExitRecovery() {
        recoveryTimer = 0f;
        SetState(State.Idle);
    }

    public void TakeProjectileHit() {
        if (currentState == State.Hitstun || currentState == State.BlockStun)
            return;

        CancelAttack();

        SetState(State.Hitstun);
        hitstunTimer = hitstunDuration;

        movement.ResetVelocity();
        movement.ApplyKnockback(-FacingDirection * knockbackForce * 0.5f, 0f);

        animator.ResetTrigger(hashHit);
        animator.SetTrigger(hashHit);
    }


    public void SpawnHitbox(GameObject prefab, float duration) {
        if (activeHitbox != null) {
            Destroy(activeHitbox);
        }

        activeHitbox = Instantiate(prefab, attackSpawnPoint.position, attackSpawnPoint.rotation, transform);

        PlayerAttack atk = activeHitbox.GetComponent<PlayerAttack>();
        atk.Init(this);

        Invoke(nameof(Anim_EndHitbox), duration);
    }

    public void SpawnHitboxDelayed(GameObject prefab, float delay, float duration) {
        StartCoroutine(SpawnHitboxDelayedRoutine(prefab, delay, duration));
    }

    private IEnumerator SpawnHitboxDelayedRoutine(GameObject prefab, float delay, float duration) {
        yield return new WaitForSeconds(delay);
        SpawnHitbox(prefab, duration);
    }


    public void PushForward(float force) {
        movement.ApplyKnockback(force * FacingDirection, 0f);
    }

    private void UpdateFacing() {
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

    public void ApplyKnockBack(float x, float y) {
        movement.ApplyKnockback(x, y);
    }

    public void ResetVelocity() {
        movement.ResetVelocity();
    }

    private void FlipVisuals() {
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