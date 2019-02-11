/*
 * Author: Shon Verch
 * File Name: Enemy.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/06/2019
 * Description: DESCRIPTION
 */

using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    /// <summary>
    /// 
    /// </summary>
    public struct Enemy
    {
        /// <summary>
        /// The position of this <see cref="Enemy"/> in the <see cref="EnemyGroup"/> grid.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// The type of this <see cref="Enemy"/>.
        /// </summary>
        public EnemyType Type { get; }

        public bool Active { get; set; }

        /// <summary>
        /// Initializes a new <see cref="Enemy"/>.
        /// </summary>
        /// <param name="position">The position of this <see cref="Enemy"/> in the <see cref="EnemyGroup"/> grid.</param>
        /// <param name="type">The type of this <see cref="Enemy"/>.</param>
        public Enemy(Vector2 position, EnemyType type)
        {
            Position = position;
            Type = type;
            Active = true;
        }
    }
}
