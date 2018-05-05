using UnityEngine;
//using UnitySampleAssets.CrossPlatformInput;
using InControl;

namespace UnitySampleAssets._2D
{

    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D character;
        private bool jump;

        InputDevice player;

        private void Awake()
        {
            character = GetComponent<PlatformerCharacter2D>();
            
        }

        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            InputDevice player = InputManager.Devices[0];

            if (!jump)
                // Read the jump input in Update so button presses aren't missed.
                jump = player.Action2.WasPressed;
            //jump = CrossPlatformInputManager.GetButtonDown("Jump");

            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);

            InputControl movecontrol = player.GetControl(InputControlType.LeftStickX);
            float h = movecontrol.Value;
            // Pass all parameters to the character control script.
            character.Move(h, crouch, jump);
            jump = false;
        }
    }
}