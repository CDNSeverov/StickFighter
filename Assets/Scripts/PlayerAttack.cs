using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerState owner;
    bool hasHit;
    [SerializeField] bool isSpecialHit;

    public void Init(PlayerState player) {
        owner = player;
        hasHit = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (hasHit) return;
        if (other.gameObject == owner.gameObject) {
            return;
        }

        PlayerState target = other.GetComponent<PlayerState>();
        if (target == null) {
            return;
        }

        hasHit = true;

        Vector3 hitDirection = (other.transform.position - owner.transform.position).normalized;

        if (isSpecialHit)
            target.TakeSpecialHit();
        else
            target.TakeHit(hitDirection);

        Debug.Log($"{other.name} was hit");
    }
}
