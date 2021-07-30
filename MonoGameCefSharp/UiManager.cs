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
        private const int _width = 1920;
        private const int _height = 1080;

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
        private System.Drawing.Rectangle _textureRect;

        public override void Initialize()
        {
            var settings = new CefSettings();
            settings.RemoteDebuggingPort = 8088;
            settings.CachePath = Directory.GetCurrentDirectory() + "\\cache";
            settings.WindowlessRenderingEnabled = true;

            if (!Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: new BrowserProcessHandler()))
            {
                throw new Exception("Unable to Initialize Cef");
            }

            var browserSettings = new BrowserSettings();
            browserSettings.WindowlessFrameRate = 60;
            var requestContextSettings = new RequestContextSettings { CachePath = Directory.GetCurrentDirectory() + "\\cachePath1" };

            requestContext = new RequestContext(requestContextSettings);
            browser = new ChromiumWebBrowser(SharedDataManager.Url, browserSettings, requestContext);

            browser.Size = new Size(_width, _height);

            _texture = new Texture2D(SharedDataManager.Instance.GraphicsDevice, _width, _height);
            _textureRect = new System.Drawing.Rectangle(0, 0, _width, _height);
        }

        public override void Update(GameTime gameTime)
        {
            if (!browser.IsBrowserInitialized || browser.IsLoading)
            {
                return;
            }

            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                browser.GetBrowser().GetHost().SendMouseClickEvent(mouseState.X, mouseState.Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
                browser.GetBrowser().GetHost().SendMouseClickEvent(mouseState.X, mouseState.Y, MouseButtonType.Left, true, 1, CefEventFlags.None);
                browser.GetBrowser().GetHost().SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.LeftMouseButton);
            }
            else if (mouseState.RightButton == ButtonState.Pressed)
            {
                browser.GetBrowser().GetHost().SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.RightMouseButton);
            }
            else if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                browser.GetBrowser().GetHost().SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.MiddleMouseButton);
            }
            else
            {
                browser.GetBrowser().GetHost().SendMouseMoveEvent(mouseState.X, mouseState.Y, false, CefEventFlags.None);
            }

            UpdateTexture(SharedDataManager.Instance.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_texture == null)
            {
                return;
            }

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

        public void UpdateTexture(GraphicsDevice graphicsDevice)
        {
            var bitmap = browser.ScreenshotOrNull()?.Clone() as Bitmap;
            if(bitmap == null)
            {
                return;
            }
            UpdateTexture2DFromBitmap(graphicsDevice, bitmap, _texture);
        }

        public void UpdateTexture2DFromBitmap(GraphicsDevice device, Bitmap bitmap, Texture2D texture)
        {
            ///================================================
            /// Best performance but color error
            ///=================================================
            //var data = bitmap.LockBits(_textureRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            //int bufferSize = data.Height * data.Stride;

            ////create data buffer 
            //var bytes = new byte[bufferSize];

            //// copy bitmap data into buffer
            //Marshal.Copy(data.Scan0, bytes, 0, bufferSize);

            //// unlock the bitmap data
            //bitmap.UnlockBits(data);

            //// copy our buffer to the texture
            //texture.SetData(bytes);

            ///================================================
            /// Color correction
            ///=================================================

            uint[] m_PixelsBuffer = new uint[bitmap.Width * bitmap.Height];

            unsafe
            {
                System.Drawing.Imaging.BitmapData origdata =
                    bitmap.LockBits(_textureRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
                uint* byteData = (uint*)origdata.Scan0;
                for (int i = 0; i < m_PixelsBuffer.Length; i++)
                {
                    m_PixelsBuffer[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);
                }
                //bitmap.UnlockBits(origdata);
            }
            texture.SetData(m_PixelsBuffer);

            bitmap.Dispose();
        }
    }
}
