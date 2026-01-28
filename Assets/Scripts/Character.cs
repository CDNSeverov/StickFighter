using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected PlayerState state;
    protected PlayerMovement movement;

    protected virtual void Awake() {
        state = GetComponent<PlayerState>();
        movement = GetComponent<PlayerMovement>();
    }

    // Attacks
    public abstract void OnAttack1();
    public abstract void OnAttack2();
    public abstract void OnAttack3();
    public abstract void OnAttackAir();
    
    // Specials
    public abstract void NeutralSpecial();
    public abstract void ForwardSpecial();
    public abstract void BackSpecial();
    public abstract void AirSpecial();
}
