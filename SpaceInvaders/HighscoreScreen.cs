/*
 * Author: Shon Verch
 * File Name: HighscoreScreen.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: The game screen that displays the high score.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;
using SpaceInvaders.Logging;

namespace SpaceInvaders
{
    /// <summary>
    /// The game screen that displays the high score.
    /// </summary>
    public class HighscoreScreen : GameScreen
    {
        /// <summary>
        /// The file name of the high score data file.
        /// </summary>
        private const string HighscoreDataFileName = "highscores.csv";

        /// <summary>
        /// The horizontal padding, in pixels, of the highscore table.
        /// </summary>
        private const float HorizontalPadding = MainGame.GameScreenWidth * 0.25f;

        /// <summary>
        /// The vertical padding, in pixels, of the highscore table.
        /// </summary>
        private const float VerticalPadding = MainGame.GameScreenHeight * 0.25f;

        private SpriteFont spaceInvadersFont;
        private SpriteBatch spriteBatch;

        private TextButton exitButton;
        private List<Tuple<string, int>> highscoreData;

        /// <summary>
        /// Loads the all the content for the highscore screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            VerifyHighscoreDataFile();

            spaceInvadersFont = MainGame.Context.Content.Load<SpriteFont>("SpaceInvadersFont");
            LoadHighscores();
        }

        /// <summary>
        /// Called when the screen switches to this screen.
        /// </summary>
        public override void OnScreenSwitched()
        {
            LoadHighscores();
            MainGame.Context.IsMouseVisible = true;
        }

        /// <summary>
        /// Loads the top 10 highscore data.
        /// </summary>
        private void LoadHighscores()
        {
            // Verify the highscore data file and create it if it doesn't exist.
            VerifyHighscoreDataFile();
            
            try
            {
                string highscoreDataPath = GetHighscoreDataPath();
                string[] data = File.ReadAllLines(highscoreDataPath);
                if (highscoreData == null)
                {
                    highscoreData = new List<Tuple<string, int>>();
                }
                else
                {
                    highscoreData.Clear();
                }

                foreach (string line in data)
                {
                    string[] parts = line.Split(',');
                    highscoreData.Add(new Tuple<string, int>(parts[0], int.Parse(parts[1])));
                }

                // Sort the data by the second value in the tuple and then only take the first 10 entries.
                highscoreData = highscoreData.OrderByDescending(x => x.Item2).Take(Math.Min(10, highscoreData.Count)).ToList();
            }
            catch (Exception e)
            {
                Logger.LogFunctionEntry(string.Empty, e.Message, LoggerVerbosity.Error);
            }
        }

        /// <summary>
        /// Write a score entry to the highscore data file.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="score">The score of the player.</param>
        public void WriteHighscore(string name, int score)
        {
            VerifyHighscoreDataFile();
            string highscoreDataPath = GetHighscoreDataPath();
            using (StreamWriter streamWriter = File.AppendText(highscoreDataPath))
            {
                streamWriter.WriteLine($"{name},{score}");
            }
        }

        /// <summary>
        /// Update the highscore screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            exitButton?.Update();
        }

        /// <summary>
        /// Render the highscore screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.DrawString(spaceInvadersFont, "NAME", new Vector2(HorizontalPadding, VerticalPadding), ColourHelpers.PureGreen);

            float scoreTextX = MainGame.GameScreenWidth - HorizontalPadding - spaceInvadersFont.MeasureString("SCORE").X;
            spriteBatch.DrawString(spaceInvadersFont, "SCORE", new Vector2(scoreTextX, VerticalPadding), ColourHelpers.PureGreen);

            const int entryVerticalPadding = 5;
            const float entriesOffset = 30;
            float entriesTotalHeight = 10 * spaceInvadersFont.LineSpacing + 9 * entryVerticalPadding + entriesOffset;

            if (highscoreData.Count <= 0)
            {
                const string emptyHighscoresText = "There are no highscores, yet...";
                float emptyHighscoresTextX = (MainGame.GameScreenWidth - spaceInvadersFont.MeasureString(emptyHighscoresText).X) * 0.5f;
                float emptyHighscoresTextY = VerticalPadding + entriesTotalHeight * 0.5f;
                spriteBatch.DrawString(spaceInvadersFont, emptyHighscoresText, new Vector2(emptyHighscoresTextX, emptyHighscoresTextY), Color.White);
            }
            else
            {
                for(int i = 0; i < highscoreData.Count; ++i)
                {
                    Tuple<string, int> entry = highscoreData[i];

                    float textY = VerticalPadding + (i + 1) * spaceInvadersFont.LineSpacing + entryVerticalPadding * i + entriesOffset;
                    spriteBatch.DrawString(spaceInvadersFont, entry.Item1, new Vector2(HorizontalPadding, textY), Color.White);

                    float entryScoreTextX = MainGame.GameScreenWidth - HorizontalPadding - spaceInvadersFont.MeasureString(entry.Item2.ToString()).X;
                    spriteBatch.DrawString(spaceInvadersFont, entry.Item2.ToString(), new Vector2(entryScoreTextX, textY), Color.White);
                }
            }

            if (exitButton == null)
            {
                exitButton = new TextButton(Vector2.Zero, spaceInvadersFont, "NEW GAME")
                {
                    HoverColour = ColourHelpers.PureGreen
                };

                exitButton.Clicked += OnExitButtonClicked;

                float buttonX = (MainGame.GameScreenWidth - exitButton.Rectangle.Width) * 0.5f;
                exitButton.Rectangle.Position = new Vector2(buttonX, VerticalPadding + entriesTotalHeight);
            }

            exitButton?.Draw(spriteBatch);
        }

        /// <summary>
        /// Called when the exit button is clicked.
        /// </summary>
        private static void OnExitButtonClicked()
        {
            MainGame.Context.SwitchScreen(GameScreenType.MainMenu);
        }

        /// <summary>
        /// Ensures that the highscore data file has been created.
        /// If it does not exist when this method is called, it will be created.
        /// </summary>
        private static void VerifyHighscoreDataFile()
        {
            string highscoreDataPath = GetHighscoreDataPath();
            if (File.Exists(highscoreDataPath)) return;

            Logger.LogFunctionEntry(string.Empty, "Highscore data file does not exist. Creating a new one!", LoggerVerbosity.Warning);
            File.Create(highscoreDataPath);
        }

        private static string GetHighscoreDataPath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HighscoreDataFileName);
    }
}
