using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class Common
    {
        public static MouseState currentMouse, lastMouse;
        public static KeyboardState currentKeyboard, lastKeyboard;

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>()
        {
            {"tilesheet", null },
            {"pixel", null }
        };

        public static void LoadTextures(ContentManager _contentManager)
        {
            foreach(string textureName in textures.Keys)
            {
                textures[textureName] = _contentManager.Load<Texture2D>(textureName);
            }
        }
    }
}
