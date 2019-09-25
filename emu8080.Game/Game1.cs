using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using emu8080.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace emu8080.Game
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Cpu _cpu;
        private Memory _memory;
        private Texture2D _texture;

        static readonly ushort SCREEN_WIDTH = 224;
        static readonly ushort SCREEN_HEIGHT = 256;

        private bool _canUpdateCpu = true;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var registers = new State();
            var bus = new Bus();
            bus.InterruptChanged += Bus_InterruptChanged;
            _cpu = new Cpu(registers, bus);

            _texture = new Texture2D(this.GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT, false, SurfaceFormat.Color);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var gameName = "invaders";
            var romsBasePath = "Content\\roms";
            var gameRomsPath = Path.Combine(romsBasePath, gameName);
            var files = Directory.GetFiles(gameRomsPath);
            var bytes = new List<byte>();
            foreach (var file in files.OrderByDescending(f => f))
            {
                var romBytes = File.ReadAllBytes(file);
                bytes.AddRange(romBytes);
            }
            _memory = Memory.Load(bytes.ToArray());

            while(_canUpdateCpu)
                _cpu.Step(_memory);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            base.Update(gameTime);
        }

        private void Bus_InterruptChanged()
        {
            _memory.UpdateVideoBuffer();

            UpdateVideoTexture();
            _canUpdateCpu = false;
        }

        private void UpdateVideoTexture()
        {
            var screen = Marshal.UnsafeAddrOfPinnedArrayElement(_memory.VideoBuffer, 0);
            var screeni = new Bitmap(SCREEN_HEIGHT, SCREEN_WIDTH, 32, System.Drawing.Imaging.PixelFormat.Format1bppIndexed,
                screen);
            screeni.RotateFlip(RotateFlipType.Rotate90FlipX);
            var screend = screeni.LockBits(new System.Drawing.Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bufferSize = screend.Height * screend.Stride;
            byte[] bytes = new byte[bufferSize];

            Marshal.Copy(screend.Scan0, bytes, 0, bytes.Length);

            _texture.SetData(bytes);

            screeni.UnlockBits(screend);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
      
            _spriteBatch.Begin();

            _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
          
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
