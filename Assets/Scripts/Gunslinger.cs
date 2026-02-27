using UnityEngine;
using System.Collections;   

public class Gunslinger : Character
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
        Debug.Log("Gunslinger Attack 1");
        state.PushForward(7f);
        state.SpawnHitboxDelayed(attack1Hitbox, 0.3f, 0.12f);
    }

    public override void OnAttack2() {
        Debug.Log("Gunslinger Attack 2");
        state.PushForward(7f);
        state.SpawnHitboxDelayed(attack2Hitbox, 0.2f, 0.16f);
    }

    public override void OnAttack3() {
        Debug.Log("Gunslinger Attack 3");
        state.PushForward(7f);
        state.SpawnHitboxDelayed(attack3Hitbox, 0.3f, 0.18f);
    }

    public override void OnAttackAir() {
        Debug.Log("Gunslinger Attack Air");
        state.SpawnHitboxDelayed(attackAirHitbox, 0.2f, 0.18f);
    }

    public override void NeutralSpecial() {
        Debug.Log("Gunslinger Neutral Special");

        StartCoroutine(NeutralSpecialRoutine());
    }

    private IEnumerator NeutralSpecialRoutine() {
        yield return new WaitForSeconds(0.2f);

        GameObject fireball = Instantiate(fireballPrefab, state.transform.position + Vector3.right * state.FacingDirection * 1.2f, Quaternion.identity);

        fireball.GetComponent<Projectile>().Init(state.FacingDirection, this, 0);
    }

    public override void ForwardSpecial() {
        Debug.Log("Gunslinger Forward Special");

        state.PushForward(4f);
        state.SpawnHitboxDelayed(fSpecialHitbox, 0.2f, 0.5f); 
        StartCoroutine(ForwardSpecialRoutine());
    }

    private IEnumerator ForwardSpecialRoutine() {
        yield return new WaitForSeconds(0.2f);
        state.ApplyKnockBack(5f * state.FacingDirection, 10f);
    }

    public override void BackSpecial() {
        Debug.Log("Gunslinger Back Special");
        
        state.PushForward(12f);
        StartCoroutine(BackSpecialRoutine());

    }

    private IEnumerator BackSpecialRoutine() {
        yield return new WaitForSeconds(0.3f);

        GameObject fireball = Instantiate(fireballPrefab, state.transform.position + Vector3.right * state.FacingDirection * 1.2f, Quaternion.identity);

        fireball.GetComponent<Projectile>().Init(state.FacingDirection, this, -1);
    }

    public override void AirSpecial() { 
        Debug.Log("Gunslinger Air Special");

        state.ResetVelocity();
        state.PushForward(-4f);
        
        StartCoroutine(AirSpecialRoutine());
    }

    private IEnumerator AirSpecialRoutine() {
        yield return new WaitForSeconds(0.3f);

        GameObject fireball = Instantiate(fireballPrefab, state.transform.position + Vector3.right * state.FacingDirection * 1.2f, Quaternion.identity);
        
        state.ApplyKnockBack(-5f * state.FacingDirection, 10f);

        fireball.GetComponent<Projectile>().Init(state.FacingDirection, this, 1);
    }

}
