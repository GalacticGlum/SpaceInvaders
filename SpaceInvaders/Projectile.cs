/*
 * Author: Shon Verch
 * File Name: Projectile.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/09/2019
 * Modified Date: 02/10/2019
 * Description: DESCRIPTION
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    public class Projectile
    {
        [JsonProperty("type", Required = Required.Always)]
        public ProjectileType Type { get; private set; }

        [JsonProperty("velocity", Required = Required.Always)]
        public Vector2 Velocity { get; private set; }

        [JsonProperty("frames", Required = Required.Always)]
        public string[] FrameNames { get; private set; }

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

        public void Update(float deltaTime)
        {
            rectangle.Position += Velocity * deltaTime;
            HandleAnimation(deltaTime);

            // If the projectile exceeds the top vertical boundary, we need to destroy it.
            if (rectangle.Position.Y <= MainGame.TopVerticalBoundary)
            {
                MainGame.Context.ProjectileController.Remove(this);
            }

            // Check whether the projectile hit any of the boundaries
            // If so, we need to decrement the health of the boundary and
            // destroy this projectile.
            if (MainGame.Context.BarrierGroup.Intersects(rectangle, out BarrierHitResult hitResult))
            {
                hitResult.Tile.Health -= 1;
                MainGame.Context.ProjectileController.Remove(this);
            }
        }

        private void HandleAnimation(float deltaTime)
        {
            // There is no reason to handle animation logic
            // if there is only one frame.
            if (frameTextures.Length <= 1) return;

            animationTimer -= deltaTime;
            if (!(animationTimer <= 0)) return;

            frameCount = (frameCount + 1) % FrameNames.Length;
            rectangle.Width = frameTextures[frameCount].Width * MainGame.SpriteScaleFactor;
            rectangle.Height = frameTextures[frameCount].Height * MainGame.SpriteScaleFactor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(frameTextures[frameCount], rectangle.Position, null, Color.White, 0, Vector2.Zero, 
                MainGame.SpriteScaleFactor, SpriteEffects.None, 0);
        }
    }
}
