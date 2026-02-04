using UnityEngine;

public class PlayerInput: MonoBehaviour
{
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode jumpButton;
    public KeyCode attack;
    public KeyCode special;

    public float Horizontal { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool SpecialPressed { get; private set; }

    void Update() {
        Horizontal = 0f;
        if (Input.GetKey(moveLeft)) Horizontal -= 1f;
        if (Input.GetKey(moveRight)) Horizontal += 1f;

        JumpPressed = Input.GetKeyDown(jumpButton);
        AttackPressed = Input.GetKeyDown(attack);
        SpecialPressed = Input.GetKeyDown(special);

    }

    
    public bool HoldingBack(int FacingDirection) {
        return Horizontal * FacingDirection < 0f;
    }

}
