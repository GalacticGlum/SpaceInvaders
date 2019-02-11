using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    public struct BarrierHitResult
    {
        public Barrier Barrier { get; }
        public BarrierTile Tile { get; }

        public BarrierHitResult(Barrier barrier, BarrierTile tile)
        {
            Barrier = barrier;
            Tile = tile;
        }
    }

    public class BarrierGroup
    {
        /// <summary>
        /// The number of barriers to spawn.
        /// </summary>
        public const int SpawnBarrierCount = 4;

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
