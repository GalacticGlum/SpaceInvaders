/*
 * Author: Shon Verch
 * File Name: BarrierTile.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/06/2019
 * Modified Date: 02/06/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public class BarrierTile
    {
        private const int MaxHealth = 4;

        public Vector2 Position { get; }
        public string SpriteSuffix { get; }
        public int Health { get; }

        public BarrierTile(Vector2 position, string spriteSuffix)
        {
            Position = position;
            SpriteSuffix = spriteSuffix;
            Health = MaxHealth;
        }
    }
}
