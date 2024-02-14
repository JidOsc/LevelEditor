using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Editor
{
    internal class Tilemap
    {
        Texture2D tilesheet;
        int amountOfColumns;

        Rectangle sourceRectangle;
        int tileSize;

        public Tilemap(int tileSize, int amountOfColumns, string sheetName)
        {
            this.tileSize = tileSize;
            this.amountOfColumns = amountOfColumns;

            tilesheet = Common.textures[sheetName];

            sourceRectangle = new Rectangle(new Point(0, 0), new Point(tileSize, tileSize));
        }

        public void Draw(SpriteBatch _spriteBatch, short[][] map, Vector2 currentTile)
        {
            for(int x = 0; x < map[0].Length; x++)
            {
                for(int y = 0;  y < map.Length; y++)
                {
                    sourceRectangle.X = (map[y][x] % amountOfColumns) * tileSize;
                    sourceRectangle.Y = (map[y][x] / amountOfColumns) * tileSize;

                    if (x == currentTile.X && y == currentTile.Y)
                    {
                        _spriteBatch.Draw(tilesheet, new Vector2(x, y) * tileSize, sourceRectangle, Color.Red);
                    }
                    else
                    {
                        _spriteBatch.Draw(tilesheet, new Vector2(x, y) * tileSize, sourceRectangle, Color.White);
                    }
                }
            }
        }
    }
}
