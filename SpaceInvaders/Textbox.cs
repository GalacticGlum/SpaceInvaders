/*
 * Author: Shon Verch
 * File Name: Textbox.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: A textbox UI element that allows for basic text input.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    /// <summary>
    /// A textbox UI element that allows for basic text input.
    /// </summary>
    public class Textbox
    {
        /// <summary>
        /// The caret symbol.
        /// </summary>
        private const string Caret = "_";

        /// <summary>
        /// The amount of time, in seconds, the caret symbol flashes for.
        /// </summary>
        private const float CaretFlashTime = 0.50f;

        /// <summary>
        /// The max amount of characters that are allowed in this <see cref="Textbox"/>.
        /// </summary>
        public int MaxCharacters { get; set; }

        /// <summary>
        /// The <see cref="SpriteFont"/> to render this <see cref="Textbox"/>.
        /// </summary>
        public SpriteFont Font { get; set; }

        /// <summary>
        /// The current text of this <see cref="Textbox"/>.
        /// </summary>
        public string Text { get; set; }
       
        /// <summary>
        /// The colour of the text.
        /// </summary>
        public Color Colour { get; set; } = Color.White;

        /// <summary>
        /// Indicates whether this <see cref="Textbox"/> is focused.
        /// </summary>
        public bool Focused { get; set; }

        /// <summary>
        /// The bounding rectangle of this <see cref="Textbox"/>.
        /// </summary>
        public RectangleF Rectangle;

        private string currentCaret;
        private float caretTimer;

        /// <summary>
        /// Initializes a new <see cref="Textbox"/>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="font">The <see cref="SpriteFont"/>.</param>
        /// <param name="maxCharacters">The maximum amount of characters allowed in the input.</param>
        /// <param name="defaultText">The default text.</param>
        public Textbox(Vector2 position, SpriteFont font, int maxCharacters = 16, string defaultText = null)
        {
            MaxCharacters = maxCharacters;
            Font = font;
            Text = string.IsNullOrEmpty(defaultText) ? string.Empty : defaultText;

            Rectangle = new RectangleF(position, font.MeasureString(Caret.Multiply(MaxCharacters)));
            caretTimer = CaretFlashTime;

            MainGame.Context.Window.TextInput += OnTextInput;
        }

        /// <summary>
        /// Called when the game window receives text input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Update this <see cref="Textbox"/>.
        /// </summary>
        /// <param name="deltaTime"></param>
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

        /// <summary>
        /// Render this <see cref="Textbox"/>.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text + currentCaret, Rectangle.Position, Colour);
        }
    }
}
