/*
 * Author: Shon Verch
 * File Name: BarrierGroup.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/07/2019
 * Modified Date: 02/24/2019
 * Description: The top-level logic manager for all the barriers.
 */

using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    /// <summary>
    /// The result of a <see cref="SpaceInvaders.Barrier"/>-<see cref="RectangleF"/> intersection query.
    /// </summary>
    public struct BarrierHitResult
    {
        /// <summary>
        /// The intersecting <see cref="SpaceInvaders.Barrier"/>.
        /// </summary>
        public Barrier Barrier { get; }

        /// <summary>
        /// The intersecting <see cref="BarrierTile"/> in the <see cref="Barrier"/>.
        /// </summary>
        public BarrierTile Tile { get; }

        /// <summary>
        /// Initializes a new <see cref="BarrierHitResult"/>.
        /// </summary>
        /// <param name="barrier">The <see cref="SpaceInvaders.Barrier"/> that was hit.</param>
        /// <param name="tile">The <see cref="BarrierTile"/> that was hit in the specified <see cref="SpaceInvaders.Barrier"/>.</param>
        public BarrierHitResult(Barrier barrier, BarrierTile tile)
        {
            Barrier = barrier;
            Tile = tile;
        }
    }

    /// <summary>
    /// The top-level logic manager for all the <see cref="Barrier"/> instances.
    /// </summary>
    public class BarrierGroup
    {
        /// <summary>
        /// The number of barriers to spawn.
        /// </summary>
        public const int SpawnBarrierCount = 4;

        /// <summary>
        /// Gets or sets a <see cref="Barrier"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="Barrier"/>.</param>
        public Barrier this[int index] => barriers[index];

        /// <summary>
        /// A collection of all the <see cref="Barrier"/> instances managed by this <see cref="EnemyGroup"/>.
        /// </summary>
        private readonly Barrier[] barriers;

        /// <summary>
        /// Initializes a new <see cref="BarrierGroup"/>.
        /// </summary>
        public BarrierGroup()
        {
            barriers = new Barrier[SpawnBarrierCount];
            for (int i = 0; i < SpawnBarrierCount; i++)
            {
                barriers[i] = new Barrier(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Barrier barrier in barriers)
            {
                barrier.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Determine whether the specified <see cref="RectangleF"/>
        /// is intersecting with one of the barriers in this <see cref="BarrierGroup"/>.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="hit"></param>
        /// <returns>A boolean indicating whether an intersection occured.</returns>
        public bool Intersects(RectangleF rectangle, out BarrierHitResult hit)
        {
            foreach (Barrier barrier in barriers)
            {
                if (!barrier.Intersects(rectangle, out BarrierTile tile)) continue;

                hit = new BarrierHitResult(barrier, tile);
                return true;
            }

            hit = new BarrierHitResult();
            return false;
        }
    }
}
