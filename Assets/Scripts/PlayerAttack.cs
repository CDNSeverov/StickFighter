using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerState owner;

    public void Init(PlayerState player) {
        owner = player;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == owner.gameObject) {
            return;
        }

        PlayerState target = other.GetComponent<PlayerState>();
        if (target == null) {
            return;
        }

        Vector3 hitDirection = (other.transform.position - owner.transform.position).normalized;

        target.TakeHit(hitDirection);

        Debug.Log($"{other.name} was hit");
    }
}
