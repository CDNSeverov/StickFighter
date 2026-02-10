using UnityEngine;
using System.Collections;   

public class Brawler : Character
{
    [Header("Hitboxes")] // draw these
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
        state.SpawnHitboxDelayed(attack2Hitbox, 0.3f, 0.16f);
    }

    public override void OnAttack3() {
        Debug.Log("Brawler Attack 3");
        state.PushForward(7f);
        state.SpawnHitboxDelayed(attack3Hitbox, 0.3f, 0.18f);
    }

    public override void OnAttackAir() {
        Debug.Log("Brawler Attack Air");
        state.SpawnHitboxDelayed(attackAirHitbox, 0.3f, 0.18f);
    }

    public override void NeutralSpecial() {
        Debug.Log("Brawler Neutral Special");

        StartCoroutine(NeutralSpecialRoutine());
    }

    private IEnumerator NeutralSpecialRoutine() {
        yield return new WaitForSeconds(0.6f);

        GameObject fireball = Instantiate(fireballPrefab, state.transform.position + Vector3.right * state.FacingDirection * 1.2f, Quaternion.identity);

        fireball.GetComponent<Projectile>().Init(state.FacingDirection, this);
    }

    public override void ForwardSpecial() {
        Debug.Log("Brawler Forward Special");

        state.PushForward(4f);
        state.SpawnHitboxDelayed(fSpecialHitbox, 0.2f, 0.5f); 
        StartCoroutine(ForwardSpecialRoutine());
    }

    private IEnumerator ForwardSpecialRoutine() {
        yield return new WaitForSeconds(0.2f);
        state.ApplyKnockBack(5f * state.FacingDirection, 10f);
    }

    public override void BackSpecial() {
        Debug.Log("Brawler Back Special");

        state.PushForward(17f);
        state.SpawnHitboxDelayed(bSpecialHitbox, 0.2f, 0.3f);
    }


    public override void AirSpecial() {
        Debug.Log("Brawler Air Special");

        state.ResetVelocity();

        state.SpawnHitboxDelayed(aSpecialHitbox, 0.4f, 0.4f);
        state.ApplyKnockBack(13f * state.FacingDirection, -6f);
    }

}
