/*
 * Author: Shon Verch
 * File Name: PrimitiveDrawingHelper.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/10/2019
 * Description: A collection of utilties related to drawing primitives.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders.Engine
{
    /// <summary>
    /// A collection of utilties related to drawing primitivess.
    /// </summary>
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
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
        /// <param name="start">The starting point of the line.</param>
        /// <param name="end">The ending point of the line.</param>
        /// <param name="tint">The colour of the line to be drawn. If this value is null, the line will be white.</param>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, 
            Color? tint = null, float thickness = 1, float layerDepth = 0)
        {
            float length = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            DrawLine(spriteBatch, start, length, angle, tint, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a line with a specific length and angle from a point.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
        /// <param name="origin">The starting point of the line.</param>
        /// <param name="length">The length of the line.</param>
        /// <param name="angle">The angle of the line from the horizontal.</param>
        /// <param name="tint">The colour of the line to be drawn. If this value is null, the line will be white.</param>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 origin, float length, float angle,
            Color? tint = null, float thickness = 1, float layerDepth = 0)
        {
            if (tint == null)
            {
                tint = Color.White;
            }

            VerifyPixelTexture(spriteBatch);

            Vector2 drawOrigin = new Vector2(0, 0.5f);
            Vector2 scale = new Vector2(length, thickness);
            spriteBatch.Draw(pixelTexture, origin, null, tint.Value, angle, drawOrigin, scale, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draws a border using the position and size of the specified <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
        /// <param name="rectangle">The <see cref="Rectangle"/>.</param>
        /// <param name="tint">The colour of the border to be drawn. If this value is null, the border will be white.</param>
        /// <param name="thickness">The thickness of the border.</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawBorder(this SpriteBatch spriteBatch, Rectangle rectangle, Color? tint = null,
            float thickness = 1, float layerDepth = 0)
        {
            // Draw top edge
            spriteBatch.DrawLine(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top), tint, thickness, layerDepth);

            // Draw left edge
            spriteBatch.DrawLine(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Left, rectangle.Bottom), tint, thickness, layerDepth);

            // Draw right edge
            spriteBatch.DrawLine(new Vector2(rectangle.Right, rectangle.Top), new Vector2(rectangle.Right, rectangle.Bottom), tint, thickness, layerDepth);

            // Draw bottom edge
            spriteBatch.DrawLine(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Bottom), tint, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a border using the position and size of the specified <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
        /// <param name="rectangle">The <see cref="RectangleF"/>.</param>
        /// <param name="tint">The colour of the border to be drawn. If this value is null, the border will be white.</param>
        /// <param name="thickness">The thickness of the border.</param>
        /// <param name="layerDepth">The layer depth.</param>
        public static void DrawBorder(this SpriteBatch spriteBatch, RectangleF rectangle, Color? tint = null,
            float thickness = 1, float layerDepth = 0)
        {
            // Draw top edge
            spriteBatch.DrawLine(rectangle.TopLeft, rectangle.TopRight, tint, thickness, layerDepth);

            // Draw left edge
            spriteBatch.DrawLine(rectangle.TopLeft, rectangle.BottomLeft, tint, thickness, layerDepth);

            // Draw right edge
            spriteBatch.DrawLine(rectangle.TopRight, rectangle.BottomRight, tint, thickness, layerDepth);

            // Draw bottom edge
            spriteBatch.DrawLine(rectangle.BottomLeft, rectangle.BottomRight, tint, thickness, layerDepth);
        }
    }
}
