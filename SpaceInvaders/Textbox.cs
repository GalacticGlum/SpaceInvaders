/*
 * Author: Shon Verch
 * File Name: Textbox.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: DESCRIPTION
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    public class Textbox
    {
        private const string Caret = "_";
        private const float CaretFlashTime = 0.50f;

        public int MaxCharacters { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Color Colour { get; set; } = Color.White;
        public bool Focused { get; set; }

        public RectangleF Rectangle;

        private string currentCaret;
        private float caretTimer;

        public Textbox(Vector2 position, SpriteFont font, int maxCharacters = 16, string defaultText = null)
        {
            MaxCharacters = maxCharacters;
            Font = font;
            Text = string.IsNullOrEmpty(defaultText) ? string.Empty : defaultText;

            Rectangle = new RectangleF(position, font.MeasureString(Caret.Multiply(MaxCharacters)));
            caretTimer = CaretFlashTime;

            MainGame.Context.Window.TextInput += OnTextInput;
        }

        private void OnTextInput(object sender, TextInputEventArgs e)
        {
            if (!Focused) return;
            if (e.Key == Keys.Back)
            {
                if (Text.Length > 0)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
            }
            else
            {
                if (Text.Length >= MaxCharacters) return;
                if (char.IsLetterOrDigit(e.Character) || char.IsWhiteSpace(e.Character)) 
                {
                    Text += e.Character;
                }
            }
        }

        public void Update(float deltaTime)
        {
            if (!Focused)
            {
                currentCaret = string.Empty;
            }

            if (Input.GetMouseButtonDown(MouseButton.Left))
            {
                Focused = Rectangle.Contains(Input.MousePosition);
            }

            if (!Focused) return;
            caretTimer -= deltaTime;
            if (!(caretTimer <= 0)) return;

            caretTimer = CaretFlashTime;
            currentCaret = currentCaret == Caret ? string.Empty : Caret;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text + currentCaret, Rectangle.Position, Colour);
        }
    }
}
