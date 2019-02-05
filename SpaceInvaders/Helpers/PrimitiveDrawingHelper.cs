/*
 * Author: Shon Verch
 * File Name: PrimitiveDrawingHelper.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders.Helpers
{
    public static class PrimitiveDrawingHelper
    {
        /// <summary>
        /// A 1x1 texture consisting of a single white pixel.
        /// </summary>
        private static Texture2D pixelTexture;

        /// <summary>
        /// Creates the pixel texture if it is null.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> context.</param>
        private static void VerifyPixelTexture(SpriteBatch spriteBatch)
        {
            if (pixelTexture != null) return;

            pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixelTexture.SetData(new[]
            {
                Color.White
            });
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> context.</param>
        /// <param name="start">The starting point of the line.</param>
        /// <param name="end">The ending point of the line.</param>
        /// <param name="tint">The colour of the line to be drawn. If this value is null, the line will be white.</param>
        /// <param name="thickness">The thickness of the line.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color? tint = null, float thickness = 1)
        {
            float length = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            DrawLine(spriteBatch, start, length, angle, tint, thickness);
        }

        /// <summary>
        /// Draws a line with a specific length and angle from a point.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> context.</param>
        /// <param name="origin">The starting point of the line.</param>
        /// <param name="length">The length of the line.</param>
        /// <param name="angle">The angle of the line from the horizontal.</param>
        /// <param name="tint">The colour of the line to be drawn. If this value is null, the line will be white.</param>
        /// <param name="thickness">The thickness of the line.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 origin, float length, float angle,
            Color? tint = null, float thickness = 1)
        {
            if (tint == null)
            {
                tint = Color.White;
            }

            VerifyPixelTexture(spriteBatch);

            Vector2 drawOrigin = new Vector2(0, 0.5f);
            Vector2 scale = new Vector2(length, thickness);
            spriteBatch.Draw(pixelTexture, origin, null, tint.Value, angle, drawOrigin, scale, SpriteEffects.None, 0);
        }
    }
}
