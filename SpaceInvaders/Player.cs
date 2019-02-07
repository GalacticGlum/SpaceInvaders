/*
 * Author: Shon Verch
 * File Name: Player.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Helpers;

using MathHelper = Microsoft.Xna.Framework.MathHelper;

namespace SpaceInvaders
{
    public class Player
    {
        /// <summary>
        /// The horizontal speed in pixels per second.
        /// </summary>
        private const int HorizontalSpeed = 5;

        /// <summary>
        /// The amount of pixels to spawn above the horizontal boundary line.
        /// </summary>
        private const int VerticalSpawnOffset = 29;

        /// <summary>
        /// The maximum horizontal coordinate that the player cannot go beyond.
        /// </summary>
        private readonly float maxHorizontalCoordinate;

        private readonly Texture2D playerTexture;
        private Vector2 position;

        public Player(TextureAtlas textureAtlas)
        {
            playerTexture = textureAtlas["player"];

            float playerY = MainGame.HorizontalBoundaryY - playerTexture.Height * MainGame.SpriteScaleFactor - VerticalSpawnOffset;
            position = new Vector2(MainGame.GameScreenWidth * 0.25f, playerY);

            maxHorizontalCoordinate = MainGame.HorizontalBoundaryEnd.X - playerTexture.Width * MainGame.SpriteScaleFactor;
        }

        public void Update(float deltaTime)
        {
            float velocity = 0;
            if (Input.GetKey(Keys.A))
            {
                velocity = -1;
            }

            if (Input.GetKey(Keys.D))
            {
                velocity = 1;
            }

            position.X = MathHelper.Clamp(position.X + velocity * HorizontalSpeed, MainGame.HorizontalBoundaryStart.X, maxHorizontalCoordinate);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, position, null, ColourHelpers.PureGreen, 0, Vector2.Zero, MainGame.SpriteScaleFactor, SpriteEffects.None, 0);
        }
    }
}
