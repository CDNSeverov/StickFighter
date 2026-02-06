using UnityEngine;

public class Brawler : Character
{
    [Header("Hitboxes")]
    [SerializeField] GameObject attack1Hitbox;
    [SerializeField] GameObject attack2Hitbox;
    [SerializeField] GameObject attack3Hitbox;
    [SerializeField] GameObject attackAirHitbox;
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject fSpecialHitbox;
    [SerializeField] GameObject bSpecialHitbox;
    [SerializeField] GameObject aSpecialHitbox;
    
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

        GameObject fireball = Instantiate(fireballPrefab, state.transform.position + Vector3.right * state.FacingDirection * 1.2f, Quaternion.identity);

        fireball.GetComponent<Projectile>().Init(state.FacingDirection, this);
    }

    public override void ForwardSpecial() {
        Debug.Log("Brawler Forward Special");

        state.PushForward(3f);
        state.SpawnHitboxDelayed(fSpecialHitbox, 0.15f, 0.25f); 
        state.ApplyKnockBack(5f * state.FacingDirection, 10f);
    }

    public override void BackSpecial() {
        Debug.Log("Brawler Back Special");

        state.PushForward(17f);
        state.SpawnHitboxDelayed(bSpecialHitbox, 0.18f, 0.35f);
    }


    public override void AirSpecial() {
        Debug.Log("Brawler Air Special");

        state.ResetVelocity();

        state.SpawnHitboxDelayed(aSpecialHitbox, 0.12f, 0.3f);
        state.ApplyKnockBack(13f * state.FacingDirection, -6f);
    }

}
