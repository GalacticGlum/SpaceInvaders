/*
 * Author: Shon Verch
 * File Name: BarrierTile.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/06/2019
 * Modified Date: 02/24/2019
 * Description: A single tile of a Barrier.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    /// <summary>
    /// A single tile of a <see cref="Barrier"/>.
    /// </summary>
    public class BarrierTile
    {
        /// <summary>
        /// The maximum amount of hits that a <see cref="BarrierTile"/> can sustain before it is destroyed.
        /// </summary>
        public const int MaxHealth = 4;

        /// <summary>
        /// The suffix of the sprite for this <see cref="BarrierTile"/>.
        /// </summary>
        public string SpriteSuffix { get; }

        /// <summary>
        /// The remaining health of this <see cref="BarrierTile"/>.
        /// </summary>
        public int Health
        {
            get => health;
            set
            {
                if (value == health) return;
                health = value;

                if (health <= 0) return;

                // Update the tile sprite to the next one in the sequence.
                Texture = MainGame.Context.MainTextureAtlas[TextureName];
                RecalculateRectangle();
            }
        }

        /// <summary>
        /// Indicates whether this <see cref="BarrierTile"/> is active.
        /// <remarks>
        /// An active <see cref="BarrierTile"/> is one who has a non-negative, non-zero health.
        /// </remarks>
        /// </summary>
        public bool Active => Health > 0;

        /// <summary>
        /// The name of the <see cref="Texture2D"/> for this <see cref="BarrierTile"/>.
        /// </summary>
        public string TextureName => $"barrier_{SpriteSuffix}_{MaxHealth - health + 1}";

        public Texture2D Texture { get; private set; }

        /// <summary>
        /// The rectangle of this <see cref="BarrierTile"/> relative to the <see cref="Barrier"/> grid.
        /// </summary>
        public RectangleF LocalRectangle { get; private set; }

        /// <summary>
        /// The position of this <see cref="BarrierTile"/> in the parent <see cref="Barrier"/> tile grid.
        /// </summary>
        private readonly Vector2 gridPosition;
        private int health;

        /// <summary>
        /// Initializes a new <see cref="BarrierTile"/>.
        /// </summary>
        /// <param name="gridPosition">The position of this <see cref="BarrierTile"/> in the parent <see cref="Barrier"/> tile grid.</param>
        /// <param name="spriteSuffix">The suffix of this <see cref="BarrierTile"/> sprite(s).</param>
        public BarrierTile(Vector2 gridPosition, string spriteSuffix)
        {
            this.gridPosition = gridPosition;

            SpriteSuffix = spriteSuffix;
            health = MaxHealth;

            Texture = MainGame.Context.MainTextureAtlas[TextureName];
            RecalculateRectangle();
        }

        /// <summary>
        /// Renders this <see cref="BarrierTile"/>.
        /// </summary>
        /// <param name="barrierPosition">The position of the parent <see cref="Barrier"/>.</param>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> context.</param>
        public void Draw(Vector2 barrierPosition, SpriteBatch spriteBatch)
        {
            if (!Active) return;

            spriteBatch.Draw(Texture, barrierPosition + LocalRectangle.Position, null, ColourHelpers.PureGreen, 0,
                Vector2.Zero, MainGame.ResolutionScale, SpriteEffects.None, 0.6f);
        }

        /// <summary>
        /// Recalculates the bounding rectangle of this <see cref="BarrierTile"/>.
        /// </summary>
        private void RecalculateRectangle()
        {
            float textureWidth = Texture.Width * MainGame.ResolutionScale;
            float textureHeight = Texture.Height * MainGame.ResolutionScale;
            float x = gridPosition.X * textureWidth;
            float y = gridPosition.Y * textureHeight;

            LocalRectangle = new RectangleF(x, y, textureWidth, textureHeight);
        }

        /// <summary>
        /// Gets the rectangle of this <see cref="BarrierTile"/> in world space.
        /// </summary>
        /// <param name="barrierPosition">The position of the <see cref="Barrier"/> which this <see cref="BarrierTile"/> belongs to.</param>
        /// <returns>The world rectangle of this <see cref="BarrierTile"/>.</returns>
        public RectangleF GetWorldRectangle(Vector2 barrierPosition) =>
            new RectangleF(LocalRectangle.Position + barrierPosition, LocalRectangle.Size);
    }
}
