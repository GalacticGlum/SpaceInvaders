/*
 * Author: Shon Verch
 * File Name: TextButton.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: A basic text-based button UI element.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    /// <summary>
    /// A basic text-based button UI element.
    /// </summary>
    public class TextButton
    {
        /// <summary>
        /// The text of this <see cref="TextButton"/>.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The <see cref="SpriteFont"/> to render this <see cref="TextButton"/>.
        /// </summary>
        public SpriteFont Font { get; }

        /// <summary>
        /// The colour of the text when the mouse button is not hovering over this <see cref="TextButton"/>.

        /// </summary>
        public Color RegularColour { get; set; } = Color.White;

        /// <summary>
        /// The colour of the text when the mouse button is hovering over this <see cref="TextButton"/>.
        /// </summary>
        public Color HoverColour { get; set; } = Color.White;

        /// <summary>
        /// The bounding rectangle of this <see cref="TextButton"/>.
        /// </summary>
        public RectangleF Rectangle;

        /// <summary>
        /// The <see cref="TextButton"/> clicked calllback.
        /// Invokved when the mouse button is over this <see cref="TextButton"/> and the left mouse button is pressed.
        /// </summary>
        public event Action Clicked;

        private Color currentColour;

        /// <summary>
        /// Initializes a new <see cref="TextButton"/>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="font">The <see cref="SpriteFont"/>.</param>
        /// <param name="text">The text.</param>
        public TextButton(Vector2 position, SpriteFont font, string text)
        {;
            Text = text;
            Font = font;

            Vector2 size = new Vector2(Font.MeasureString(text).X, Font.LineSpacing);
            Rectangle = new RectangleF(position, size);
            currentColour = RegularColour;
        }

        /// <summary>
        /// Update this <see cref="TextButton"/>.
        /// </summary>
        public void Update()
        {
            if (Rectangle.Contains(Input.MousePosition))
            {
                currentColour = HoverColour;
                if (Input.GetMouseButtonDown(MouseButton.Left))
                {
                    Clicked?.Invoke();
                }
            }
            else
            {
                currentColour = RegularColour;
            }
        }

        /// <summary>
        /// Render this <see cref="TextButton"/>.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="layerDepth"></param>
        public void Draw(SpriteBatch spriteBatch, float layerDepth = 0)
        {
            spriteBatch.DrawString(Font, Text, Rectangle.Position, currentColour, 0, Vector2.Zero, Vector2.One,
                SpriteEffects.None, layerDepth);
        }
    }
}
