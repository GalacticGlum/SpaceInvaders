/*
 * Author: Shon Verch
 * File Name: Player.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/09/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Engine;
using MathHelper = Microsoft.Xna.Framework.MathHelper;

namespace SpaceInvaders
{
    public class Player
    {
        /// <summary>
        /// The amount of pixels to spawn above the horizontal boundary line.
        /// </summary>
        public const int VerticalSpawnOffset = 29;

        /// <summary>
        /// The horizontal speed in pixels per second.
        /// </summary>
        private const int HorizontalSpeed = 200;

        /// <summary>
        /// The position of this <see cref="Player"/>.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The texture of this <see cref="Player"/>.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// The maximum horizontal coordinate that the player cannot go beyond.
        /// </summary>
        private readonly float maxHorizontalCoordinate;

        public Player()
        {
            Texture = MainGame.Context.MainTextureAtlas["player"];

            float playerY = MainGame.HorizontalBoundaryY - Texture.Height * MainGame.ResolutionScale - VerticalSpawnOffset;
            Position = new Vector2(MainGame.GameScreenWidth * 0.25f, playerY);

            maxHorizontalCoordinate = MainGame.HorizontalBoundaryEnd.X - Texture.Width * MainGame.ResolutionScale;
        }

        public void Update(float deltaTime)
        {
            HandleMovement(deltaTime);

            if (Input.GetKey(Keys.Space))
            {
                MainGame.Context.ProjectileController.CreatePlayerProjectile();
            }
        }

        private void HandleMovement(float deltaTime)
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

            float newX = MathHelper.Clamp(Position.X + velocity * HorizontalSpeed * deltaTime, MainGame.HorizontalBoundaryStart.X, maxHorizontalCoordinate);
            Position = new Vector2(newX, Position.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, ColourHelpers.PureGreen, 0, Vector2.Zero, MainGame.ResolutionScale, SpriteEffects.None, 0.5f);
        }
    }
}
