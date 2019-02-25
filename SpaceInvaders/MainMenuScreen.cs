/*
 * Author: Shon Verch
 * File Name: MainMenuScreen.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: The game screen that displays the main menu.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    /// <inheritdoc />
    /// <summary>
    /// The game screen that displays the main menu.
    /// </summary>
    public class MainMenuScreen : GameScreen
    {
        private SpriteBatch spriteBatch;
        private SpriteFont spaceInvadersFont;

        private TextButton gameplayButton;
        private TextButton highscoreButton;

        /// <summary>
        /// Called when the game screen switches to this screen type.
        /// </summary>
        public override void OnScreenSwitched()
        {
            MainGame.Context.IsMouseVisible = true;
        }

        /// <summary>
        /// Load the content for this game screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            spaceInvadersFont = MainGame.Context.Content.Load<SpriteFont>("SpaceInvadersFont");
        }

        /// <summary>
        /// Update the main menu screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            gameplayButton?.Update();
            highscoreButton?.Update();
        }

        /// <summary>
        /// Render the main menu screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (gameplayButton == null && highscoreButton == null)
            {
                gameplayButton = new TextButton(Vector2.Zero, spaceInvadersFont, "PLAY")
                {
                    HoverColour = ColourHelpers.PureGreen
                };

                highscoreButton = new TextButton(Vector2.Zero, spaceInvadersFont, "HIGH SCORES")
                {
                    HoverColour = ColourHelpers.PureGreen
                };

                const int buttonVerticalPadding = 20;
                gameplayButton.Rectangle.Position = new Vector2((MainGame.GameScreenWidth - gameplayButton.Rectangle.Width) * 0.5f,
                    (MainGame.GameScreenHeight - gameplayButton.Rectangle.Height) * 0.5f - buttonVerticalPadding);

                highscoreButton.Rectangle.Position = new Vector2((MainGame.GameScreenWidth - highscoreButton.Rectangle.Width) * 0.5f,
                    (MainGame.GameScreenHeight - highscoreButton.Rectangle.Height) * 0.5f + buttonVerticalPadding);

                gameplayButton.Clicked += OnGameplayButtonClicked;
                highscoreButton.Clicked += OnHighscoreButtonClicked;
            }

            gameplayButton?.Draw(spriteBatch);
            highscoreButton?.Draw(spriteBatch);
        }

        /// <summary>
        /// Called when the highscore button is clicked.
        /// </summary>
        private static void OnHighscoreButtonClicked()
        {
            MainGame.Context.SwitchScreen(GameScreenType.Highscore);
        }

        /// <summary>
        /// Called when the gameplay button is clicked.
        /// </summary>
        private static void OnGameplayButtonClicked()
        {
            // Reload the gameplay screen so that everything is reset.
            MainGame.Context.ReloadScreen<GameplayScreen>(GameScreenType.Gameplay);
            MainGame.Context.SwitchScreen(GameScreenType.Gameplay);
        }
    }
}
