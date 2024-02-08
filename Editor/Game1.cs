using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text.Json;

namespace Editor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Tilemap tilemap;
        short[][] map;
        Vector2 currentTile;

        TileSelection tileSelection;

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

            tileSelection = new TileSelection(new Vector2(GraphicsDevice.Viewport.Width - 200, 100), 32, 11);
            tilemap = new Tilemap(32, 11);
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
            tileSelection.Update(gameTime);

            if(Common.currentMouse.LeftButton == ButtonState.Pressed /*&& Common.lastMouse.LeftButton != ButtonState.Pressed*/ && currentTile.X >= 0 && currentTile.X < map[0].Length && currentTile.Y >= 0 && currentTile.Y < map.Length)
            {
                SetTile(tileSelection.GetSelectedTile(), currentTile);
            }

            // TODO: Add your update logic here

            Common.lastMouse = Common.currentMouse;
            Common.lastKeyboard = Common.currentKeyboard;
            base.Update(gameTime);
        }

        void SetFullscreen(GraphicsDeviceManager _graphics)
        {
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1980;
            _graphics.ApplyChanges();
            Window.Position = new Point(0, 0);
        }

        void SetTile(int tileNumber, Vector2 position)
        {
            map[(int)position.Y][(int)position.X] = (short)tileNumber;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            tilemap.Draw(_spriteBatch, map, currentTile);
            tileSelection.Draw(_spriteBatch);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        void ExitEditor()
        {
            SaveMap(map);
            Exit();
        }

        void SaveMap(short[][] map)
        {
            File.WriteAllText(filepathMaps, JsonSerializer.Serialize<short[][]>(map, new JsonSerializerOptions { WriteIndented = true}));
        }

        short[][] LoadMap()
        {
            return JsonSerializer.Deserialize<short[][]>(File.ReadAllText(filepathMaps));
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
                map = LoadMap();
            }
            else
            {
                map = new short[34][];

                for(int i = 0; i < map.Length; i++)
                {
                    map[i] = new short[50];
                }
            }
        }

        public Vector2 WorldToGrid(Vector2 worldPosition, int tileSize)
        {
            return new Vector2((int)(worldPosition.X / tileSize), (int)(worldPosition.Y / tileSize));
        }
    }
}