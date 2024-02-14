using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Transactions;

namespace Editor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public enum EditorMode { tilemap, markers };
        EditorMode currentMode = EditorMode.tilemap;

        Tilemap tileMap;
        Tilemap markerMap;

        short[][] map;
        short[][] markers;

        Vector2 currentTile;

        TileSelection tileSelection;
        TileSelection markerSelection;

        string filepathFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Brutal Business\");
        string filepathMaps;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            AccessFolder();
            AccessMaps();

            SetFullscreen(_graphics);

            Common.LoadTextures(Content);

            tileSelection = new TileSelection(new Vector2(GraphicsDevice.Viewport.Width - 200, 100), 32, 23, "tilesheet");
            markerSelection = new TileSelection(new Vector2(GraphicsDevice.Viewport.Width - 150, 100), 32, 5, "markers");

            tileMap = new Tilemap(32, 23, "tilesheet");
            markerMap = new Tilemap(32, 4, "markers");

            currentTile = Vector2.Zero;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            Common.currentMouse = Mouse.GetState();
            Common.currentKeyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ExitEditor();

            currentTile = WorldToGrid(Common.currentMouse.Position.ToVector2(), 32);

            switch (currentMode)
            {
                case EditorMode.tilemap:
                    if(Common.currentKeyboard.IsKeyDown(Keys.S) && Common.lastKeyboard.IsKeyUp(Keys.S))
                    {
                        currentMode = EditorMode.markers;
                    }
                    tileSelection.Update(gameTime);

                    if (Common.currentMouse.LeftButton == ButtonState.Pressed /*&& Common.lastMouse.LeftButton != ButtonState.Pressed*/ && currentTile.X >= 0 && currentTile.X < map[0].Length && currentTile.Y >= 0 && currentTile.Y < map.Length)
                    {
                        SetTile(tileSelection.GetSelectedTile(), currentTile, currentMode);
                    }
                    break;

                case EditorMode.markers:
                    if (Common.currentKeyboard.IsKeyDown(Keys.S) && Common.lastKeyboard.IsKeyUp(Keys.S))
                    {
                        currentMode = EditorMode.tilemap;
                    }
                    markerSelection.Update(gameTime);

                    if (Common.currentMouse.LeftButton == ButtonState.Pressed /*&& Common.lastMouse.LeftButton != ButtonState.Pressed*/ && currentTile.X >= 0 && currentTile.X < markers[0].Length && currentTile.Y >= 0 && currentTile.Y < markers.Length)
                    {
                        SetTile(markerSelection.GetSelectedTile(), currentTile, currentMode);
                    }
                    break;
            }

            Common.lastMouse = Common.currentMouse;
            Common.lastKeyboard = Common.currentKeyboard;
            base.Update(gameTime);
        }

        void SetFullscreen(GraphicsDeviceManager _graphics)
        {
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.ApplyChanges();
            Window.Position = new Point(0, 0);
        }

        void SetTile(int tileNumber, Vector2 position, EditorMode currentMode)
        {
            switch (currentMode)
            {
                case EditorMode.tilemap:
                    map[(int)position.Y][(int)position.X] = (short)tileNumber;
                    break;

                case EditorMode.markers:
                    markers[(int)position.Y][(int)position.X] = (short)tileNumber;
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            
            tileMap.Draw(_spriteBatch, map, currentTile);
            markerMap.Draw(_spriteBatch, markers, currentTile);

            tileSelection.Draw(_spriteBatch);
            markerSelection.Draw(_spriteBatch);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        void ExitEditor()
        {
            SaveMap(map, markers);
            Exit();
        }

        void SaveMap(short[][] map, short[][] markers)
        {
            File.WriteAllText(filepathMaps, JsonSerializer.Serialize<Dictionary<string, short[][]>>(new Dictionary<string, short[][]>() { { "tilemap", map }, { "markers", markers } }, new JsonSerializerOptions { WriteIndented = true}));
        }

        Dictionary<string, short[][]> LoadMap()
        {
            return JsonSerializer.Deserialize<Dictionary<string, short[][]>>(File.ReadAllText(filepathMaps));
        }

        void AccessFolder()
        {
            if(!Directory.Exists(filepathFolder))
            {
                Directory.CreateDirectory(filepathFolder);
            }
        }

        void AccessMaps()
        {
            filepathMaps = Path.Combine(filepathFolder, @"maps.txt");

            if(File.Exists(filepathMaps))
            {
                Dictionary<string, short[][]> tempDictionary = LoadMap();

                map = tempDictionary["tilemap"];
                markers = tempDictionary["markers"];
            }
            else
            {
                map = new short[34][];
                markers = new short[34][];

                for(int i = 0; i < map.Length; i++)
                {
                    map[i] = new short[60];
                    markers[i] = new short[60];
                }
            }
        }

        public Vector2 WorldToGrid(Vector2 worldPosition, int tileSize)
        {
            return new Vector2((int)(worldPosition.X / tileSize), (int)(worldPosition.Y / tileSize));
        }
    }
}