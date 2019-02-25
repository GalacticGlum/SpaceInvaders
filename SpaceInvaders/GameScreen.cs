/*
 * Author: Shon Verch
 * File Name: GameScreen.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: The base game screen.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders
{
    /// <summary>
    /// The base game screen.
    /// </summary>
    public class GameScreen
    {
        public virtual void LoadContent(SpriteBatch spriteBatch) { }
        public virtual void OnScreenSwitched() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }
}
