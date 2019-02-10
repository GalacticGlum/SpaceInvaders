/*
 * Author: Shon Verch
 * File Name: Input.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/06/2019
 * Description: The main input manager which provides an interface into keyboard and mouse input.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders.Engine
{
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    /// <summary>
    /// The main input manager which facilitates the primary logic for receiving keyboard and mouse input.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// The current position of the mouse in pixels coordinates.
        /// </summary>
        public static Vector2 MousePosition => currentMouseState.Position.ToVector2();

        private static KeyboardState currentKeyState;
        private static KeyboardState lastKeyState;

        private static MouseState currentMouseState;
        private static MouseState lastMouseState;

        /// <summary>
        /// Updates the keyboard and mouse state.
        /// </summary>
        public static void Update()
        {
            lastMouseState = currentMouseState;
            lastKeyState = currentKeyState;

            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();
        }

        /// <summary>
        /// Gets whether the user is holding down the key with the specified code.
        /// </summary>
        /// <param name="keyCode">The keycode to check.</param>
        /// <returns>Returns <value>true</value> if the user is holding down the key; otherwise, <value>false</value>.</returns>
        public static bool GetKey(Keys keyCode) => currentKeyState.IsKeyDown(keyCode);

        /// <summary>
        /// Gets whether the user pressed down the key with the specified code in the current frame.
        /// </summary>
        /// <param name="keyCode">The keycode to check.</param>
        /// <returns>Returns <value>true</value> if the user pressed down the key in this frame; otherwise, <value>false</value>.</returns>
        public static bool GetKeyDown(Keys keyCode) => lastKeyState.IsKeyUp(keyCode) && currentKeyState.IsKeyDown(keyCode);

        /// <summary>
        /// Gets whether the user released the key with the specified code in the current frame.
        /// </summary>
        /// <param name="keyCode">The keycode to check.</param>
        /// <returns>Returns <value>true</value> if the user released the key in this frame; otherwise, <value>false</value>.</returns>
        public static bool GetKeyUp(Keys keyCode) => lastKeyState.IsKeyDown(keyCode) && currentKeyState.IsKeyUp(keyCode);

        /// <summary>
        /// Gets whether the user is pressing down the specified mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to check.</param>
        /// <returns>Returns <value>true</value> if the user is pressing down the mouse button; otherwise, <value>false</value>.</returns>
        public static bool GetMouseButton(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    if (currentMouseState.LeftButton == ButtonState.Pressed) return true;
                    break;
                case MouseButton.Right:
                    if (currentMouseState.RightButton == ButtonState.Pressed) return true;
                    break;
                case MouseButton.Middle:
                    if (currentMouseState.MiddleButton == ButtonState.Pressed) return true;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets whether the user pressed down the specified mouse button in the current frame.
        /// </summary>
        /// <param name="mouseButton">The mouse button to check.</param>
        /// <returns>Returns <value>true</value> if the user pressed down the mouse button in this frame; otherwise, <value>false</value>.</returns>
        public static bool GetMouseButtonDown(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        return true;
                    }

                    break;
                case MouseButton.Right:
                    if (lastMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed)
                    {
                        return true;
                    }

                    break;
                case MouseButton.Middle:
                    if (lastMouseState.MiddleButton == ButtonState.Released && currentMouseState.MiddleButton == ButtonState.Pressed)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets whether the user released the specified mouse button in the current frame.
        /// </summary>
        /// <param name="mouseButton">The mouse button to check.</param>
        /// <returns>Returns <value>true</value> if the user released the mouse button in this frame; otherwise, <value>false</value>.</returns>
        public static bool GetMouseButtonUp(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    if (lastMouseState.LeftButton == ButtonState.Pressed &&
                        currentMouseState.LeftButton == ButtonState.Released)
                    {
                        return true;
                    }

                    break;
                case MouseButton.Right:
                    if (lastMouseState.RightButton == ButtonState.Pressed &&
                        currentMouseState.RightButton == ButtonState.Released)
                    {
                        return true;
                    }

                    break;
                case MouseButton.Middle:
                    if (lastMouseState.MiddleButton == ButtonState.Pressed &&
                        currentMouseState.MiddleButton == ButtonState.Released)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }
    }
}
