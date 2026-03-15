using UnityEngine;
using System.Collections;   

public class Swordsman : Character
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

    [Header("Audio")]
    [SerializeField] AudioSource src;
    [SerializeField] AudioClip punchSFX;
    [SerializeField] AudioClip slashSFX;
    [SerializeField] AudioClip knifeSFX;

    public override void OnAttack1() {
        //Debug.Log("Swordsman Attack 1");
        state.PushForward(7f);
        state.SpawnHitboxDelayed(attack1Hitbox, 0.3f, 0.12f);
        src.clip = punchSFX;
        src.Play();
    }

    public override void OnAttack2() {
        //Debug.Log("Swordsman Attack 2");
        state.PushForward(7f);
        state.SpawnHitboxDelayed(attack2Hitbox, 0.2f, 0.16f);
        src.clip = slashSFX;
        src.Play();
    }

    public override void OnAttack3() {
        //Debug.Log("Swordsman Attack 3");
        state.PushForward(7f);
        state.SpawnHitboxDelayed(attack3Hitbox, 0.3f, 0.18f);
        src.clip = slashSFX;
        src.Play();
    }

    public override void OnAttackAir() {
        //Debug.Log("Swordsman Attack Air");
        state.SpawnHitboxDelayed(attackAirHitbox, 0.2f, 0.18f);
        src.clip = slashSFX;
        src.Play();
    }

    public override void NeutralSpecial() {
        //Debug.Log("Swordsman Neutral Special");
        
        src.clip = knifeSFX;
        src.Play();

        StartCoroutine(NeutralSpecialRoutine());
    }

    private IEnumerator NeutralSpecialRoutine() {
        yield return new WaitForSeconds(0.3f);

        GameObject fireball = Instantiate(fireballPrefab, state.transform.position + Vector3.right * state.FacingDirection * 1.2f, Quaternion.identity);

        fireball.GetComponent<Projectile>().Init(state.FacingDirection, this, 0);
    }

    public override void ForwardSpecial() {
        //Debug.Log("Swordsman Forward Special");

        state.PushForward(4f);
        src.clip = slashSFX;
        src.Play();
        state.SpawnHitboxDelayed(fSpecialHitbox, 0.2f, 0.5f); 
        StartCoroutine(ForwardSpecialRoutine());
    }

    private IEnumerator ForwardSpecialRoutine() {
        yield return new WaitForSeconds(0.2f);
        state.ApplyKnockBack(5f * state.FacingDirection, 10f);
    }

    public override void BackSpecial() {
        //Debug.Log("Swordsman Back Special");
        
        src.clip = slashSFX;
        src.Play();
        state.PushForward(17f);
        state.SpawnHitboxDelayed(bSpecialHitbox, 0.4f, 0.2f);
    }


    public override void AirSpecial() { 
        //Debug.Log("Swordsman Air Special");

        state.ResetVelocity();
        
        src.clip = knifeSFX;
        src.Play();
        
        StartCoroutine(AirSpecialRoutine());
    }

    private IEnumerator AirSpecialRoutine() {
        yield return new WaitForSeconds(0.3f);

        GameObject fireball = Instantiate(fireballPrefab, state.transform.position + Vector3.right * state.FacingDirection * 1.2f, Quaternion.identity);

        fireball.GetComponent<Projectile>().Init(state.FacingDirection, this, 1);
    }

}
