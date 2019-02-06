using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public class Enemy
    {
        public Vector2 GridPosition { get; }
        public EnemyType Type { get; }

        public Enemy(Vector2 gridPosition, EnemyType type)
        {
            GridPosition = gridPosition;
            Type = type;
        }
    }
}
