using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float lifetime = 3f;

    int direction;
    Character owner;

    public void Init(int facingDirection, Character ownerCharacter)
    {
        direction = facingDirection;
        owner = ownerCharacter;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        PlayerState target = other.GetComponent<PlayerState>();
        if (target == null) return;

        if (target.IsUnhittable)
            return;

        if (target.GetComponent<Character>() == owner)
            return;

        target.TakeProjectileHit();
        Destroy(gameObject);
    }

}
