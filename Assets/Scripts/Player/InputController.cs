using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController instances; 
    [SerializeField] private PlayerInput input;

    public InputAction move { get; private set; } 
    public InputAction attack { get; private set; } 
    public InputAction sling { get; private set; } 
    public InputAction roll { get; private set; } 
    public InputAction test { get; private set; }
    public InputAction interact { get; private set; }
    public InputAction inventory { get; private set; }
    public InputAction switchInventoryRight { get; private set; }
    public InputAction switchInventoryLeft { get; private set; }

    private void Awake() 
        => instances = this;

    private void OnEnable()
    {
        move = input.actions["Move"];
        attack = input.actions["Attack"];
        sling = input.actions["Sling"];
        roll = input.actions["Roll"];
        test = input.actions["Test"];
        inventory = input.actions["Inventory"];
        switchInventoryRight = input.actions["SwitchInventoryRight"];
        switchInventoryLeft = input.actions["SwitchInventoryLeft"];
        interact = input.actions["Interact"];

        EnableInput();
        interact.Enable();
    }

    public void EnableInput()
    {
        move.Enable();
        attack.Enable();
        sling.Enable();
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
        interact.Disable();
    }

    public void DisableInput()
    {
        move.Disable();
        attack.Disable();
        sling.Disable();
        roll.Disable();
        test.Disable();
        inventory.Disable();
        switchInventoryRight.Disable();
        switchInventoryLeft.Disable();
    }
}
