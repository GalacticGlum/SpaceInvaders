/*
 * Author: Shon Verch
 * File Name: TextureHelpers.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: A collection of texture related utilities.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders.Engine
{
    /// <summary>
    /// A collection of texture related utilities.
    /// </summary>
    public static class TextureHelpers
    {
        /// <summary>
        /// Get a specific area of a <see cref="Texture2D"/>.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="source">The rectangle area to extract.</param>
        /// <param name="graphicsDevice">The graphics devcice.</param>
        /// <returns>
        /// A <see cref="Texture2D"/> that is the same size as the specified rectangle
        /// and that contains the data contained in the specified rectangle on the texture.
        /// </returns>
        public static Texture2D GetCroppedTexture(Texture2D texture, Rectangle source, GraphicsDevice graphicsDevice)
        {
            Texture2D result = new Texture2D(graphicsDevice, source.Width, source.Height);
            Color[] pixels = new Color[source.Width * source.Height];
            texture.GetData(0, source, pixels, 0, pixels.Length);
            result.SetData(pixels);

            return result;
        }
    }
}
