using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController instances; 
    [SerializeField] private PlayerInput input;

    // NOTE: Overworld action
    public InputAction move { get; private set; } 
    public InputAction attack { get; private set; } 
    public InputAction sling { get; private set; } 
    public InputAction roll { get; private set; } 
    public InputAction test { get; private set; }
    public InputAction heal { get; private set; }

    // NOTE: Global action
    public InputAction interact { get; private set; }

    // NOTE: UI action
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
        heal = input.actions["Heal"];
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
        heal.Enable();
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
        heal.Disable();
        inventory.Disable();
        switchInventoryRight.Disable();
        switchInventoryLeft.Disable();
    }
}
