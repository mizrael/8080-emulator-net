using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using emu8080.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

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

        private Color[] _textureData;
        private Texture2D _texture;

        private const ushort SCREEN_WIDTH = 224;
        private const ushort SCREEN_HEIGHT = 256;

        private int _scale = 2;

        private int _interruptToGenerate = 1;
        private TimeSpan _lastInterruptTime = TimeSpan.Zero;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH * _scale;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT * _scale;
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
            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());

            var sp = services.BuildServiceProvider();

            var registers = new Registers();
            var bus = new Bus();
            bus.InterruptChanged += Bus_InterruptChanged;

            var logger = sp.GetRequiredService<ILogger<Cpu>>();
            _cpu = new Cpu(registers, bus, logger);

            _textureData = new Color[SCREEN_WIDTH * SCREEN_HEIGHT];
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
            var gameRomsPath = Path.Combine(Content.RootDirectory, "roms", gameName);
            var files = Directory.GetFiles(gameRomsPath);
            var bytes = new List<byte>();
            foreach (var file in files.OrderByDescending(f => f))
            {
                var romBytes = File.ReadAllBytes(file);
                bytes.AddRange(romBytes);
            }
            _memory = Memory.Load(bytes.ToArray());
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

            var canGenerateInterrupt = _cpu.Bus.InterruptEnabled && (gameTime.TotalGameTime - _lastInterruptTime).TotalMilliseconds > 8;
            if (canGenerateInterrupt)
            {
                _lastInterruptTime = gameTime.TotalGameTime;

                GenerateInterrupt(_interruptToGenerate);

                _interruptToGenerate = (1 == _interruptToGenerate) ? 2 : 1;

                UpdateVideoTexture();
            }

            int cycles = 2000;
            while (0 != cycles--)
            {
                _cpu.Step(_memory);
            }

            base.Update(gameTime);
        }

        private void Bus_InterruptChanged(bool value)
        {
        }

        private void GenerateInterrupt(int interruptNum)
        {
            Ops.PUSH_PC(_memory, _cpu);

            Ops.DI(_memory, _cpu);

            //Set the PC to the low memory vector.    
            //This is identical to an "RST interrupt_num" instruction.    
            _cpu.Registers.ProgramCounter = (ushort)(8 * interruptNum);
        }
        
        private void UpdateVideoTexture()
        {
            var videoBuffer = _memory.VideoBuffer.Span;
            const int rowSize = SCREEN_WIDTH / 8;
            for (int i = 0; i < SCREEN_HEIGHT; i++) {
                for (int j = 0; j < SCREEN_WIDTH; j++) {
                    byte colorByte = videoBuffer[i * rowSize + j / 8]; // Get 1 byte containing 8 pixels
                    int colorIndex = (colorByte >> (7 - (j % 8))) & 0x01; // Extract color bit
                     _textureData[(255 - i) * 224 + j] = colorIndex == 1 ? Color.White : Color.Black; // Rotate and set color
                    //_textureData[i * SCREEN_WIDTH + j] = colorIndex == 1 ? Color.White : Color.Black;
                }
            }
            _texture.SetData(_textureData);

            // ReadOnlySpan<byte> imageData = _memory.VideoBuffer;
            // using var image = Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.L8>(imageData, SCREEN_WIDTH, SCREEN_HEIGHT);
            // image.Mutate(x => x.Rotate(RotateMode.Rotate90));

            // var bufferSize = SCREEN_WIDTH * SCREEN_HEIGHT;
            // var bytes = new byte[bufferSize];
            // image.CopyPixelDataTo()

            // var gameScreen = Marshal.UnsafeAddrOfPinnedArrayElement(_memory.VideoBuffer, 0);

            // var bmp = new Bitmap(SCREEN_HEIGHT, SCREEN_WIDTH, 32, PixelFormat.Format1bppIndexed, gameScreen);
            // bmp.RotateFlip(RotateFlipType.Rotate90FlipX);

            // var rect = new System.Drawing.Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
            // var bmpBits = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // var bufferSize = bmpBits.Height * bmpBits.Stride;
            // var bytes = new byte[bufferSize];

            // Marshal.Copy(bmpBits.Scan0, bytes, 0, bytes.Length);

            // _texture.SetData(bytes);

            // bmp.UnlockBits(bmpBits);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_texture, Vector2.Zero, null, Color.White, 0f,
                            Vector2.Zero, Vector2.One * _scale, SpriteEffects.None, 0);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
