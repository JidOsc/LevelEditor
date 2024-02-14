using Microsoft.Xna.Framework;
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
    internal class TileSelection
    {
        Rectangle background;
        Texture2D backgroundTexture;
        Vector2 position;

        int srcRectangleNumber;
        Rectangle[] sourceRectangles;

        Texture2D tilesheet;
        int tileSize;

        public TileSelection(Vector2 position, int tileSize, int numberOfFrames, string textureName)
        {
            this.position = position;
            this.tileSize = tileSize;

            tilesheet = Common.textures[textureName];
            backgroundTexture = Common.textures["pixel"];

            background = new Rectangle(position.ToPoint(), new Point(tileSize + 100, tileSize * numberOfFrames + 100));
            srcRectangleNumber = 0;
            
            sourceRectangles = new Rectangle[numberOfFrames];
            for(int i = 0; i < sourceRectangles.Length; i++)
            {
                sourceRectangles[i] = new Rectangle(new Point(i * tileSize, 0), new Point(tileSize, tileSize));
            }
        }

        public int GetSelectedTile()
        {
            return srcRectangleNumber;
        }

        public void Update(GameTime _gameTime)
        {
            if(srcRectangleNumber > 0 && Common.currentKeyboard.IsKeyDown(Keys.Up) && !Common.lastKeyboard.IsKeyDown(Keys.Up))
            {
                srcRectangleNumber--;
            }
            else if(srcRectangleNumber < sourceRectangles.Length - 1 && Common.currentKeyboard.IsKeyDown(Keys.Down) && !Common.lastKeyboard.IsKeyDown(Keys.Down))
            {
                srcRectangleNumber++;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(backgroundTexture, position, background, Color.RosyBrown);

            for(int i = 0; i < sourceRectangles.Length; i++)
            {
                if(i == srcRectangleNumber)
                {
                    _spriteBatch.Draw(tilesheet, position + new Vector2(0, i * tileSize + 0), sourceRectangles[i], Color.Blue);
                }
                else
                {
                    _spriteBatch.Draw(tilesheet, position + new Vector2(0, i * tileSize + 0), sourceRectangles[i], Color.White);
                }
            }
        }
    }
}
