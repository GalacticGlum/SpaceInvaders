/*
 * Author: Shon Verch
 * File Name: GameplayScreen.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/24/19
 * Modified Date: 02/24/19
 * Description: The game screen containing all the gameplay.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Engine;

namespace SpaceInvaders
{
    /// <inheritdoc />
    /// <summary>
    /// The game screen containing all the gameplay.
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        public const int HorizontalBoundarySize = 5;
        
        /// <summary>
        /// The y-coordinate of the horizontal boundary line.
        /// The line is placed 1/8 above the bottom of the screen;
        /// or 7/8 (0.875) below the top of the screen.
        /// </summary>
        public const float HorizontalBoundaryY = MainGame.GameScreenHeight * 0.875f;

        /// <summary>
        /// The vertical padding, in pixels, from the top of the screen
        /// that is allocated for UI elements.
        /// This value is 10% of the screen height.
        /// </summary>
        public const float TopVerticalBoundary = MainGame.GameScreenHeight * 0.1f;

        /// <summary>
        /// The gameover header text.
        /// </summary>
        private const string GameoverHeaderText = "GAME OVER";

        /// <summary>
        /// The amount of time, in seconds, that it takes for a character
        /// to "type" in the gameover header.
        /// </summary>
        private const float GameoverTextAnimationTypeTime = 0.1f;
        private const string HudLivesText = "LIVES";
        private const string HudScoreText = "SCORE";
        private const string NamePromptText = "ENTER YOUR NAME:";
        private const int UIElementPadding = 10;

        /// <summary>
        /// The starting point of the horizontal boundary line.
        /// </summary>
        public static readonly Vector2 HorizontalBoundaryStart = new Vector2(HorizontalBoundarySize, HorizontalBoundaryY);

        /// <summary>
        /// The ending point of the horizontal boundary line.
        /// </summary>
        public static readonly Vector2 HorizontalBoundaryEnd = new Vector2(MainGame.GameScreenWidth - 
                HorizontalBoundarySize, HorizontalBoundaryY);

        /// <summary>
        /// The vertical padding for HUD elements, in pixels.
        /// </summary>
        private static readonly Vector2 HudPadding = new Vector2(HorizontalBoundarySize, 10);

        /// <summary>
        /// Indicates whether the game is frozen.
        /// When the game is frozen, no gameplay is simulated.
        /// </summary>
        public bool IsFrozen { get; private set; }

        public Player Player { get; private set; }
        public EnemyGroup EnemyGroup { get; private set; }
        public BarrierGroup BarrierGroup { get; private set; }
        public ProjectileController ProjectileController { get; private set; }
        public UfoController UfoController { get; private set; }

        private SpriteBatch spriteBatch;
        private SpriteFont hudSpriteFont;
        private SpriteFont headerSpriteFont;

        private float freezeTimer;
        private bool isGameover;
        private float gameoverTextTypeTimer = GameoverTextAnimationTypeTime;
        private int gameoverTextCharacterCount;

        private Textbox nameInputTextbox;
        private TextButton nameInputConfirmButton;

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            // Load fonts
            hudSpriteFont = MainGame.Context.Content.Load<SpriteFont>("SpaceInvadersFont");
            headerSpriteFont = MainGame.Context.Content.Load<SpriteFont>("SpaceInvadersFontHeader");

            // Load all the enemy types
            EnemyType.Load(MainGame.Context.Content);

            Player = new Player();
            BarrierGroup = new BarrierGroup();
            Player.InitializeHorizontalPosition();

            EnemyGroup = new EnemyGroup();
            ProjectileController = new ProjectileController();
            UfoController = new UfoController();
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the freeze timer if it has a non-negative duration.
            // A non-negative duration indicates an indefinite freeze.
            if (IsFrozen && freezeTimer > 0)
            {
                freezeTimer -= deltaTime;
                if (freezeTimer <= 0)
                {
                    IsFrozen = false;
                }
            }

            UpdateGameplay(deltaTime);
            UpdateGameoverUI(deltaTime);
        }

        private void UpdateGameplay(float deltaTime)
        {
            if (Input.GetKeyDown(Keys.Q))
            {
                Player.Lives -= 1;
            }

            Player.Update(deltaTime);
            EnemyGroup.Update(deltaTime);
            ProjectileController.Update(deltaTime);
            UfoController.Update(deltaTime);

            if (EnemyGroup.RemainingEnemyCount <= 0)
            {
                EnemyGroup.Spawn();
                Player.Lives += 1;
            }
        }

        private void UpdateGameoverUI(float deltaTime)
        {
            if (!isGameover) return;

            if (gameoverTextCharacterCount < GameoverHeaderText.Length)
            {
                gameoverTextTypeTimer -= deltaTime;
                if (gameoverTextTypeTimer <= 0)
                {
                    gameoverTextTypeTimer = GameoverTextAnimationTypeTime;
                    gameoverTextCharacterCount += 1;
                }
            }

            nameInputTextbox?.Update(deltaTime);
            nameInputConfirmButton?.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.DrawLine(HorizontalBoundaryStart, HorizontalBoundaryEnd, ColourHelpers.PureGreen, 2);

            if (!isGameover)
            {
                EnemyGroup.Draw(spriteBatch);
                UfoController.Draw(spriteBatch);
                ProjectileController.Draw(spriteBatch);
            }

            Player.Draw(spriteBatch);
            BarrierGroup.Draw(spriteBatch);

            DrawUI();
            DrawGameoverUI();
        }

        /// <summary>
        /// Render the user interface.
        /// </summary>
        private void DrawUI()
        {
            spriteBatch.DrawString(hudSpriteFont, HudScoreText, HudPadding, Color.White);

            Vector2 scoreTextPosition = new Vector2(hudSpriteFont.MeasureString(HudScoreText).X + UIElementPadding, 0) + HudPadding;
            spriteBatch.DrawString(hudSpriteFont, Player.Score.ToString(), scoreTextPosition, ColourHelpers.PureGreen);

            Texture2D playerTexture = MainGame.Context.MainTextureAtlas["player"];
            Texture2D playerOutlineTexture = MainGame.Context.MainTextureAtlas["player_outline"];

            int playerTextureScale = hudSpriteFont.LineSpacing / playerTexture.Height;
            int playerOutlineTextureScale = hudSpriteFont.LineSpacing / playerOutlineTexture.Height;

            float widthA = Player.Lives * playerTexture.Width * playerTextureScale;
            float widthB = (Player.MaxLives - Player.Lives) * playerOutlineTexture.Width * playerOutlineTextureScale;
            float livesWidth = widthA + widthB + (Player.MaxLives - 1) * UIElementPadding;

            float livesStartingPositionX = MainGame.GameScreenWidth - HudPadding.X - livesWidth;

            Vector2 livesTextSize = hudSpriteFont.MeasureString(HudLivesText);
            float textPositionX = livesStartingPositionX - UIElementPadding - livesTextSize.X;
            spriteBatch.DrawString(hudSpriteFont, HudLivesText, new Vector2(textPositionX, HudPadding.Y), Color.White);

            for (int i = 0; i < Player.MaxLives; ++i)
            {
                if (Player.Lives <= Player.DefaultLives && i > Player.DefaultLives - 1) continue;

                Texture2D texture = i < Player.Lives ? playerTexture : playerOutlineTexture;
                float scale = i < Player.Lives ? playerTextureScale : playerOutlineTextureScale;
                Vector2 position = new Vector2(livesStartingPositionX + i * (UIElementPadding + texture.Width * scale), HudPadding.Y);

                Color colour = i < Player.Lives ? ColourHelpers.PureGreen : Color.Red;
                spriteBatch.Draw(texture, position, null, colour, 0, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
            }
        }

        /// <summary>
        /// Render the gameover interface.
        /// </summary>
        private void DrawGameoverUI()
        {
            if (!isGameover) return;

            float headerTextPositionX = (MainGame.GameScreenWidth - headerSpriteFont.MeasureString(GameoverHeaderText).X) * 0.5f;
            float headerTextPositionY = (MainGame.GameScreenHeight - headerSpriteFont.LineSpacing) * 0.5f - 50;

            if (gameoverTextCharacterCount > 0)
            {
                string text = GameoverHeaderText.Substring(0, gameoverTextCharacterCount);
                spriteBatch.DrawString(headerSpriteFont, text, new Vector2(headerTextPositionX, headerTextPositionY), Color.White);
            }

            float namePromptPositionX = (MainGame.GameScreenWidth - hudSpriteFont.MeasureString(NamePromptText).X) * 0.5f;
            float namePromptPositionY = headerTextPositionY + headerSpriteFont.LineSpacing + UIElementPadding;
            spriteBatch.DrawString(hudSpriteFont, NamePromptText, new Vector2(namePromptPositionX, namePromptPositionY), Color.White);

            if (gameoverTextCharacterCount == GameoverHeaderText.Length)
            {
                if (nameInputTextbox == null)
                {
                    nameInputTextbox = new Textbox(Vector2.Zero, hudSpriteFont)
                    {
                        Colour = ColourHelpers.PureGreen,
                        Focused = true
                    };

                    float textboxX = (MainGame.GameScreenWidth - nameInputTextbox.Rectangle.Width) * 0.5f;
                    float textboxY = namePromptPositionY + hudSpriteFont.LineSpacing + UIElementPadding;

                    nameInputTextbox.Rectangle.Position = new Vector2(textboxX, textboxY);
                }

                if (nameInputConfirmButton == null)
                {
                    nameInputConfirmButton = new TextButton(Vector2.Zero, hudSpriteFont, "OK")
                    {
                        HoverColour = ColourHelpers.PureGreen
                    };

                    nameInputConfirmButton.Clicked += OnNameInputConfirmButtonClicked;

                    float buttonX = (MainGame.GameScreenWidth - nameInputConfirmButton.Rectangle.Width) * 0.5f;
                    float buttonY = nameInputTextbox.Rectangle.Y + hudSpriteFont.LineSpacing + UIElementPadding;

                    nameInputConfirmButton.Rectangle.Position = new Vector2(buttonX, buttonY);
                }
            }

            nameInputTextbox?.Draw(spriteBatch);
            nameInputConfirmButton?.Draw(spriteBatch);
        }

        private void OnNameInputConfirmButtonClicked()
        {
            // Save the score to the highscores data file
            HighscoreScreen highscoreScreen = MainGame.Context.GetGameScreen<HighscoreScreen>(GameScreenType.Highscore);
            highscoreScreen.WriteHighscore(nameInputTextbox.Text, Player.Score);

            MainGame.Context.SwitchScreen(GameScreenType.Highscore);
        }

        /// <summary>
        /// Freezes the game for a specified amount of seconds.
        /// </summary>
        /// <param name="time">The amount of seconds to freeze the game for.
        /// A negative value indicates an indefinite freeze.
        /// </param>
        public void Freeze(float time = -1)
        {
            freezeTimer = time;
            IsFrozen = true;
        }

        /// <summary>
        /// Unfreezes the game.
        /// </summary>
        public void Unfreeze()
        {
            IsFrozen = false;
        }

        /// <summary>
        /// Triggers the gameover state.
        /// </summary>
        public void TriggerGameover()
        {
            Freeze();

            isGameover = true;
            MainGame.Context.IsMouseVisible = true;
        }
    }
}
