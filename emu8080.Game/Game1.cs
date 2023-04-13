using emu8080.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            services.AddLogging(configure =>
            {
                // logging should be disabled to prevent the app from stalling
#if DEBUG
                configure.AddConsole();
#endif
            });

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
            
            int index = 0;
            Color[] tmpTextureData = new Color[SCREEN_WIDTH * SCREEN_HEIGHT];
            for (int i = 0; i < videoBuffer.Length; i++)
            {
                // unpacking 8 pixels per byte
                byte data = videoBuffer[i];
                for(int j = 0; j != 8; j++)
                {
                    // we shift data of j positions so that the bit we care about is in the
                    // least significant position. At this point we can check if it's on
                    // by masking it with 0x1 (binary 0000 0001) and comparing with 1
                    tmpTextureData[index++] = ((data >> j) & 0x1) == 1 ? Color.White : Color.Black;
                }
            }

            // Rotate 90 degrees and flip on X
            index = 0;
            for (var x = SCREEN_HEIGHT - 1; x >= 0; x--)
            {
                for (var y = 0; y < SCREEN_WIDTH; y++)
                {
                    _textureData[index++] = tmpTextureData[y * SCREEN_HEIGHT + x];
                }
            }

            _texture.SetData(_textureData);
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
