using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Helpers;

namespace SpaceInvaders
{
    public class EnemyGroup
    {
        /// <summary>
        /// The amount of pixels between each enemy cell.
        /// </summary>
        private const int Padding = 10;

        private const int GridWidth = 11;
        private const int GridHeight = 5;

        /// <summary>
        /// The layers of the enemy grid.
        /// The i-th element of this array indicates the type of all the enemies
        /// on the i-th row of the enemy grid.
        /// </summary>
        private static readonly EnemyType[] enemyTypeLayers =
        {
            EnemyType.Small,
            EnemyType.Medium,
            EnemyType.Medium,
            EnemyType.Big,
            EnemyType.Big
        };

        /// <summary>
        /// The width of the enemy with the largest width, in pixels.
        /// </summary>
        private readonly int largestEnemyWidth;

        private readonly float gridCellWidth;
        private readonly float gridCellHeight;

        /// <summary>
        /// The total width of the enemy group in pixels, including padding.
        /// </summary>
        private readonly float totalWidth;

        /// <summary>
        /// The total height of the enemy group in pixels, including padding.
        /// </summary>
        private readonly float totalHeight;

        private readonly TextureAtlas textureAtlas;

        /// <summary>
        /// A 2D array where the (x, y) element indicates whether
        /// the enemy at that position is dead (where true indicates
        /// that the enemy is NOT dead).
        /// </summary>
        private readonly Enemy[,] enemyGrid;
        private Vector2 position;

        public EnemyGroup(TextureAtlas textureAtlas)
        {
            this.textureAtlas = textureAtlas;

            enemyGrid = new Enemy[GridWidth, GridHeight];
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    enemyGrid[x, y] = new Enemy(new Vector2(x, y), enemyTypeLayers[y]);
                }
            }

            // We want the size of each grid cell to be the same so we need to find
            // the width of the largest texture; all other textures will be horizontally centered
            // in the grid cell according to the largest width. The height of all enemies is the same.
            largestEnemyWidth = Enum.GetNames(typeof(EnemyType)).Select(name => textureAtlas[$"enemy_{name}_1"].Width).Max();

            gridCellWidth = largestEnemyWidth * MainGame.SpriteScaleFactor;
            gridCellHeight = textureAtlas["enemy_Big_1"].Height * MainGame.SpriteScaleFactor;

            totalWidth = GridWidth * gridCellWidth + (GridWidth - 1) * Padding;
            totalHeight = gridCellHeight * gridCellHeight + (GridHeight - 1) * Padding;

            position = new Vector2((MainGame.GameScreenWidth - totalWidth) * 0.5f, MainGame.GameScreenHeight * 0.25f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Enemy enemy = enemyGrid[x, y];
                    Texture2D texture = textureAtlas[$"enemy_{enemy.Type.ToString()}_{1}"];

                    float paddingX = enemy.GridPosition.X * Padding;

                    // In order to centre the enemy horizontally within the grid cell,
                    // we need to account for the DIFFERENCE between the largest texture
                    // width and the current texture width.
                    float centeringOffsetX = (largestEnemyWidth - texture.Width) * 0.5f * MainGame.SpriteScaleFactor;
                    float offsetX = enemy.GridPosition.X * gridCellWidth + paddingX + centeringOffsetX;

                    float paddingY = enemy.GridPosition.Y * Padding;
                    float offsetY = enemy.GridPosition.Y * gridCellHeight + paddingY;

                    spriteBatch.Draw(texture, position + new Vector2(offsetX, offsetY), null, Color.White, 0, Vector2.Zero, MainGame.SpriteScaleFactor, SpriteEffects.None, 0);
                }
            }
        }
    }
}
