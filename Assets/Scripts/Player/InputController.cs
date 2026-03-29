using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private PlayerInput input;

    public InputAction move { get; private set; } 
    public InputAction attack { get; private set; } 
    public InputAction roll { get; private set; } 
    public InputAction test { get; private set; }
    public InputAction interact { get; private set; }

    private void OnEnable()
    {
        EnableInput();
    }

    private void EnableInput()
    {
        move = input.actions["Move"];
        attack = input.actions["Attack"];
        roll = input.actions["Roll"];
        test = input.actions["Test"];
        interact = input.actions["Interact"];

        move.Enable();
        attack.Enable();
        roll.Enable();
        test.Enable();
        interact.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
        roll.Disable();
        test.Disable();
        interact.Disable();
    }
}
