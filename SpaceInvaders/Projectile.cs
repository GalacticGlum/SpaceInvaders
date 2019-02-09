using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [JsonIgnore]
        public Vector2 Position { get; private set; }

        private readonly Texture2D[] frameTextures;

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
            Position = position;
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
        }

        public void Update(float deltaTime)
        {
            // If the projectile exceeds the top vertical boundary, we need to destroy it.
            if (Position.Y <= MainGame.TopVerticalBoundary)
            {
                MainGame.Context.ProjectileController.Remove(this);
            }

            Position += Velocity * deltaTime;

            animationTimer -= deltaTime;
            if (animationTimer <= 0)
            {
                frameCount = (frameCount + 1) % FrameNames.Length;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(frameTextures[frameCount], Position, null, Color.White, 0, Vector2.Zero, 
                MainGame.SpriteScaleFactor, SpriteEffects.None, 0);
        }
    }
}
