using UnityEngine;

public class UnitySampleScript : MonoBehaviour
{
	void Start ()
    {
	
	}
	

	void Update ()
    {
        // Update once(!) per Unity update.
        // It checks all 4 controller states and their input.
        Gamepad.Update();



        // BUTTONS ---------------------------------------------

        if (Gamepad.A(0, ButtonState.Down))
        {
            // Controller 1 pressed the A Button
        }

        if (Gamepad.A(0, ButtonState.Start))
        {
            // Controller 1 pressed the Start Button
        }

        if (Gamepad.A(1, ButtonState.Down))
        {
            // Controller 2 pressed the A Button
        }



        // ANALOG STICK -----------------------------------------

        // Check the amplitude of the left analog stick
        int x = Gamepad.GetLeftAnalogX(0);
        int y = Gamepad.GetLeftAnalogY(0);

        // Or simply check the direction
        if (Gamepad.LeftAnalogLeft(0, ButtonState.Pressed))
        {
            // Left
        }
        if (Gamepad.LeftAnalogRight(0, ButtonState.Pressed))
        {
            // Right
        }
        if (Gamepad.LeftAnalogUp(0, ButtonState.Pressed))
        {
            // Up
        }
        if (Gamepad.LeftAnalogDown(0, ButtonState.Pressed))
        {
            // Down
        }



        // RUMBLE -----------------------------------------------

        // This lets Controller 1 rumble for 0.5 seconds
        // You can specify the strength of the left and the right motor
        Gamepad.SetRumbleOn(0, 1.0f, 0.0f, 0.5f);

        // Turns off the rumble
        Gamepad.SetRumbleOff(0);
    }
}
