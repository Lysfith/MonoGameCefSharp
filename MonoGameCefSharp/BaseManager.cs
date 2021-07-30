using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameCefSharp
{
    public abstract class BaseManager : IDisposable
    {
        protected BaseManager()
        {

        }

        public virtual void Initialize()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }

        public virtual void Dispose()
        {

        }
    }
}
