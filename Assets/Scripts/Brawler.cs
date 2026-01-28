using UnityEngine;

public class Brawler : Character
{
    public override void OnAttack1() {
        Debug.Log("Brawler Attack 1");
    }

    public override void OnAttack2() {
        Debug.Log("Brawler Attack 2");
    }

    public override void OnAttack3() {
        Debug.Log("Brawler Attack 3");
    }

    public override void OnAttackAir() {
        Debug.Log("Brawler Attack Air");
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
