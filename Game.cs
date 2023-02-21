using System.Numerics;

namespace C__Test2DGame
{
    public class Game
    {
        #region GameConfig
        static public int frameTarget;
        static public int fps;
        static int fpsTotalCount;
        static public int gameTicks;
        static public int shootTick;
        private static int totalTicksCount;
        static public double seconds = 1000;
        static public bool run = false;
        static public bool canRender;
        private static double time;
        private static int windowHeight;
        private static int windowWidth;
        #endregion GameConfig

        #region Player
        static public Vector2 player = new Vector2(15, 8);
        private static int playerSpeedHorizontal = 2;
        private static int playerSpeedVertical = 1;
        #endregion Player

        #region Enemy
        static public Vector2 enemy1Pos;
        private static Vector2 bullet;
        private static bool bulletVisible = false;
        private static float bulletSpeed = 3f;
        private static bool stopBullet;
        private static float enemySpeed = 1f;
        #endregion Enemy

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
            gameTicks = 64;
            seconds = 1000 / gameTicks;
            frameTarget = 144;
            fps = 1000 / frameTarget;

            time = 0;

            Console.CursorVisible = false;
            canRender = true;
            windowHeight = Console.WindowHeight;
            windowWidth = Console.WindowWidth;

            enemy1Pos = new Vector2(windowWidth / 2, windowHeight / 2);
            bulletVisible = false;

            Console.WriteLine("Start");
            Console.Title = "TestGame";
        }

        static public void Update()
        {
            Task.Run(() =>
            {
                Random rnd = new Random();
                Vector2 offSet = new Vector2(0,0);

                while (true)
                {
                    totalTicksCount++;
                    if (enemy1Pos != player)
                    {
                        Thread.Sleep(100);
                        Vector2 direction = player + offSet - enemy1Pos;
                        direction = Vector2.Normalize(direction);
                        Vector2 displacement = direction * enemySpeed;
                        enemy1Pos += displacement;
                    }

                    shootTick++;

                    if (bulletVisible == false && shootTick > 5)
                    {
                        CastBullet();
                        offSet = new Vector2(rnd.Next(-20,20), rnd.Next(-10,10));
                        shootTick = 0;
                    }
                }
            });

            while (run)
            {
                //? Calcular
                //Console.WriteLine("Update");

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
                            player += new Vector2(-1 * playerSpeedHorizontal, 0);
                            break;

                        case ConsoleKey.S:
                            player += new Vector2(0, 1 * playerSpeedVertical);
                            break;

                        case ConsoleKey.D:
                            player += new Vector2(1 * playerSpeedHorizontal, 0);
                            break;

                        default:
                            //CastBullet();
                            break;
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

                // Renderizar enemigo, jugador, bala y datos
                Console.ForegroundColor = ConsoleColor.Blue;
                RenderOn(player, "i", true);
                Console.ForegroundColor = ConsoleColor.Red;
                RenderOn(enemy1Pos, "Ç", true);
                RenderOn(bullet, "*", bulletVisible);
                Console.ResetColor();

                //RenderOn(new Vector2(0, windowHeight - 4), Convert.ToString(shootTick), true);
                //RenderOn(new Vector2(0, windowHeight - 3), $"Enemy position: {enemy1Pos}\nEnemy speed: {enemySpeed}", true);
                //RenderOn(new Vector2(0, windowHeight - 1), $"Player Position{player}", true);

                canRender = false;

                // Mostrar información de rendimiento
                //Console.WriteLine($"Ticks: {gameTicks} ({seconds}ms)\nTotal Ticks: {totalTicksCount}\nFps: {frameTarget} ({fps}ms)\nTotal FPS: {fpsTotalCount}");
                //Console.WriteLine("Frame render");
            }
        }

        static public void RenderOn(Vector2 pos, string sprite, bool visible)
        {
            if (visible)
            {
                Console.SetCursorPosition(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y));
                Console.Write(sprite);
            }
        }

        static public void CastBullet()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                bullet = enemy1Pos;
                bulletVisible = true;
                Vector2 oldPlayerPos = player;
                Vector2 direction = oldPlayerPos - bullet;
                while (true)
                {
                    if (bullet.X < 1 || bullet.X > windowWidth - 1 || bullet.Y < 1 || bullet.Y > windowHeight - 1)
                    {
                        bulletVisible = false;
                        cancellationTokenSource.Cancel();
                    }

                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        // La tarea ha sido cancelada, así que salimos del bucle.
                        break;
                    }

                    totalTicksCount++;
                    if (enemy1Pos != player)
                    {
                        Thread.Sleep(50);

                        direction = Vector2.Normalize(direction);
                        Vector2 displacement = direction * bulletSpeed;
                        bullet += displacement;
                    }
                    else
                    {
                        // La tarea ha terminado, así que cancelamos el token.
                        stopBullet = true;
                        cancellationTokenSource.Cancel();
                    }
                }
            }, cancellationTokenSource.Token);
        }
    }
}