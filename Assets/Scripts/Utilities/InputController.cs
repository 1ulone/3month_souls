using UnityEngine;
using UnityEngine.InputSystem;

namespace wine.util
{
    public enum InputType 
    {
        press,
        hold,
        release
    }

    public class InputController : MonoBehaviour
    {
        public static InputController instances; 
        [SerializeField] private PlayerInput input;

        // NOTE: Overworld action
        private InputAction move; 
        private InputAction attack; 
        private InputAction sling; 
        private InputAction addItem; 
        private InputAction roll; 
        private InputAction test;
        private InputAction heal;

        // NOTE: Global action
        private InputAction interact;

        // NOTE: UI action
        private InputAction inventory;
        private InputAction switchInventoryRight;
        private InputAction switchInventoryLeft;


        private void Awake() 
            => instances = this;

        private void OnEnable()
        {
            move = input.actions["Move"];
            attack = input.actions["Attack"];
            sling = input.actions["Sling"];
            addItem = input.actions["AddItem"];
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
            addItem.Enable();
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
            addItem.Disable();
            roll.Disable();
            test.Disable();
            heal.Disable();
            inventory.Disable();
            switchInventoryRight.Disable();
            switchInventoryLeft.Disable();
        }

        // NOTE: Instant Action Call
        public Vector2 RawMouse()
        {
            return Mouse.current.position.ReadValue();
        }

        public Vector2 Move()
        {
            if (!move.enabled)
                return Vector2.zero;
            return move.ReadValue<Vector2>(); 
        }

        public bool GetInput(string tag, InputType type = InputType.press)
        {
            tag = tag.ToLower();
            InputAction action = tag switch
            {
                "attack" => attack,
                "sling" => sling,
                "additem" => addItem,
                "roll" => roll,
                "test" => test,
                "heal" => heal,
                "interact" => interact,
                "inventory" => inventory,
                "switchinventoryleft" => switchInventoryLeft,
                "switchinventoryright" => switchInventoryRight,
                _ => null,
            };

            if (!action.enabled)
                return false;

            bool inputReturn = action.WasPressedThisFrame();
            switch(type)
            {
                case InputType.press:
                { inputReturn = action.WasPressedThisFrame(); } break;
                case InputType.hold:
                { inputReturn = action.IsInProgress(); } break;
                case InputType.release:
                { inputReturn = action.WasReleasedThisFrame(); } break;
            }

            return inputReturn;
        }
    }
}
