using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders
{
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
    }
}
