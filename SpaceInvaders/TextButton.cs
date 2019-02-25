/*
 * Author: Shon Verch
 * File Name: TextButton.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: DESCRIPTION
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    public class TextButton
    {
        public string Text { get; }
        public SpriteFont Font { get; }
        public Color RegularColour { get; set; } = Color.White;
        public Color HoverColour { get; set; } = Color.White;

        public RectangleF Rectangle;
        public event Action Clicked;

        private Color currentColour;

        public TextButton(Vector2 position, SpriteFont font, string text)
        {;
            Text = text;
            Font = font;

            Vector2 size = new Vector2(Font.MeasureString(text).X, Font.LineSpacing);
            Rectangle = new RectangleF(position, size);
            currentColour = RegularColour;
        }

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

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Rectangle.Position, currentColour);
        }
    }
}
