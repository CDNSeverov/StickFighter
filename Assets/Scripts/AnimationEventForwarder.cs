using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    public PlayerState owner; 

    public void Anim_OnAttackEnd()
    {
        if (owner != null)
            owner.Anim_OnAttackEnd();
    }
}
