/*
 * Author: Shon Verch
 * File Name: TextureHelpers.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders.Helpers
{
    public static class TextureHelpers
    {
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
