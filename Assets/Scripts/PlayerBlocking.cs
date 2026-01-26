using UnityEngine;

public class PlayerBlocking : MonoBehaviour
{
    PlayerState state;
    PlayerInput input;

    public bool WantsToBlock { get; private set; }

    void Awake() {
        state = GetComponent<PlayerState>();
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        WantsToBlock = input.HoldingBack(state.FacingDirection);
    }
}
