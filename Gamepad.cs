using System.Runtime.InteropServices;
using UnityEngine;

public static class Gamepad
{
    const byte DEADZONE_TRIGGER = 30;           // Range: 0 - 255
    const short DEADZONE_LEFT_ANALOG = 12500;   // Range: -32.768 - 32.767
    const short DEADZONE_RIGHT_ANALOG = 12500;  // Range: -32.768 - 32.767

    private static XboxController[] m_XboxController = new XboxController[4];

    static Gamepad()
    {
        for (int i = 0; i < 4; ++i)
        {
            m_XboxController[i] = new XboxController(i, DEADZONE_TRIGGER, DEADZONE_LEFT_ANALOG, DEADZONE_RIGHT_ANALOG);
        }
    }

    /// <summary>
    /// Checks for new User Input. Call this once per Unity update.
    /// </summary>
    public static void Update()
    {
        foreach (XboxController controller in m_XboxController)
        {
            controller.Update();
        }
    }

    /// <summary>
    /// Checks if the controller is connected and ready to use.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <returns>Value (true/false) if the controller is connected.</returns>
    public static bool IsConnected(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            return m_XboxController[a_ControllerIndex].m_IsConnected;
        }

        return false;
    }

    /// <summary>
    /// Turns the controller rumble on.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_LeftMotor">Strength of the left motor (0.0f - 1.0f).</param>
    /// <param name="a_RightMotor">Strength of the right motor (0.0f - 1.0f).</param>
    /// <param name="a_Time">Time in seconds.</param>
    public static void SetRumbleOn(int a_ControllerIndex, float a_LeftMotor, float a_RightMotor, float a_Time)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            if (a_Time >= 0.0f)
            {
                if (a_LeftMotor > 1.0f) a_LeftMotor = 1.0f;
                else if (a_LeftMotor < 0.0f) a_LeftMotor = 0.0f;

                if (a_RightMotor > 1.0f) a_RightMotor = 1.0f;
                else if (a_RightMotor < 0.0f) a_RightMotor = 0.0f;

                ushort rumbleLeft = System.Convert.ToUInt16(a_LeftMotor * ushort.MaxValue);
                ushort rumbleRight = System.Convert.ToUInt16(a_RightMotor * ushort.MaxValue);

                XInput.XInputVibration xInputVibration = new XInput.XInputVibration(rumbleLeft, rumbleRight);
                XInput.XInputSetState(a_ControllerIndex, ref xInputVibration);

                m_XboxController[a_ControllerIndex].m_RumbleTime = a_Time;
                m_XboxController[a_ControllerIndex].m_RumbleHasEnded = false;
            }
        }
    }

    /// <summary>
    /// Turns the controller rumble off.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    public static void SetRumbleOff(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            m_XboxController[a_ControllerIndex].m_RumbleTime = 0.0f;

            XInput.XInputVibration xInputVibration = new XInput.XInputVibration(0, 0);
            XInput.XInputSetState(a_ControllerIndex, ref xInputVibration);
            m_XboxController[a_ControllerIndex].m_RumbleHasEnded = false;
        }
    }

    /// <summary>
    /// Returns the X axis' amplitude of the left analog stick (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <returns>Value is between -32.768 and 32.767.</returns>
    public static int GetLeftAnalogX(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            short a = m_XboxController[a_ControllerIndex].m_LeftAnalogX;
            short b = m_XboxController[a_ControllerIndex].m_LeftAnalogY;
            return DeadzoneCheck(a, b, DEADZONE_LEFT_ANALOG);
        }

        return 0;
    }

    /// <summary>
    /// Returns the Y axis' amplitude of the left analog stick (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <returns>Value is between -32.768 and 32.767.</returns>
    public static int GetLeftAnalogY(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            short a = m_XboxController[a_ControllerIndex].m_LeftAnalogY;
            short b = m_XboxController[a_ControllerIndex].m_LeftAnalogX;
            return DeadzoneCheck(a, b, DEADZONE_LEFT_ANALOG);
        }

        return 0;
    }

    /// <summary>
    /// Returns the x axis' amplitude of the right analog stick (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <returns>Value is between -32.768 and 32.767.</returns>
    public static int GetRightAnalogX(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            short a = m_XboxController[a_ControllerIndex].m_RightAnalogX;
            short b = m_XboxController[a_ControllerIndex].m_RightAnalogY;
            return DeadzoneCheck(a, b, DEADZONE_LEFT_ANALOG);
        }

        return 0;
    }

    /// <summary>
    /// Returns the Y axis' amplitude of the right analog stick (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <returns>Value is between -32.768 and 32.767.</returns>
    public static int GetRightAnalogY(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            short a = m_XboxController[a_ControllerIndex].m_RightAnalogY;
            short b = m_XboxController[a_ControllerIndex].m_RightAnalogX;
            return DeadzoneCheck(a, b, DEADZONE_LEFT_ANALOG);
        }

        return 0;
    }

    /// <summary>
    /// Returns the amplitude of the left trigger (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <returns>Value is between 0 and 255.</returns>
    public static int GetLeftTrigger(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            short a = m_XboxController[a_ControllerIndex].m_LeftTrigger;
            short b = 0;
            return DeadzoneCheck(a, b, DEADZONE_TRIGGER);
        }

        return 0;
    }

    /// <summary>
    /// Returns the amplitude of the right trigger (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <returns>Value is between 0 and 255.</returns>
    public static int GetRightTrigger(int a_ControllerIndex)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            short a = m_XboxController[a_ControllerIndex].m_RightTrigger;
            short b = 0;
            return DeadzoneCheck(a, b, DEADZONE_TRIGGER);
        }

        return 0;
    }

    /// <summary>
    /// Checks the Left Bumper state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool LeftBumper(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_LeftBumper;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Right Bumper state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool RightBumper(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_RightBumper;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Left Trigger state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool LeftTrigger(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_LeftTriggerState;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Right Trigger state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool RightTrigger(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_RightTriggerState;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the A button state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool A(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_A;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the B button state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool B(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_B;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the X button state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool X(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_X;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Y button state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool Y(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_Y;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Start button state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool Start(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_Start;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Back button state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool Back(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_Back;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Digital Pad Left state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool DigitalLeft(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_DigitalLeft;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Digital Pad Right state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool DigitalRight(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_DigitalRight;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Digital Pad Up state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool DigitalUp(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_DigitalUp;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Digital Pad Down state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool DigitalDown(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_DigitalDown;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Left Analog Stick Left state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool LeftAnalogLeft(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_LeftAnalogLeft;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Left Analog Stick Right state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool LeftAnalogRight(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_LeftAnalogRight;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Left Analog Stick Up state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool LeftAnalogUp(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_LeftAnalogUp;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Left Analog Stick Down state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool LeftAnalogDown(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_LeftAnalogDown;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Right Analog Stick Left state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool RightAnalogLeft(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_RightAnalogLeft;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Right Analog Stick Right state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool RightAnalogRight(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_RightAnalogRight;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Right Analog Stick Up state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool RightAnalogUp(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_RightAnalogUp;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Right Analog Stick Down state (performs a dead zone check).
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool RightAnalogDown(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_RightAnalogDown;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Left Thumb state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool LeftThumb(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_LeftThumb;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks the Right Thumb state.
    /// </summary>
    /// <param name="a_ControllerIndex">Index can be from 0-3.</param>
    /// <param name="a_ButtonState">The button state that shall be checked.</param>
    /// <returns>Returns the value (true/false) if the states match.</returns>
    public static bool RightThumb(int a_ControllerIndex, ButtonState a_ButtonState)
    {
        if (a_ControllerIndex >= 0 && a_ControllerIndex <= 3)
        {
            ButtonState current = m_XboxController[a_ControllerIndex].m_RightThumb;
            switch (a_ButtonState)
            {
                case ButtonState.Pressed:
                    {
                        bool value = (current == ButtonState.Pressed) || (current == ButtonState.Down);
                        return value;
                    }
                case ButtonState.Released:
                    {
                        bool value = (current == ButtonState.Released) || (current == ButtonState.Up);
                        return value;
                    }
                default:
                    {
                        bool value = (current == a_ButtonState);
                        return value;
                    }
            }
        }

        return false;
    }

    // --------------------------------------------------------------------------------

    private static int DeadzoneCheck(short a, short b, short deadzone)
    {
        int num1 = a;
        int num2 = b;

        if (num1 < 0) num1 *= -1;
        if (num2 < 0) num2 *= -1;

        if (num1 + num2 > deadzone)
        {
            return a;
        }
        else
        {
            return 0;
        }
    }

    // --------------------------------------------------------------------------------

    private class XboxController
    {
        const uint CONNECTION_SUCCESS = 0x000;

        private byte m_DeadzoneTrigger = 30;
        private short m_DeadzoneLeftAnalog = 12500;
        private short m_DeadzoneRightTrigger = 12500;
        private int m_Index = 0;

        public float m_RumbleTime = 0.0f;
        public bool m_IsConnected = false;
        public bool m_RumbleHasEnded = false;

        public short m_LeftAnalogX = 0;
        public short m_LeftAnalogY = 0;

        public short m_RightAnalogX = 0;
        public short m_RightAnalogY = 0;

        public short m_LeftTrigger = 0;
        public short m_RightTrigger = 0;

        public ButtonState m_LeftAnalogLeft = ButtonState.Released;
        public ButtonState m_LeftAnalogRight = ButtonState.Released;
        public ButtonState m_LeftAnalogUp = ButtonState.Released;
        public ButtonState m_LeftAnalogDown = ButtonState.Released;

        public ButtonState m_RightAnalogLeft = ButtonState.Released;
        public ButtonState m_RightAnalogRight = ButtonState.Released;
        public ButtonState m_RightAnalogUp = ButtonState.Released;
        public ButtonState m_RightAnalogDown = ButtonState.Released;

        public ButtonState m_LeftBumper = ButtonState.Released;
        public ButtonState m_RightBumper = ButtonState.Released;

        public ButtonState m_LeftTriggerState = ButtonState.Released;
        public ButtonState m_RightTriggerState = ButtonState.Released;

        public ButtonState m_A = ButtonState.Released;
        public ButtonState m_B = ButtonState.Released;
        public ButtonState m_X = ButtonState.Released;
        public ButtonState m_Y = ButtonState.Released;

        public ButtonState m_DigitalUp = ButtonState.Released;
        public ButtonState m_DigitalDown = ButtonState.Released;
        public ButtonState m_DigitalLeft = ButtonState.Released;
        public ButtonState m_DigitalRight = ButtonState.Released;

        public ButtonState m_Start = ButtonState.Released;
        public ButtonState m_Back = ButtonState.Released;

        public ButtonState m_LeftThumb = ButtonState.Released;
        public ButtonState m_RightThumb = ButtonState.Released;

        public XboxController(int a_ControllerIndex, byte a_DeadzoneTrigger, short a_DeadzoneLeftAnalog, short a_DeadzoneRightTrigger)
        {
            m_Index = a_ControllerIndex;
            m_DeadzoneTrigger = a_DeadzoneTrigger;
            m_DeadzoneLeftAnalog = a_DeadzoneLeftAnalog;
            m_DeadzoneRightTrigger = a_DeadzoneRightTrigger;
        }

        public void Update()
        {
            CheckConnection();

            if (m_IsConnected == true)
            {
                XInput.XInputState inputState;
                XInput.XInputGetState(m_Index, out inputState);

                // Left Shoulder
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.LeftShoulder))
                {
                    if (m_LeftBumper == ButtonState.Released) m_LeftBumper = ButtonState.Down;
                    else m_LeftBumper = ButtonState.Pressed;
                }
                else
                {
                    if (m_LeftBumper == ButtonState.Pressed) m_LeftBumper = ButtonState.Up;
                    else m_LeftBumper = ButtonState.Released;
                }

                // Right Shoulder
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.RightShoulder))
                {
                    if (m_RightBumper == ButtonState.Released) m_RightBumper = ButtonState.Down;
                    else m_RightBumper = ButtonState.Pressed;
                }
                else
                {
                    if (m_RightBumper == ButtonState.Pressed) m_RightBumper = ButtonState.Up;
                    else m_RightBumper = ButtonState.Released;
                }

                // A
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.A))
                {
                    if (m_A == ButtonState.Released) m_A = ButtonState.Down;
                    else m_A = ButtonState.Pressed;
                }
                else
                {
                    if (m_A == ButtonState.Pressed) m_A = ButtonState.Up;
                    else m_A = ButtonState.Released;
                }

                // B
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.B))
                {
                    if (m_B == ButtonState.Released) m_B = ButtonState.Down;
                    else m_B = ButtonState.Pressed;
                }
                else
                {
                    if (m_B == ButtonState.Pressed) m_B = ButtonState.Up;
                    else m_B = ButtonState.Released;
                }

                // X
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.X))
                {
                    if (m_X == ButtonState.Released) m_X = ButtonState.Down;
                    else m_X = ButtonState.Pressed;
                }
                else
                {
                    if (m_X == ButtonState.Pressed) m_X = ButtonState.Up;
                    else m_X = ButtonState.Released;
                }

                // Y
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.Y))
                {
                    if (m_Y == ButtonState.Released) m_Y = ButtonState.Down;
                    else m_Y = ButtonState.Pressed;
                }
                else
                {
                    if (m_Y == ButtonState.Pressed) m_Y = ButtonState.Up;
                    else m_Y = ButtonState.Released;
                }

                // Start
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.Start))
                {
                    if (m_Start == ButtonState.Released) m_Start = ButtonState.Down;
                    else m_Start = ButtonState.Pressed;
                }
                else
                {
                    if (m_Start == ButtonState.Pressed) m_Start = ButtonState.Up;
                    else m_Start = ButtonState.Released;
                }

                // Back
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.Back))
                {
                    if (m_Back == ButtonState.Released) m_Back = ButtonState.Down;
                    else m_Back = ButtonState.Pressed;
                }
                else
                {
                    if (m_Back == ButtonState.Pressed) m_Back = ButtonState.Up;
                    else m_Back = ButtonState.Released;
                }

                // Left Thumb
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.LeftThumb))
                {
                    if (m_LeftThumb == ButtonState.Released) m_LeftThumb = ButtonState.Down;
                    else m_LeftThumb = ButtonState.Pressed;
                }
                else
                {
                    if (m_LeftThumb == ButtonState.Pressed) m_LeftThumb = ButtonState.Up;
                    else m_LeftThumb = ButtonState.Released;
                }

                // Right Thumb
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.RightThumb))
                {
                    if (m_RightThumb == ButtonState.Released) m_RightThumb = ButtonState.Down;
                    else m_RightThumb = ButtonState.Pressed;
                }
                else
                {
                    if (m_RightThumb == ButtonState.Pressed) m_RightThumb = ButtonState.Up;
                    else m_RightThumb = ButtonState.Released;
                }

                // Digital Up
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.DPadUp))
                {
                    if (m_DigitalUp == ButtonState.Released) m_DigitalUp = ButtonState.Down;
                    else m_DigitalUp = ButtonState.Pressed;
                }
                else
                {
                    if (m_DigitalUp == ButtonState.Pressed) m_DigitalUp = ButtonState.Up;
                    else m_DigitalUp = ButtonState.Released;
                }

                // Digital Down
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.DPadDown))
                {
                    if (m_DigitalDown == ButtonState.Released) m_DigitalDown = ButtonState.Down;
                    else m_DigitalDown = ButtonState.Pressed;
                }
                else
                {
                    if (m_DigitalDown == ButtonState.Pressed) m_DigitalDown = ButtonState.Up;
                    else m_DigitalDown = ButtonState.Released;
                }

                // Digital Left
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.DPadLeft))
                {
                    if (m_DigitalLeft == ButtonState.Released) m_DigitalLeft = ButtonState.Down;
                    else m_DigitalLeft = ButtonState.Pressed;
                }
                else
                {
                    if (m_DigitalLeft == ButtonState.Pressed) m_DigitalLeft = ButtonState.Up;
                    else m_DigitalLeft = ButtonState.Released;
                }

                // Digital Right
                if (inputState.Gamepad.IsButtonDown(XInput.XInputButtons.DPadRight))
                {
                    if (m_DigitalRight == ButtonState.Released) m_DigitalRight = ButtonState.Down;
                    else m_DigitalRight = ButtonState.Pressed;
                }
                else
                {
                    if (m_DigitalRight == ButtonState.Pressed) m_DigitalRight = ButtonState.Up;
                    else m_DigitalRight = ButtonState.Released;
                }

                // Left Trigger
                if (inputState.Gamepad.LeftTrigger > m_DeadzoneTrigger)
                {
                    if (m_LeftTriggerState == ButtonState.Released) m_LeftTriggerState = ButtonState.Down;
                    else m_LeftTriggerState = ButtonState.Pressed;
                }
                else
                {
                    if (m_LeftTriggerState == ButtonState.Pressed) m_LeftTriggerState = ButtonState.Up;
                    else m_LeftTriggerState = ButtonState.Released;
                }

                // Right Trigger
                if (inputState.Gamepad.RightTrigger > m_DeadzoneTrigger)
                {
                    if (m_RightTriggerState == ButtonState.Released) m_RightTriggerState = ButtonState.Down;
                    else m_RightTriggerState = ButtonState.Pressed;
                }
                else
                {
                    if (m_RightTriggerState == ButtonState.Pressed) m_RightTriggerState = ButtonState.Up;
                    else m_RightTriggerState = ButtonState.Released;
                }

                // Left Analog Left
                if (-inputState.Gamepad.LeftThumbX > m_DeadzoneLeftAnalog)
                {
                    if (m_LeftAnalogLeft == ButtonState.Released) m_LeftAnalogLeft = ButtonState.Down;
                    else m_LeftAnalogLeft = ButtonState.Pressed;
                }
                else
                {
                    if (m_LeftAnalogLeft == ButtonState.Pressed) m_LeftAnalogLeft = ButtonState.Up;
                    else m_LeftAnalogLeft = ButtonState.Released;
                }

                // Left Analog Right
                if (inputState.Gamepad.LeftThumbX > m_DeadzoneLeftAnalog)
                {
                    if (m_LeftAnalogRight == ButtonState.Released) m_LeftAnalogRight = ButtonState.Down;
                    else m_LeftAnalogRight = ButtonState.Pressed;
                }
                else
                {
                    if (m_LeftAnalogRight == ButtonState.Pressed) m_LeftAnalogRight = ButtonState.Up;
                    else m_LeftAnalogRight = ButtonState.Released;
                }

                // Left Analog Up
                if (inputState.Gamepad.LeftThumbY > m_DeadzoneLeftAnalog)
                {
                    if (m_LeftAnalogUp == ButtonState.Released) m_LeftAnalogUp = ButtonState.Down;
                    else m_LeftAnalogUp = ButtonState.Pressed;
                }
                else
                {
                    if (m_LeftAnalogUp == ButtonState.Pressed) m_LeftAnalogUp = ButtonState.Up;
                    else m_LeftAnalogUp = ButtonState.Released;
                }

                // Left Analog Down
                if (-inputState.Gamepad.LeftThumbY > m_DeadzoneLeftAnalog)
                {
                    if (m_LeftAnalogDown == ButtonState.Released) m_LeftAnalogDown = ButtonState.Down;
                    else m_LeftAnalogDown = ButtonState.Pressed;
                }
                else
                {
                    if (m_LeftAnalogDown == ButtonState.Pressed) m_LeftAnalogDown = ButtonState.Up;
                    else m_LeftAnalogDown = ButtonState.Released;
                }

                // Right Analog Left
                if (-inputState.Gamepad.RightThumbX > m_DeadzoneRightTrigger)
                {
                    if (m_RightAnalogLeft == ButtonState.Released) m_RightAnalogLeft = ButtonState.Down;
                    else m_RightAnalogLeft = ButtonState.Pressed;
                }
                else
                {
                    if (m_RightAnalogLeft == ButtonState.Pressed) m_RightAnalogLeft = ButtonState.Up;
                    else m_RightAnalogLeft = ButtonState.Released;
                }

                // Right Analog Right
                if (inputState.Gamepad.RightThumbX > m_DeadzoneRightTrigger)
                {
                    if (m_RightAnalogRight == ButtonState.Released) m_RightAnalogRight = ButtonState.Down;
                    else m_RightAnalogRight = ButtonState.Pressed;
                }
                else
                {
                    if (m_RightAnalogRight == ButtonState.Pressed) m_RightAnalogRight = ButtonState.Up;
                    else m_RightAnalogRight = ButtonState.Released;
                }

                // Right Analog Up
                if (inputState.Gamepad.RightThumbY > m_DeadzoneRightTrigger)
                {
                    if (m_RightAnalogUp == ButtonState.Released) m_RightAnalogUp = ButtonState.Down;
                    else m_RightAnalogUp = ButtonState.Pressed;
                }
                else
                {
                    if (m_RightAnalogUp == ButtonState.Pressed) m_RightAnalogUp = ButtonState.Up;
                    else m_RightAnalogUp = ButtonState.Released;
                }

                // Right Analog Down
                if (-inputState.Gamepad.RightThumbY > m_DeadzoneRightTrigger)
                {
                    if (m_RightAnalogDown == ButtonState.Released) m_RightAnalogDown = ButtonState.Down;
                    else m_RightAnalogDown = ButtonState.Pressed;
                }
                else
                {
                    if (m_RightAnalogDown == ButtonState.Pressed) m_RightAnalogDown = ButtonState.Up;
                    else m_RightAnalogDown = ButtonState.Released;
                }

                m_LeftAnalogX = inputState.Gamepad.LeftThumbX;
                m_LeftAnalogY = inputState.Gamepad.LeftThumbY;
                m_RightAnalogX = inputState.Gamepad.RightThumbX;
                m_RightAnalogY = inputState.Gamepad.RightThumbY;
                m_LeftTrigger = inputState.Gamepad.LeftTrigger;
                m_RightTrigger = inputState.Gamepad.RightTrigger;

                if (m_RumbleHasEnded == false)
                {
                    m_RumbleTime -= Time.deltaTime;

                    if (m_RumbleTime <= 0f)
                    {
                        XInput.XInputVibration xInputVibration = new XInput.XInputVibration(0, 0);
                        XInput.XInputSetState(m_Index, ref xInputVibration);
                    }
                }
            }
        }

        private void CheckConnection()
        {
            XInput.XInputState xInputState;
            m_IsConnected = (XInput.XInputGetState(m_Index, out xInputState) == CONNECTION_SUCCESS);
        }
    }

    // --------------------------------------------------------------------------------

    private static class XInput
    {
        const string DllName = "XInput9_1_0.dll";

        [DllImport(DllName)]
        public extern static uint XInputGetState(
            int index,  // [in] Index of the gamer associated with the device
            out XInputState state // [out] Receives the current state
        );

        [DllImport(DllName)]
        public extern static uint XInputSetState(
            int index,  // [in] Index of the gamer associated with the device
            ref XInputVibration vibration// [in, out] The vibration information to send to the controller
        );

        // -----------------------------------------------------------------------

        public enum XInputButtons : ushort
        {
            DPadUp = 0x1,
            DPadDown = 0x2,
            DPadLeft = 0x4,
            DPadRight = 0x8,
            Start = 0x10,
            Back = 0x20,
            LeftThumb = 0x40,
            RightThumb = 0x80,
            LeftShoulder = 0x100,
            RightShoulder = 0x200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000
        };

        // -----------------------------------------------------------------------

        public struct XInputVibration
        {
            public ushort LeftMotorSpeed;
            public ushort RightMotorSpeed;

            public XInputVibration(ushort left, ushort right)
            {
                this.LeftMotorSpeed = left;
                this.RightMotorSpeed = right;
            }
        }

        // -----------------------------------------------------------------------

        public struct XInputState
        {
            public uint PacketNumber;
            public XInputGamepad Gamepad;

            public bool Equals(XInputState rhs)
            {
                return PacketNumber == rhs.PacketNumber
                        && Gamepad.Equals(rhs.Gamepad);
            }

            public override bool Equals(object obj)
            {
                if (obj is XInputState)
                    return this.Equals((XInputState)obj);
                return false;
            }

            public override int GetHashCode()
            {
                return (int)PacketNumber;
            }
        }

        // -----------------------------------------------------------------------

        public struct XInputGamepad
        {
            public XInputButtons Buttons;
            public byte LeftTrigger;
            public byte RightTrigger;
            public short LeftThumbX;
            public short LeftThumbY;
            public short RightThumbX;
            public short RightThumbY;

            public bool IsButtonDown(XInputButtons button)
            {
                return (Buttons & button) != 0;
            }

            public override bool Equals(object obj)
            {
                if (obj is XInputGamepad)
                    return this.Equals((XInputGamepad)obj);
                return false;
            }

            public override int GetHashCode()
            {
                return (int)Buttons + LeftTrigger + RightTrigger;
            }

            public bool Equals(XInputGamepad rhs)
            {
                return
                    Buttons == rhs.Buttons
                    && LeftTrigger == rhs.LeftTrigger
                    && RightTrigger == rhs.RightTrigger
                    && LeftThumbX == rhs.LeftThumbX
                    && LeftThumbY == rhs.LeftThumbY
                    && RightThumbX == rhs.RightThumbX
                    && RightThumbY == rhs.RightThumbY;
            }
        }
    }
}

public enum ButtonState
{
    Down,
    Pressed,
    Up,
    Released
}