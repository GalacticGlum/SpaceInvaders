/*
 * Author: Shon Verch
 * File Name: EnemyGroup.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/06/2019
 * Description: DESCRIPTION
 */

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Helpers;

using MathHelper = SpaceInvaders.Helpers.MathHelper;
using MonoGameMathHelper = Microsoft.Xna.Framework.MathHelper;

namespace SpaceInvaders
{
    public class EnemyGroup
    {
        /// <summary>
        /// The amount of pixels between each enemy cell.
        /// </summary>
        private const int Padding = 10;

        /// <summary>
        /// The amount of enemies in each row.
        /// </summary>
        private const int GroupWidth = 11;

        /// <summary>
        /// The amount of enemies in each column.
        /// </summary>
        private const int GroupHeight = 5;
        
        /// <summary>
        /// The total number of enemies in an <see cref="EnemyGroup"/>.
        /// </summary>
        private const int TotalGroupEnemies = GroupWidth * GroupHeight;

        /// <summary>
        /// The number of pixels to move horizontally.
        /// </summary>
        private const float HorizontalMovementShift = 1;

        /// <summary>
        /// The number of pixels to move vertically when an enemy touches the horizontal boundary.
        /// </summary>
        private const float VerticalMovementShift = 5;

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

        /// <summary>
        /// The width of an individual cell in this <see cref="EnemyGroup"/> in pixels.
        /// </summary>
        private readonly float groupCellWidth;

        /// <summary>
        /// The height of an individual cell in this <see cref="EnemyGroup"/> in pixels.
        /// </summary>
        private readonly float groupCellHeight;

        /// <summary>
        /// The total width of this <see cref="EnemyGroup"/> in pixels, including padding.
        /// </summary>
        private readonly float totalWidth;

        /// <summary>
        /// The total height of this <see cref="EnemyGroup"/> in pixels, including padding.
        /// </summary>
        private readonly float totalHeight;

        private readonly TextureAtlas textureAtlas;

        /// <summary>
        /// A 2D array where the (x, y) element indicates whether
        /// the enemy at that position is dead (where true indicates
        /// that the enemy is NOT dead).
        /// </summary>
        private readonly Enemy[,] enemyGrid;

        /// <summary>
        /// The number of enemies in this <see cref="EnemyGroup"/> that are not dead.
        /// </summary>
        private readonly int remainingEnemyCount;

        /// <summary>
        /// The position of this <see cref="EnemyGroup"/> relative to the top-left.
        /// </summary>
        private Vector2 position;

        private int movementDirection = 1;
        private float timeToMovement;
        private bool canVerticallyMove;

        public EnemyGroup(TextureAtlas textureAtlas)
        {
            this.textureAtlas = textureAtlas;

            enemyGrid = new Enemy[GroupWidth, GroupHeight];
            for (int y = 0; y < GroupHeight; y++)
            {
                for (int x = 0; x < GroupWidth; x++)
                {
                    enemyGrid[x, y] = new Enemy(new Vector2(x, y), enemyTypeLayers[y]);
                }
            }

            // We want the size of each grid cell to be the same so we need to find
            // the width of the largest texture; all other textures will be horizontally centered
            // in the grid cell according to the largest width. The height of all enemies is the same.
            largestEnemyWidth = Enum.GetNames(typeof(EnemyType)).Select(name => textureAtlas[$"enemy_{name}_1"].Width).Max();

            groupCellWidth = largestEnemyWidth * MainGame.SpriteScaleFactor;
            groupCellHeight = textureAtlas["enemy_Big_1"].Height * MainGame.SpriteScaleFactor;

            totalWidth = GroupWidth * groupCellWidth + (GroupWidth - 1) * Padding;
            totalHeight = groupCellHeight * groupCellHeight + (GroupHeight - 1) * Padding;

            remainingEnemyCount = TotalGroupEnemies;
            position = new Vector2((MainGame.GameScreenWidth - totalWidth) * 0.5f, MainGame.GameScreenHeight * 0.25f);
            timeToMovement = GetMovementTimeCurve();
        }

        public void Update(float deltaTime)
        {
            timeToMovement -= deltaTime;
            if (timeToMovement <= 0)
            {
                bool isTouchingHorizontalBounds = position.X == MainGame.HorizontalBoundaryStart.X || position.X == MainGame.HorizontalBoundaryEnd.X - totalWidth;
                if (canVerticallyMove && isTouchingHorizontalBounds)
                {
                    movementDirection *= -1;
                    position.Y += VerticalMovementShift * MainGame.SpriteScaleFactor;
                    canVerticallyMove = false;
                }
                else
                {
                    position.X += HorizontalMovementShift * MainGame.SpriteScaleFactor * movementDirection;
                    canVerticallyMove = true;
                }

                timeToMovement = GetMovementTimeCurve();
            }

            // Make sure our horizontal position does not exceed the horizontal boundary
            position.X = MonoGameMathHelper.Clamp(position.X, MainGame.HorizontalBoundaryStart.X, MainGame.HorizontalBoundaryEnd.X - totalWidth);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < GroupHeight; y++)
            {
                for (int x = 0; x < GroupWidth; x++)
                {
                    Enemy enemy = enemyGrid[x, y];
                    Texture2D texture = textureAtlas[$"enemy_{enemy.Type.ToString()}_{1}"];

                    float paddingX = enemy.GridPosition.X * Padding;

                    // In order to centre the enemy horizontally within the grid cell,
                    // we need to account for the DIFFERENCE between the largest texture
                    // width and the current texture width.
                    float centeringOffsetX = (largestEnemyWidth - texture.Width) * 0.5f * MainGame.SpriteScaleFactor;
                    float offsetX = enemy.GridPosition.X * groupCellWidth + paddingX + centeringOffsetX;

                    float paddingY = enemy.GridPosition.Y * Padding;
                    float offsetY = enemy.GridPosition.Y * groupCellHeight + paddingY;

                    spriteBatch.Draw(texture, position + new Vector2(offsetX, offsetY), null, Color.White, 0, Vector2.Zero, MainGame.SpriteScaleFactor, SpriteEffects.None, 0);
                }
            }
        }

        /// <summary>
        /// <para>
        /// Gets the time between movements, in seconds, implemented as a rational function.
        /// For the full equation, see <see href="https://www.desmos.com/calculator/5oxp18dplc"></see>.
        /// </para>
        /// This implementation uses a simplified version of the one on Desmos to leverage a fast inverse
        /// square root operation.
        /// </summary>
        /// <returns>The time, in seconds, until the next movement.</returns>
        private float GetMovementTimeCurve()
        {
            float r = 1000 * MathHelper.InverseSqrt((float)Math.Pow(TotalGroupEnemies + remainingEnemyCount + 1, 3));
            float e = 2 * r - 0.5f;

            return 1 / e;
        }
    }
}
