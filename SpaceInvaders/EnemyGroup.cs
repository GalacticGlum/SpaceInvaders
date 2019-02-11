/*
 * Author: Shon Verch
 * File Name: EnemyGroup.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/11/2019
 * Description: The top-level logic manager for all enemies.
 */

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Engine;
using MathHelper = SpaceInvaders.Engine.MathHelper;
using MonoGameMathHelper = Microsoft.Xna.Framework.MathHelper;

namespace SpaceInvaders
{
    /// <summary>
    /// The top-level logic manager for all enemies.
    /// </summary>
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
        private const float HorizontalMovementShift = 2;

        /// <summary>
        /// The number of pixels to move vertically when an enemy touches the horizontal boundary.
        /// </summary>
        private const float VerticalMovementShift = 5;

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

        /// <summary>
        /// A 2D array where the (x, y) element indicates whether
        /// the enemy at that position is dead (where true indicates
        /// that the enemy is NOT dead).
        /// </summary>
        private readonly Enemy[,] enemyGrid;

        /// <summary>
        /// The number of enemies in this <see cref="EnemyGroup"/> that are not dead.
        /// </summary>
        private int remainingEnemyCount;
        
        /// <summary>
        /// The bounding <see cref="RectangleF"/> of this <see cref="EnemyGroup"/>.
        /// </summary>
        private RectangleF boundingRectangle;

        private int movementDirection = 1;
        private float timeToMovement;
        private bool canVerticallyMove;

        private int animationFrameCounter;

        public EnemyGroup()
        {
            enemyGrid = new Enemy[GroupWidth, GroupHeight];
            EnemyType[] enemyTypeLayers = LoadEnemyTypeLayers();
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
            largestEnemyWidth = EnemyType.All().Select(type => MainGame.Context.MainTextureAtlas[$"enemy_{type}_1"].Width).Max();

            groupCellWidth = largestEnemyWidth * MainGame.ResolutionScale;
            groupCellHeight = MainGame.Context.MainTextureAtlas["enemy_Big_1"].Height * MainGame.ResolutionScale;

            totalWidth = GroupWidth * groupCellWidth + (GroupWidth - 1) * Padding;
            totalHeight = groupCellHeight * groupCellHeight + (GroupHeight - 1) * Padding;

            remainingEnemyCount = TotalGroupEnemies;

            float positionX = (MainGame.GameScreenWidth - totalWidth) * 0.5f;
            const float positionY = MainGame.GameScreenHeight * 0.25f;
            boundingRectangle = new RectangleF(positionX, positionY, totalWidth, totalHeight);

            timeToMovement = GetMovementTimeCurve();
        }

        /// <summary>
        /// Loads the enemy type layers from a content file.
        /// </summary>
        /// <returns>An array of enemy types containing the enemy type layers.</returns>
        private static EnemyType[] LoadEnemyTypeLayers()
        {
            string json = MainGame.Context.Content.Load<JsonObject>("EnemyTypeLayers").JsonSource;
            string[] enemyTypeNames = JsonConvert.DeserializeObject<string[]>(json);

            EnemyType[] enemyTypes = new EnemyType[enemyTypeNames.Length];
            for (int i = 0; i < enemyTypes.Length; i++)
            {
                enemyTypes[i] = EnemyType.Parse(enemyTypeNames[i]);
            }

            return enemyTypes;
        }

        /// <summary>
        /// Simulates the gameplay logic for this <see cref="EnemyGroup"/>.
        /// </summary>
        /// <param name="deltaTime">The elapsed time between this frame and the last frame, in seconds.</param>
        public void Update(float deltaTime)
        {
            timeToMovement -= deltaTime;
            if (timeToMovement <= 0)
            {
                bool isTouchingHorizontalBounds = boundingRectangle.X == MainGame.HorizontalBoundaryStart.X || 
                                                  boundingRectangle.X == MainGame.HorizontalBoundaryEnd.X - totalWidth;

                if (canVerticallyMove && isTouchingHorizontalBounds)
                {
                    movementDirection *= -1;
                    boundingRectangle.Y += VerticalMovementShift * MainGame.ResolutionScale;
                    canVerticallyMove = false;
                }
                else
                {
                    boundingRectangle.X += HorizontalMovementShift * MainGame.ResolutionScale * movementDirection;
                    canVerticallyMove = true;
                }

                timeToMovement = GetMovementTimeCurve();
                animationFrameCounter = (animationFrameCounter + 1) % 2;
            }

            // Make sure our horizontal position does not exceed the horizontal boundary
            boundingRectangle.X = MonoGameMathHelper.Clamp(boundingRectangle.X, MainGame.HorizontalBoundaryStart.X, 
                MainGame.HorizontalBoundaryEnd.X - totalWidth);
        }

        /// <summary>
        /// Renders this <see cref="EnemyGroup"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> context.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < GroupHeight; y++)
            {
                for (int x = 0; x < GroupWidth; x++)
                {
                    Enemy enemy = enemyGrid[x, y];
                    if (!enemy.Active) continue;

                    RectangleF worldRectangle = GetEnemyWorldRectangle(enemy);

                    spriteBatch.Draw(GetEnemyTexture(enemy), worldRectangle.Position, 
                        null, Color.White, 0, Vector2.Zero, MainGame.ResolutionScale, SpriteEffects.None, 0.7f);
                }
            }
        }

        private RectangleF GetEnemyWorldRectangle(Enemy enemy)
        {
            Texture2D texture = GetEnemyTexture(enemy);
            float paddingX = enemy.Position.X * Padding;

            // In order to centre the enemy horizontally within the grid cell,
            // we need to account for the DIFFERENCE between the largest texture
            // width and the current texture width.
            float centeringOffsetX = (largestEnemyWidth - texture.Width) * 0.5f * MainGame.ResolutionScale;
            float offsetX = enemy.Position.X * groupCellWidth + paddingX + centeringOffsetX;

            float paddingY = enemy.Position.Y * Padding;
            float offsetY = enemy.Position.Y * groupCellHeight + paddingY;

            return new RectangleF(boundingRectangle.X + offsetX, boundingRectangle.Y + offsetY,
                texture.Width * MainGame.ResolutionScale, texture.Height * MainGame.ResolutionScale);
        }

        private Texture2D GetEnemyTexture(Enemy enemy) =>
            MainGame.Context.MainTextureAtlas[$"enemy_{enemy.Type}_{animationFrameCounter + 1}"];

        public void RemoveEnemy(int x, int y)
        {
            enemyGrid[x, y].Active = false;
            remainingEnemyCount -= 1;
        }

        public bool Intersects(RectangleF rectangle, out Point result)
        {
            result = Point.Zero;
            if (!rectangle.Intersects(boundingRectangle)) return false;

            for (int y = 0; y < GroupHeight; y++)
            {
                for (int x = 0; x < GroupWidth; x++)
                {
                    Enemy enemy = enemyGrid[x, y];
                    if (!enemy.Active) continue;
                    if (!rectangle.Intersects(GetEnemyWorldRectangle(enemy))) continue;
                    result = new Point(x, y);
                    return true;
                }
            }

            return false;
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
            // The value of h(x) = 1000 / ((T + 1 - x)^3)
            // where T is the total enemy count and x is the remaining enemy count.
            float hx = 1000 * MathHelper.InverseSqrt((float)Math.Pow(TotalGroupEnemies + remainingEnemyCount + 1, 3));
            
            // m(x) = 1 / 2.5(h(x) - 0.25)
            return 1 / (2.5f * hx - 0.625f);
        }
    }
}
