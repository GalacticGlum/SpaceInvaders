/*
 * Author: Shon Verch
 * File Name: HighscoreScreen.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: DESCRIPTION
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
    public class HighscoreScreen : GameScreen
    {
        private const string HighscoreDataFileName = "highscores.csv";
        private const float HorizontalPadding = MainGame.GameScreenWidth * 0.25f;
        private const float VerticalPadding = MainGame.GameScreenHeight * 0.25f;

        private SpriteFont spaceInvadersFont;
        private SpriteBatch spriteBatch;

        private TextButton exitButton;
        private List<Tuple<string, int>> highscoreData;

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            VerifyHighscoreDataFile();

            spaceInvadersFont = MainGame.Context.Content.Load<SpriteFont>("SpaceInvadersFont");
            LoadHighscores();
        }

        public override void OnScreenSwitched()
        {
            LoadHighscores();
        }

        private void LoadHighscores()
        {
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

        public void WriteHighscore(string name, int score)
        {
            VerifyHighscoreDataFile();
            string highscoreDataPath = GetHighscoreDataPath();
            using (StreamWriter streamWriter = File.AppendText(highscoreDataPath))
            {
                streamWriter.WriteLine($"{name},{score}");
            }
        }

        public override void Update(GameTime gameTime)
        {
            exitButton?.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.DrawString(spaceInvadersFont, "NAME", new Vector2(HorizontalPadding, VerticalPadding), ColourHelpers.PureGreen);

            float scoreTextX = MainGame.GameScreenWidth - HorizontalPadding - spaceInvadersFont.MeasureString("SCORE").X;
            spriteBatch.DrawString(spaceInvadersFont, "SCORE", new Vector2(scoreTextX, VerticalPadding), ColourHelpers.PureGreen);

            const int entryVerticalPadding = 5;
            float entriesOffset = 30;
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

        private void OnExitButtonClicked()
        {
        }

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
