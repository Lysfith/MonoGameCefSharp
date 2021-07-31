using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGameCefSharp
{
    public class UiManager : BaseManager
    {
        private static UiManager _instance;

        public static UiManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UiManager();
                }

                return _instance;
            }
        }

        private RequestContext requestContext;
        private ChromiumWebBrowser browser;
        private Texture2D _texture;
        private DefaultRenderHandler _renderHandler;
        private uint[] _pixelsBuffer;
        private int _previousWheelValue;
        private ButtonState _previousLeftButtonState;
        private ButtonState _previousMiddleButtonState;
        private ButtonState _previousRightButtonState;

        public override void Initialize()
        {
            Cef.EnableHighDPISupport();

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };
            settings.WindowlessRenderingEnabled = true;
            settings.CefCommandLineArgs.Add("enable-media-stream"); //Enable WebRTC
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
            settings.CefCommandLineArgs.Add("no-proxy-server"); //Don't use a proxy server, always make direct connections. Overrides any other proxy server flags that are passed.
            settings.CefCommandLineArgs.Remove("mute-audio");
            settings.WindowlessRenderingEnabled = false;

            if (!Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null))
            {
                throw new Exception("Unable to Initialize Cef");
            }

            var browserSettings = new BrowserSettings();
            browserSettings.WindowlessFrameRate = 60;
            var requestContextSettings = new RequestContextSettings { CachePath = Directory.GetCurrentDirectory() + "\\cachePath1" };

            requestContext = new RequestContext(requestContextSettings);
            browser = new ChromiumWebBrowser(SharedDataManager.Url, browserSettings, requestContext);

            browser.Size = new Size(SharedDataManager.Width, SharedDataManager.Height);
            _renderHandler = browser.RenderHandler as DefaultRenderHandler;

            _texture = new Texture2D(SharedDataManager.Instance.GraphicsDevice, SharedDataManager.Width, SharedDataManager.Height, false, SurfaceFormat.Color);
            _pixelsBuffer = new uint[SharedDataManager.Width * SharedDataManager.Height];
        }

        public override void Update(GameTime gameTime)
        {
            if (!browser.IsBrowserInitialized || browser.IsLoading)
            {
                return;
            }

            var mouseState = Mouse.GetState();
            var host = browser.GetBrowser().GetHost();

            if (_previousLeftButtonState == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                host.SendMouseClickEvent(mouseState.X, mouseState.Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
                host.SendMouseClickEvent(mouseState.X, mouseState.Y, MouseButtonType.Left, true, 1, CefEventFlags.None);
                host.SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.LeftMouseButton);
            }
            else if (_previousRightButtonState == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
            {
                host.SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.RightMouseButton);
            }
            else if (_previousMiddleButtonState == ButtonState.Released && mouseState.MiddleButton == ButtonState.Pressed)
            {
                host.SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.MiddleMouseButton);
            }
            else
            {
                host.SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.None);
            }

            host.SendMouseWheelEvent(mouseState.X, mouseState.Y, 0, mouseState.ScrollWheelValue - _previousWheelValue, CefEventFlags.None);

            _previousWheelValue = mouseState.ScrollWheelValue;
            _previousLeftButtonState = mouseState.LeftButton;
            _previousMiddleButtonState = mouseState.MiddleButton;
            _previousRightButtonState = mouseState.RightButton;

            
        }

        public override void Draw(GameTime gameTime)
        {
            if (_texture == null)
            {
                return;
            }

            UpdateTexture();

            var spriteBatch = SharedDataManager.Instance.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(_texture, new Microsoft.Xna.Framework.Rectangle(0, 0, SharedDataManager.Instance.ScreenWidth, SharedDataManager.Instance.ScreenHeight), 
                Microsoft.Xna.Framework.Color.White);

            spriteBatch.End();
        }

        public override void Dispose()
        {
            browser.Dispose();
            requestContext.Dispose();

            Cef.Shutdown();
        }

        public void UpdateTexture()
        {
            if (_renderHandler.BitmapBuffer.Buffer == null)
            {
                return;
            }

            Buffer.BlockCopy(_renderHandler.BitmapBuffer.Buffer, 0, _pixelsBuffer, 0, _renderHandler.BitmapBuffer.Buffer.Length);

            for (int i = 0; i < _pixelsBuffer.Length; i++)
            {
                _pixelsBuffer[i] = (_pixelsBuffer[i] & 0x000000ff) << 16 | (_pixelsBuffer[i] & 0x0000FF00) | (_pixelsBuffer[i] & 0x00FF0000) >> 16 | (_pixelsBuffer[i] & 0xFF000000);
            }

            _texture.SetData(_pixelsBuffer);
        }

        private uint ColorToUint(byte a, byte r, byte g, byte b)
        {
            return (uint)(((a << 24) | (r << 16) | (g << 8) | b) & 0xffffffff);
        }
    }
}
