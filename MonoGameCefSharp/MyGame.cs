using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameCefSharp
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MyGame : Game
    {
        GraphicsDeviceManager graphics;

        private int width = 1920;
        private int height = 1080;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = SharedDataManager.Width;
            graphics.PreferredBackBufferHeight = SharedDataManager.Height;
            graphics.PreferMultiSampling = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            SharedDataManager.Instance.GraphicsDevice = GraphicsDevice;
            SharedDataManager.Instance.Window = Window;
            SharedDataManager.Instance.ScreenWidth = SharedDataManager.Width;
            SharedDataManager.Instance.ScreenHeight = SharedDataManager.Height;

            SharedDataManager.Instance.Initialize();
            UiManager.Instance.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
           
        }

        private void ExitEvent(object parameter)
        {
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            UiManager.Instance.Dispose();
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UiManager.Instance.Update(gameTime);

            if(Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            UiManager.Instance.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
