/*
 * Author: Shon Verch
 * File Name: Input.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: The main input manager which facilitates the primary logic for receiving keyboard and mouse input.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders
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
        public static Vector2 MousePosition { get; private set; }

        private static KeyboardState currentKeyState;
        private static KeyboardState lastKeyState;

        private static MouseState currentMouseState;
        private static MouseState lastMouseState;

        public static void Update()
        {
            lastMouseState = currentMouseState;
            lastKeyState = currentKeyState;

            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();

            MousePosition = currentMouseState.Position.ToVector2();
        }

        public static bool GetKey(Keys keyCode) => currentKeyState.IsKeyDown(keyCode);
        public static bool GetKeyDown(Keys keyCode) => lastKeyState.IsKeyUp(keyCode) && currentKeyState.IsKeyDown(keyCode);
        public static bool GetKeyUp(Keys keyCode) => lastKeyState.IsKeyDown(keyCode) && currentKeyState.IsKeyUp(keyCode);

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
