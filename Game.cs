using System.Numerics;

namespace C__Test2DGame
{
    public class Game
    {
        static public int frameTarget;
        static public int fps;
        static int fpsTotalCount;
        static public int gameTicks;
        private static int totalTicksCount;
        static public double seconds = 1000;
        static public bool run = false;


        static public Vector2 player = new Vector2(15, 8);
        private static int playerSpeedHorizontal = 3;
        private static int playerSpeedVertical = 1;


        #region Enemy
        static public Vector2 enemy1Pos;

        #endregion Enemy

        private static double time;
        private static int windowHeight;
        private static int windowWidth;
        private static float enemySpeed = 0.5f;

        static public void Run()
        {
            run = true;

            Start();

            new Thread(() => //? Hilo a parte para las actualizaciones de los frames.
                {
                    Thread.CurrentThread.IsBackground = true;
                    Render();
                }).Start();

            Update();
        }

        static public void Start()
        {
            gameTicks = 60;
            seconds = 1000 / gameTicks;
            frameTarget = 60;
            fps = 1000 / frameTarget;

            time = 0;

            Console.CursorVisible = false;
            windowHeight = Console.WindowHeight;
            windowWidth = Console.WindowWidth;

            enemy1Pos = new Vector2(windowWidth/2, windowHeight/2);

            Console.WriteLine("Start");
            Console.Title = "TestGame";
        }

        static public void Update()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    totalTicksCount++;
                    if (enemy1Pos != player)
                    {
                        Thread.Sleep(100);
                        Vector2 direction = player - enemy1Pos;
                        direction = Vector2.Normalize(direction);
                        Vector2 displacement = direction * enemySpeed;
                        enemy1Pos += displacement;
                        //enemy1Pos = new Vector2(Convert.ToSingle(Math.Round(enemy1Pos.X)), Convert.ToSingle(Math.Round(enemy1Pos.Y)));
                    }
                }
            });

            while (run)
            {
                //? Calcular
                //Console.WriteLine("Update");

                while (run)
                {
                    // Comprobar entrada del usuario
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.W:
                                player += new Vector2(0, -1 * playerSpeedVertical);
                                break;

                            case ConsoleKey.A:
                                player += new Vector2(-1, 0 * playerSpeedHorizontal);
                                break;

                            case ConsoleKey.S:
                                player += new Vector2(0, 1 * playerSpeedVertical);
                                break;

                            case ConsoleKey.D:
                                player += new Vector2(1, 0 * playerSpeedHorizontal);
                                break;

                            default:
                                // Tecla no reconocida
                                break;
                        }
                    }
                }

                Thread.Sleep(Convert.ToInt32(seconds));
            }
        }

        static public void Render()
        {
            while (run)
            {
                Thread.Sleep(fps);
                fpsTotalCount++;
                Console.Clear();

                RenderOn(enemy1Pos, "&");
                RenderOn(player, "大");
                RenderOn(new Vector2(0, windowHeight-3), $"Enemy position: {enemy1Pos}\nEnemy speed: {enemySpeed}");
                RenderOn(new Vector2(0, windowHeight-1), $"Player Position{player}");
                RenderOn(new Vector2(20, 20),"*");

                //Console.WriteLine($"Ticks: {gameTicks} ({seconds}ms)\nTotal Ticks: {totalTicksCount}\nFps: {frameTarget} ({fps}ms)\nTotal FPS: {fpsTotalCount}");
                //Console.WriteLine("Frame render");
            }
        }

        static public void RenderOn(Vector2 pos, string sprite)
        {
            Console.SetCursorPosition(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y));
            Console.Write(sprite);
        }
    }
}