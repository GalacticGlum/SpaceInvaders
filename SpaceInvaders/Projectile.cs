/*
 * Author: Shon Verch
 * File Name: Projectile.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/09/2019
 * Modified Date: 02/24/2019
 * Description: A single projectile instance.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    /// <summary>
    /// A single projectile instance.
    /// </summary>
    public class Projectile
    {
        /// <summary>
        /// The type of this <see cref="Projectile"/>.
        /// </summary>
        [JsonProperty("type", Required = Required.Always)]
        public ProjectileType Type { get; private set; }

        /// <summary>
        /// The velocity of this <see cref="Projectile"/>.
        /// </summary>
        [JsonProperty("velocity", Required = Required.Always)]
        public Vector2 Velocity { get; private set; }

        /// <summary>
        /// The texture atlas names of the frames to animate this <see cref="Projectile"/>.
        /// </summary>
        [JsonProperty("frames", Required = Required.Always)]
        public string[] FrameNames { get; private set; }

        /// <summary>
        /// The animation rate of this <see cref="Projectile"/>.
        /// </summary>
        [JsonProperty("animation_rate", Required = Required.Always)]
        public float AnimationRate { get; private set; }

        private readonly Texture2D[] frameTextures;

        private RectangleF rectangle;
        private float animationTimer;
        private int frameCount;

        /// <summary>
        /// A parameterless constructor required for JSON serialization.
        /// </summary>
        [JsonConstructor]
        private Projectile() { }

        /// <summary>
        /// Initializes a new <see cref="Projectile"/> from an instance and a position.
        /// </summary>
        /// <param name="prototype">The <see cref="Projectile"/> instance to clone.</param>
        /// <param name="position">The position of the new projectile.</param>
        public Projectile(Projectile prototype, Vector2 position)
        {
            Type = prototype.Type;
            Velocity = prototype.Velocity;
            FrameNames = prototype.FrameNames;
            AnimationRate = prototype.AnimationRate;

            frameCount = 0;
            animationTimer = AnimationRate;

            // Preload our textures so that we don't have to retrieve
            // the texture from our atlas every draw call.
            frameTextures = new Texture2D[FrameNames.Length];
            for (int i = 0; i < FrameNames.Length; i++)
            {
                frameTextures[i] = MainGame.Context.MainTextureAtlas[FrameNames[i]];
            }

            rectangle = new RectangleF(position, new Vector2(frameTextures[0].Width, frameTextures[0].Height));
        }

        /// <summary>
        /// Update the projectile.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            rectangle.Position += Velocity * deltaTime;
            HandleAnimation(deltaTime);

            GameplayScreen gameplayScreen = MainGame.Context.GetGameScreen<GameplayScreen>(GameScreenType.Gameplay);

            // If the projectile exceeds the top or bottom vertical boundary, we need to destroy it.
            if (rectangle.Top <= GameplayScreen.TopVerticalBoundary || rectangle.Bottom > GameplayScreen.HorizontalBoundaryY)
            {
                gameplayScreen.ProjectileController.Remove(this);
            }

            // Check whether the projectile hit any of the boundaries
            // If so, we need to decrement the health of the boundary and
            // destroy this projectile.
            if (gameplayScreen.BarrierGroup.Intersects(rectangle, out BarrierHitResult barrierHitResult))
            {
                barrierHitResult.Tile.Health -= 1;
                gameplayScreen.ProjectileController.Remove(this);
            }

            // Only projectiles fired by a certain entity can collide with other entities.
            // For example, only projectiles fired by the player can collide with enemies.
            switch (Type)
            {
                case ProjectileType.Player:
                    if (gameplayScreen.EnemyGroup.Intersects(rectangle, out Point enemyHitResult))
                    {
                        gameplayScreen.EnemyGroup.RemoveEnemy(enemyHitResult.X, enemyHitResult.Y);
                        gameplayScreen.ProjectileController.Remove(this);
                    }

                    if (gameplayScreen.UfoController.Intersects(rectangle))
                    {
                        gameplayScreen.UfoController.Destroy();
                        gameplayScreen.ProjectileController.Remove(this);
                    }

                    break;
                case ProjectileType.Enemy:

                    if (gameplayScreen.Player.Intersects(rectangle))
                    {
                        gameplayScreen.Player.Lives -= 1;
                        gameplayScreen.ProjectileController.Remove(this);
                    }

                    break;
            }
        }

        /// <summary>
        /// Animate the projectile.
        /// </summary>
        /// <param name="deltaTime"></param>
        private void HandleAnimation(float deltaTime)
        {
            // There is no reason to handle animation logic
            // if there is only one frame.
            if (frameTextures.Length <= 1) return;

            animationTimer -= deltaTime;
            if (!(animationTimer <= 0)) return;

            animationTimer = AnimationRate;
            frameCount = (frameCount + 1) % FrameNames.Length;
            rectangle.Width = frameTextures[frameCount].Width * MainGame.ResolutionScale;
            rectangle.Height = frameTextures[frameCount].Height * MainGame.ResolutionScale;
        }

        /// <summary>
        /// Render the projectile.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(frameTextures[frameCount], rectangle.Position, null, Color.White, 0, Vector2.Zero, 
                MainGame.ResolutionScale, SpriteEffects.None, 0);
        }
    }
}
