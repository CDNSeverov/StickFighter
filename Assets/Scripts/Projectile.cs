using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float lifetime = 3f;

    int direction;
    Character owner;

    Vector3 moveDirection = Vector3.right; 

    public void Init(int facingDirection, Character ownerCharacter, bool diagonal = false)
    {
        direction = facingDirection;
        owner = ownerCharacter;

        if (diagonal)
        {
            moveDirection = new Vector3(direction, -1f, 0f).normalized;
        }
        else
        {
            moveDirection = new Vector3(direction, 0f, 0f);
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
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