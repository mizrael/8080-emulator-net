using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

namespace emu8080
{
    class Program
    {

        static readonly ushort SCREEN_WIDTH = 224;
        static readonly ushort SCREEN_HEIGHT = 256;

        /// <summary>
        /// http://www.emulator101.com/
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var gameName = "invaders";
            
            var romsBasePath = "roms";
            
            var gameRomsPath = Path.Combine(romsBasePath, gameName);
            
            Console.WriteLine($"loading roms from {gameRomsPath}...");

            var files = Directory.GetFiles(gameRomsPath);
            var bytes = new List<byte>();
            foreach (var file in files.OrderByDescending(f => f))
            {
                var romBytes = File.ReadAllBytes(file);
                bytes.AddRange(romBytes);
            }
            
            var memory = Memory.Load(bytes.ToArray());

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("processing program, press ESC to quit");

            var registers = new State();
            var bus = new Bus();
            
            var canUpdateFrame = false;
            bus.InterruptChanged += () =>
            {
                canUpdateFrame = true;
            };

            var cpu = new Cpu(registers, bus);
            cpu.Reset();

            using (var game = new GameWindow(SCREEN_WIDTH, SCREEN_HEIGHT, GraphicsMode.Default, "LearnOpenTK", GameWindowFlags.FixedWindow))
            {
                game.VSync = VSyncMode.On;

                var videoBuff = new byte[Memory.videoBufferSize];
             
                int frameIndex = 0;
                game.RenderFrame += (object sender, FrameEventArgs e) =>
                {
                    Console.WriteLine($"{frameIndex++} - {cpu.State}");

                    cpu.Step(memory);

                    if (!canUpdateFrame)
                        return;
                    videoBuff = memory.GetVideoBuffer(videoBuff);

                    var gw = sender as GameWindow;
                    OnRenderFrame(gw, videoBuff);

                    gw.Title = "Play8080 FPS: " + (1f / e.Time).ToString("0.");
                };
                game.Run(60.0);
            }

            //Debug(cpu, memory);

            //Render(cpu, memory, ref startRendering);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("done!");

            Console.ResetColor();
        }

        private static void Debug(Cpu cpu, Memory memory)
        {
            int i = 0;
            var sb = new StringBuilder();
            while (++i < 45000)
            {
                sb.Append(i);
                sb.Append(" ) ");
                sb.Append(cpu.State);

                cpu.Step(memory);

                sb.Append(cpu.State.ProgramCounter.ToString("X"));
                sb.Append(" : ");
                sb.Append(memory[cpu.State.ProgramCounter].ToString("X"));
                sb.Append(memory[cpu.State.ProgramCounter + 1].ToString("X"));
                sb.Append(memory[cpu.State.ProgramCounter + 2].ToString("X"));

                Console.WriteLine(sb.ToString());
                sb.Clear();
            }
        }

        private static void Render(Cpu cpu, Memory memory, ref bool startRendering)
        {
            int cycle = 0;
            int screenWidth = 256;
            int screenHeight = 224;

            var videoBuff = new byte[Memory.videoBufferSize];
            var chars = new char[Memory.videoBufferSize * 8 + 256];

            while (true)
            {
                cpu.Step(memory);
                
                if (startRendering)
                {
                    Console.Clear();

                    memory.GetVideoBuffer(videoBuff);
                    int index = 0;
                    int charIndex = 0;
                    while (index < videoBuff.Length)
                    {
                        var videoData = videoBuff[index++];
                        bool[] pixels = GetPixels(videoData);
                        foreach(var pixel in pixels)
                            chars[charIndex++] = (pixel ? '▓' : ' ');

                        var newLine = 0 == (index % 224);
                        if(newLine)
                            chars[charIndex++] = '\n';
                    }

                    Console.Write(chars);
                }

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Escape)
                        break;
                }
            }
        }

        private static bool[] GetPixels(byte videoData)
        {
            var result = new bool[8];
            var value = videoData;
            for(int i=0;i!=8;++i)
            {
                result[i] = (value & 0x1) == 0x1;
                value = (byte) (value >> 1);
            }

            return result;
        }

        static void OnRenderFrame(GameWindow window, byte[] screenBuffer)
        {
            int texID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texID);

            var screen = Marshal.UnsafeAddrOfPinnedArrayElement(screenBuffer, 0);

            using (var screeni = new Bitmap(SCREEN_HEIGHT, SCREEN_WIDTH, 32,
                                            System.Drawing.Imaging.PixelFormat.Format1bppIndexed, screen))
            {
                screeni.RotateFlip(RotateFlipType.Rotate90FlipNone);

                var screend = screeni.LockBits(new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), 
                                            ImageLockMode.ReadOnly,
                                                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                                SCREEN_WIDTH, SCREEN_HEIGHT, 0,
                                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, screend.Scan0);
                screeni.UnlockBits(screend);
            }

            GL.Enable(EnableCap.Texture2D);

            float[] vertices =
            {
                //Position          Texture coordinates
                0.5f, 0.5f, 0.0f, 1.0f, 1.0f, // top right
                0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
                -0.5f, 0.5f, 0.0f, 0.0f, 1.0f // top left
            };

            var VertexBufferObject = GL.GenBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);
            

            window.SwapBuffers();
        }
    }
}
