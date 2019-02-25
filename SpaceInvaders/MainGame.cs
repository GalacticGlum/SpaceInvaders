/*
 * Author: Shon Verch
 * File Name: MainGame.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/24/2019
 * Description: The core engine instance of the game that spawns all other entities
 *              and simulates logic and rendering.
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Engine;
using SpaceInvaders.Logging;

namespace SpaceInvaders
{
    /// <summary>
    /// The core engine instance of the game that spawns
    /// all other entities and simulates logic and rendering.
    /// </summary>
    public class MainGame : Game
    {
        public const int GameScreenWidth = 628;
        public const int GameScreenHeight = 580;

        /// <summary>
        /// The scale factor of all game sprites.
        /// </summary>
        public const float ResolutionScale = 2.5f;

        /// <summary>
        /// The intensity of sounds.
        /// </summary>
        public const float Volume = 0.25f;

        /// <summary>
        /// The current instance of this <see cref="MainGame"/>.
        /// </summary>
        public static MainGame Context { get; private set; }

        /// <summary>
        /// The main texture atlas containing the player, enemy, and combat sprites.
        /// </summary>
        public TextureAtlas MainTextureAtlas { get; private set; }
 
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private readonly Dictionary<GameScreenType, GameScreen> gameScreens;
        private GameScreenType activeGameScreenType;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Context = this;

            // Preload all the game screens
            gameScreens = new Dictionary<GameScreenType, GameScreen>
            {
                {GameScreenType.Gameplay, new GameplayScreen()},
                {GameScreenType.Highscore, new HighscoreScreen()},
                {GameScreenType.MainMenu, new MainMenuScreen()}
            };

            SwitchScreen(GameScreenType.MainMenu);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = GameScreenWidth;
            graphics.PreferredBackBufferHeight = GameScreenHeight;
            graphics.ApplyChanges();

            Logger.Destination = LoggerDestination.File;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MainTextureAtlas = new TextureAtlas("MainAtlas", GraphicsDevice, Content);

            foreach (KeyValuePair<GameScreenType, GameScreen> screen in gameScreens)
            {
                screen.Value.LoadContent(spriteBatch);
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // We NEED to update input before we execute game logic
            // so that the gameplay does not lag by a frame (due to not synchronized input).
            Input.Update();

            gameScreens[activeGameScreenType].Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            gameScreens[activeGameScreenType].Draw(gameTime);
            spriteBatch.End();
        }

        /// <summary>
        /// Called when the game is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            // Write the logger buffer to the file system
            Logger.FlushMessageBuffer();
        }

        /// <summary>
        /// Switch the active game screen to the specified <see cref="GameScreenType"/>.
        /// </summary>
        /// <param name="type">The new <see cref="GameScreenType"/>.</param>
        public void SwitchScreen(GameScreenType type)
        {
            activeGameScreenType = type;
            gameScreens[activeGameScreenType].OnScreenSwitched();
        }

        /// <summary>
        /// Reload a game screen of the specified <see cref="GameScreenType"/>.
        /// <remarks>
        /// This method reinitializes the game screen by calling its constructor.
        /// It will reset all data.
        /// </remarks>
        /// </summary>
        /// <typeparam name="T">The type of the game screen.</typeparam>
        /// <param name="type">The <see cref="GameScreenType"/>.</param>
        public void ReloadScreen<T>(GameScreenType type) where T : GameScreen
        {
            gameScreens[type] = (GameScreen)Activator.CreateInstance(typeof(T));
            gameScreens[type].LoadContent(spriteBatch);
        }

        /// <summary>
        /// Get a game screen of the specified <see cref="GameScreenType"/>.
        /// </summary>
        /// <typeparam name="T">The type of the game screen to cast to.</typeparam>
        /// <param name="type">The type of the <see cref="GameScreenType"/> to retrieve.</param>
        /// <returns>A <see cref="GameScreen"/> of type <typeparamref name="T"/>.</returns>
        public T GetGameScreen<T>(GameScreenType type) where T : GameScreen  => (T)gameScreens[type];
    }
}
