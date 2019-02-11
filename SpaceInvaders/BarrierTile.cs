/*
 * Author: Shon Verch
 * File Name: BarrierTile.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/06/2019
 * Modified Date: 02/10/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    public class BarrierTile
    {
        public const int MaxHealth = 4;

        public string SpriteSuffix { get; }

        private int health;
        public int Health
        {
            get => health;
            set
            {
                if (value == health) return;
                health = value;

                if (health <= 0) return;
                Texture = MainGame.Context.MainTextureAtlas[TextureName];
                RecalculateRectangle();
            }
        }

        public bool Active => Health > 0;
        public string TextureName => $"barrier_{SpriteSuffix}_{MaxHealth - health + 1}";

        public Texture2D Texture { get; private set; }

        /// <summary>
        /// The rectangle of this <see cref="BarrierTile"/> relative to the <see cref="Barrier"/> grid.
        /// </summary>
        public RectangleF LocalRectangle { get; private set; }

        private readonly Vector2 gridPosition;

        public BarrierTile(Vector2 gridPosition, string spriteSuffix)
        {
            this.gridPosition = gridPosition;

            SpriteSuffix = spriteSuffix;
            health = MaxHealth;

            Texture = MainGame.Context.MainTextureAtlas[TextureName];
            RecalculateRectangle();
        }

        public void Draw(Vector2 barrierPosition, SpriteBatch spriteBatch)
        {
            if (!Active) return;

            spriteBatch.Draw(Texture, barrierPosition + LocalRectangle.Position, null, ColourHelpers.PureGreen, 0,
                Vector2.Zero, MainGame.ResolutionScale, SpriteEffects.None, 0.6f);

            // Debug drawing
            spriteBatch.DrawBorder(GetWorldRectangle(barrierPosition), Color.Blue, 2, 0.8f);
        }

        private void RecalculateRectangle()
        {
            float textureWidth = Texture.Width * MainGame.ResolutionScale;
            float textureHeight = Texture.Height * MainGame.ResolutionScale;
            float x = gridPosition.X * textureWidth;
            float y = gridPosition.Y * textureHeight;
            LocalRectangle = new RectangleF(x, y, textureWidth, textureHeight);
        }

        /// <summary>
        /// Retrieve the rectangle of this <see cref="BarrierTile"/> in world space.
        /// </summary>
        /// <param name="barrierPosition">The position of the <see cref="Barrier"/> which this <see cref="BarrierTile"/> belongs to.</param>
        /// <returns>The world rectangle of this <see cref="BarrierTile"/>.</returns>
        public RectangleF GetWorldRectangle(Vector2 barrierPosition) =>
            new RectangleF(LocalRectangle.Position + barrierPosition, LocalRectangle.Size);
    }
}
