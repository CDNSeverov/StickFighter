using UnityEngine;

public class Brawler : Character
{
    [Header("Hitboxes")]
    [SerializeField] GameObject attack1Hitbox;
    [SerializeField] GameObject attack2Hitbox;
    [SerializeField] GameObject attack3Hitbox;
    [SerializeField] GameObject attackAirHitbox;
    
    public override void OnAttack1() {
        Debug.Log("Brawler Attack 1");
        state.PushForward(7f);
        state.SpawnHitbox(attack1Hitbox, 0.12f);
    }

    public override void OnAttack2() {
        Debug.Log("Brawler Attack 2");
        state.PushForward(7f);
        state.SpawnHitbox(attack2Hitbox, 0.16f);
    }

    public override void OnAttack3() {
        Debug.Log("Brawler Attack 3");
        state.PushForward(7f);
        state.SpawnHitbox(attack3Hitbox, 0.18f);
    }

    public override void OnAttackAir() {
        Debug.Log("Brawler Attack Air");
        state.SpawnHitbox(attackAirHitbox, 0.18f);
    }

    public override void NeutralSpecial() {
        Debug.Log("Brawler Neutral Special");
    }

    public override void ForwardSpecial() {
        Debug.Log("Brawler Forward Special");
    }

    public override void BackSpecial() {
        Debug.Log("Brawler Back Special");
    }

    public override void AirSpecial() {
        Debug.Log("Brawler Air Special");
    }
}
