/*
 * Author: Shon Verch
 * File Name: MainMenuScreen.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: DESCRIPTION
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    public class MainMenuScreen : GameScreen
    {
        private SpriteBatch spriteBatch;
        private SpriteFont spaceInvadersFont;

        private TextButton gameplayButton;
        private TextButton highscoreButton;

        public override void OnScreenSwitched()
        {
            MainGame.Context.IsMouseVisible = true;
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            spaceInvadersFont = MainGame.Context.Content.Load<SpriteFont>("SpaceInvadersFont");
        }

        public override void Update(GameTime gameTime)
        {
            gameplayButton?.Update();
            highscoreButton?.Update();
        }

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

        private static void OnHighscoreButtonClicked()
        {
            MainGame.Context.SwitchScreen(GameScreenType.Highscore);
        }

        private static void OnGameplayButtonClicked()
        {
            // Reload the gameplay screen so that everything is reset.
            MainGame.Context.ReloadScreen<GameplayScreen>(GameScreenType.Gameplay);
            MainGame.Context.SwitchScreen(GameScreenType.Gameplay);
        }
    }
}
