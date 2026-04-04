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
    public InputAction inventory { get; private set; }
    public InputAction switchInventoryRight { get; private set; }
    public InputAction switchInventoryLeft { get; private set; }

    private void OnEnable()
    {
        EnableInput();
    }

    public void EnableInput()
    {
        move = input.actions["Move"];
        attack = input.actions["Attack"];
        roll = input.actions["Roll"];
        test = input.actions["Test"];
        interact = input.actions["Interact"];
        inventory = input.actions["Inventory"];
        switchInventoryRight = input.actions["SwitchInventoryRight"];
        switchInventoryLeft = input.actions["SwitchInventoryLeft"];

        move.Enable();
        attack.Enable();
        roll.Enable();
        test.Enable();
        interact.Enable();
        inventory.Enable();
        switchInventoryRight.Enable();
        switchInventoryLeft.Enable();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    public void DisableInput()
    {
        move.Disable();
        attack.Disable();
        roll.Disable();
        test.Disable();
        interact.Disable();
        inventory.Disable();
        switchInventoryRight.Disable();
        switchInventoryLeft.Disable();
    }
}
