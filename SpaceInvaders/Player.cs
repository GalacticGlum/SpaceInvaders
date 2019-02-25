/*
 * Author: Shon Verch
 * File Name: Player.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/24/2019
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
        /// The amount of lives that a <see cref="Player"/> starts with.
        /// </summary>
        public const int DefaultLives = 3;

        /// <summary>
        /// The maximum amount of lives that a <see cref="Player"/> can have.
        /// </summary>
        public const int MaxLives = 4;

        /// <summary>
        /// The horizontal speed in pixels per second.
        /// </summary>
        private const int HorizontalSpeed = 200;

        /// <summary>
        /// The amount of time, in seconds, that the death animation lasts for.
        /// </summary>
        private const float DeathAnimationDuration = 1;

        /// <summary>
        /// The animation rate of the death animation, in seconds.
        /// </summary>
        private const float DeathAnimationRate = 0.10f;

        /// <summary>
        /// The score of this <see cref="Player"/>.
        /// </summary>
        public int Score { get; set; } = 0;

        /// <summary>
        /// The number of lives that this <see cref="Player"/> has remaining.
        /// </summary>
        public int Lives
        {
            get => lives;
            set
            {
                if (value == lives) return;
                lives = value;

                MainGame.Context.Freeze(DeathAnimationDuration);
                isDeathAnimation = true;
                deathAnimationFrame = false;
                deathAnimationTimer = DeathAnimationRate;
            }
        }

        /// <summary>
        /// The position of this <see cref="Player"/>.
        /// </summary>
        public Vector2 Position => boundingRectangle.Position;

        /// <summary>
        /// The texture of this <see cref="Player"/>.
        /// </summary>
        public Texture2D Texture { get; }

        private readonly Vector2 startingPosition;

        /// <summary>
        /// The maximum horizontal coordinate that the player cannot go beyond.
        /// </summary>
        private readonly float maxHorizontalCoordinate;

        /// <summary>
        /// The bounding <see cref="RectangleF"/> of this <see cref="Player"/>.
        /// </summary>
        private RectangleF boundingRectangle;

        private int lives = DefaultLives;

        private bool isDeathAnimation;
        private bool deathAnimationFrame;
        private float deathAnimationTimer;

        public Player()
        {
            Texture = MainGame.Context.MainTextureAtlas["player"];
            float playerY = MainGame.HorizontalBoundaryY - Texture.Height * MainGame.ResolutionScale - VerticalSpawnOffset;

            startingPosition = new Vector2(MainGame.GameScreenWidth * 0.25f, playerY);
            boundingRectangle = new RectangleF(startingPosition.X, startingPosition.Y, Texture.Width, Texture.Height);
            maxHorizontalCoordinate = MainGame.HorizontalBoundaryEnd.X - Texture.Width * MainGame.ResolutionScale;
        }

        public void Update(float deltaTime)
        {
            if (isDeathAnimation)
            {
                deathAnimationTimer -= deltaTime;
                if (deathAnimationTimer <= 0)
                {
                    deathAnimationTimer = DeathAnimationRate;
                    deathAnimationFrame = !deathAnimationFrame;
                }
            }

            if (MainGame.Context.IsFrozen) return;
            if (isDeathAnimation)
            {
                isDeathAnimation = false;
                boundingRectangle.Position = startingPosition;
            }

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

            float newX = MathHelper.Clamp(boundingRectangle.X + velocity * HorizontalSpeed * deltaTime, MainGame.HorizontalBoundaryStart.X, maxHorizontalCoordinate);
            boundingRectangle.X = newX;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int frameIndex = deathAnimationFrame ? 2 : 1;
            Texture2D texture = isDeathAnimation ? MainGame.Context.MainTextureAtlas[$"player_death_{frameIndex}"] : Texture;
            spriteBatch.Draw(texture, boundingRectangle.Position, null, ColourHelpers.PureGreen, 0, Vector2.Zero, MainGame.ResolutionScale, SpriteEffects.None, 0.5f);
        }

        public bool Intersects(RectangleF rectangle) => boundingRectangle.Intersects(rectangle);
    }
}
