using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoGameCefSharp
{
    public class SharedDataManager : BaseManager
    {
        private static SharedDataManager _instance;

        public static SharedDataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SharedDataManager();
                }

                return _instance;
            }
        }

        public const string Url = "https://html5test.com";
        //public const string Url = "https://www.60fps.fr/en";
        //public const string Url = "https://youtu.be/dR-4vgED8pQ?t=687";

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public Version Version { get; set; }

        public GameComponentCollection Components { get; set; }
        public ContentManager Content { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public GameServiceContainer Services { get; set; }
        public GameWindow Window { get; set; }

        public ScalingViewportAdapter ViewportAdapter { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        public override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, ScreenWidth, ScreenHeight);

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            ViewportAdapter = null;
            SpriteBatch.Dispose();

            base.Dispose();
        }
    }
}
