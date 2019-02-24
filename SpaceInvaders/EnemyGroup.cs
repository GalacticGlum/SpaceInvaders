/*
 * Author: Shon Verch
 * File Name: EnemyGroup.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/23/2019
 * Description: The top-level logic manager for all enemies.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpaceInvaders.ContentPipeline;
using SpaceInvaders.Engine;

using MathHelper = SpaceInvaders.Engine.MathHelper;
using Random = SpaceInvaders.Engine.Random;

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
        /// The time, in seconds, that an explosion effect will remain on screen.
        /// </summary>
        private const float ExplosionTime = 0.15f;

        public Enemy this[int x, int y] => enemyGrid[x, y];

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
        /// A list of (<see cref="Enemy"/>, <see cref="float"/>) tuples containing
        /// where the first value of the tuple indicates the <see cref="Enemy"/> that
        /// exploded and the second value indicates the time, in seconds, until the
        /// explosion effect should turn off.
        /// </summary>
        private readonly List<Tuple<Enemy, float>> activeExplosions;

        /// <summary>
        /// The starting coordinates of this <see cref="EnemyGroup"/>.
        /// </summary>
        private readonly Vector2 startingPosition;

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
        private bool animationFrameToggle;

        public EnemyGroup()
        {
            activeExplosions = new List<Tuple<Enemy, float>>();

            enemyGrid = new Enemy[GroupWidth, GroupHeight];
            EnemyType[] enemyTypeLayers = LoadEnemyTypeLayers();
            for (int y = 0; y < GroupHeight; y++)
            {
                for (int x = 0; x < GroupWidth; x++)
                {
                    enemyGrid[x, y] = new Enemy(new Vector2(x, y), enemyTypeLayers[y], GetEnemyAttackTime());
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

            timeToMovement = GetMovementTime();
            startingPosition = boundingRectangle.Position;
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
                if (canVerticallyMove && IsTouchingHorizontalBounds())
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

                timeToMovement = GetMovementTime();
                animationFrameToggle = !animationFrameToggle;
            }

            for (int x = 0; x < GroupWidth; x++)
            {
                for (int y = GroupHeight - 1; y >= 0; y--)
                {
                    // Only bottom-most enemies in their respective columns can attack
                    if (!enemyGrid[x, y].Active || y < GroupHeight - 1 && enemyGrid[x, y + 1].Active) continue;

                    Enemy enemy = enemyGrid[x, y];
                    enemy.AttackTime -= deltaTime;
                    if (!(enemy.AttackTime <= 0)) continue;

                    enemy.AttackTime = GetEnemyAttackTime();
                    MainGame.Context.ProjectileController.CreateEnemyProjectile(enemy);

                    // Since we have the bottom-most enemy that active, we are done looking
                    // at the current column.
                    break;
                }
            }

            for (int i = activeExplosions.Count - 1; i >= 0; i--)
            {
                activeExplosions[i] = new Tuple<Enemy, float>(activeExplosions[i].Item1, activeExplosions[i].Item2 - deltaTime);
                if (activeExplosions[i].Item2 <= 0)
                {
                    activeExplosions.RemoveAt(i);
                }
            }
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

            for (int x = 0; x < GroupWidth; x++)
            {
                for (int y = GroupHeight - 1; y >= 0; y--)
                {
                    // Only bottom-most enemies in their respective columns can attack
                    if (!enemyGrid[x,y].Active || y < GroupHeight - 1 && enemyGrid[x, y + 1].Active) continue;

                    RectangleF worldRectangle = GetEnemyWorldRectangle(enemyGrid[x,y]);
                    spriteBatch.DrawBorder(worldRectangle, Color.Green, 3, 1);

                    // Since we have the bottom-most enemy that active, we are done looking
                    // at the current column.
                    break;
                }
            }

            foreach (Tuple<Enemy, float> explosion in activeExplosions)
            {
                RectangleF worldRectangle = GetEnemyWorldRectangle(explosion.Item1);
                spriteBatch.Draw(MainGame.Context.MainTextureAtlas["explosion"], worldRectangle.Position, 
                    null, Color.White, 0, Vector2.Zero, MainGame.ResolutionScale, SpriteEffects.None, 0.7f);
            }
        }

        private bool IsTouchingHorizontalBounds()
        {
            Enemy leftMost = GetLeftMostEnemy();
            Enemy rightMost = GetRightMostEnemy();

            if (leftMost != null && rightMost != null)
            {
                return GetEnemyWorldRectangle(leftMost).Left <= MainGame.HorizontalBoundaryStart.X ||
                       GetEnemyWorldRectangle(rightMost).Right >= MainGame.HorizontalBoundaryEnd.X;
            }

            return false;
        }

        private Enemy GetLeftMostEnemy()
        {
            if (remainingEnemyCount == 0) return null;

            Enemy leftMostEnemy = null;
            float boundaryX = float.MaxValue;
            for (int x = 0; x < GroupWidth; x++)
            {
                for (int y = 0; y < GroupHeight; y++)
                {
                    Enemy enemy = enemyGrid[x, y];
                    if (!enemy.Active) continue;

                    RectangleF rectangle = GetEnemyWorldRectangle(enemy);
                    if (rectangle.Left >= boundaryX) continue;

                    boundaryX = rectangle.Left;
                    leftMostEnemy = enemy;
                }
            }

            return leftMostEnemy;
        }

        private Enemy GetRightMostEnemy()
        {
            if (remainingEnemyCount == 0) return null;

            Enemy rightMostEnemy = null;
            float boundaryX = float.MinValue;
            for (int x = GroupWidth - 1; x >= 0; x--)
            {
                for (int y = 0; y < GroupHeight; y++)
                {
                    Enemy enemy = enemyGrid[x, y];
                    if (!enemy.Active) continue;

                    RectangleF rectangle = GetEnemyWorldRectangle(enemy);
                    if (rectangle.Right <= boundaryX) continue;

                    boundaryX = rectangle.Right;
                    rightMostEnemy = enemy;
                }
            }

            return rightMostEnemy;
        }

        public RectangleF GetEnemyWorldRectangle(Enemy enemy)
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
            MainGame.Context.MainTextureAtlas[$"enemy_{enemy.Type}_{(animationFrameToggle ? 2 : 1)}"];

        public void RemoveEnemy(int x, int y)
        {
            Enemy enemy = enemyGrid[x, y];
            enemy.Active = false;
            remainingEnemyCount -= 1;
            activeExplosions.Add(new Tuple<Enemy, float>(enemy, ExplosionTime));
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
                    if (!enemy.Active || !rectangle.Intersects(GetEnemyWorldRectangle(enemy))) continue;

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
        private float GetMovementTime()
        {
            // The intensity (horizontal compression) of the function;
            // the smaller this value is, the longer the movement rate.
            const float intensityCoefficient = 2.5f;

            // The value of h(x) = 1000 / ((T + 1 - x)^3)
            // where T is the total enemy count and x is the remaining enemy count.
            float hx = 1000 * MathHelper.InverseSqrt((float)Math.Pow(TotalGroupEnemies + remainingEnemyCount + 1, 3));
            
            // m(x) = 1 / 2.5(h(x) - 0.25)
            return 1 / (intensityCoefficient * (hx - 0.25f));
        }

        /// <summary>
        /// <para>
        /// Gets the time until an <see cref="Enemy"/> can attack, in seconds, implemented as a range
        /// between a pair of exponential functions.
        /// </para>
        /// <remarks>
        /// More formally, the upper and lower bounds of the enemy attack times are given by a pair of functions
        /// f(x) and g(x) where f(x) represents the maximum time, g(x) represents the minimum time, and x represents the
        /// change in the vertical distance, in pixels (x in [0, d] where d represents the max vertical distance).
        /// The enemy attack time is defined as a random value uniformly distributed on [f(x), g(x)]: t(x) ~ U([f(x), g(x)])
        /// where U represents the distribution function. For the full equations, see <see href="https://www.desmos.com/calculator/wzpbqmwmwt"></see>.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        private float GetEnemyAttackTime()
        {
            // The initial maximum time until an enemy attacks, in seconds; the value of g(0).
            const float initialMaximumTime = 30;
            // The initial minimum time until an enemy attacks, in seconds; the value of f(0).
            const float initialMinimumTime = 2;
            // The final maximum time until an enemy attacks, in seconds; the value of g(d).
            const float finalMaximumTime = 6;
            // The final minimum time until an enemy attacks; the value of f(d).
            const float finalMinimumTime = 1;

            float validVerticalDistance = 1 / (MainGame.Context.BarrierGroup[0].Rectangle.Top - startingPosition.Y);
            float b1 = (float) Math.Pow(initialMinimumTime / initialMaximumTime, validVerticalDistance);
            float b2 = (float) Math.Pow(finalMinimumTime / finalMaximumTime, validVerticalDistance);

            float distance = boundingRectangle.Y - startingPosition.Y;
            float maximumTime = initialMaximumTime * (float) Math.Pow(b1, distance);
            float minimumTime = finalMaximumTime * (float) Math.Pow(b2, distance);

            return Random.Range(minimumTime, maximumTime);
        }
    }
}
