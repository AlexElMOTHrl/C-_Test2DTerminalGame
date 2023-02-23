using System.Numerics;

namespace C__Test2DGame
{
    public class Game
    {
        #region GameConfig
        private static bool isDebugEnabled = false;
        static public int score = 0;
        static public string deathMsg = "Has muerto";
        static public string restartMsg = "Presiona cualquier tecla para terminar.";
        static public string deathMsgAnimation = string.Empty;
        static public int frameTarget;
        static public int fps;
        static int fpsTotalCount;
        static public int gameTicks;
        static public int shootTick;
        private static int totalTicksCount;
        static public double seconds = 1000;
        static public bool run = false;
        static public bool canRender;
        //private static double time;
        private static int windowHeight;
        private static int windowWidth;
        #endregion GameConfig

        #region Player
        static public Vector2 player = new Vector2(15, 8);
        private static int playerSpeedHorizontal = 2;
        private static int playerSpeedVertical = 1;
        private static bool isDead = false;
        #endregion Player

        #region Enemy
        static public Vector2 enemy1Pos;
        private static Vector2 bullet;
        private static bool bulletVisible = false;
        private static float bulletSpeed = 1f;
        private static bool stopBullet;
        private static Vector2 roundBulletPosition;
        private static float enemySpeed = 1f;
        #endregion Enemy

        #region Animations
        private static string? restartMsgAnimation;
        private static string scoreMsg = "Score: ";
        private static string? scoreMsgAnimation;
        #endregion

        public static string map = "";

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
            isDead = false;
            score = 0;
            run = true;

            gameTicks = 64;
            seconds = 1000 / gameTicks;
            frameTarget = 144;
            fps = 1000 / frameTarget;

            deathMsgAnimation = string.Empty;
            restartMsgAnimation = string.Empty;
            scoreMsgAnimation = string.Empty;

            Console.CursorVisible = true;
            canRender = true;
            windowHeight = Console.WindowHeight;
            windowWidth = Console.WindowWidth;


            enemy1Pos = new Vector2(windowWidth / 1.2f, windowHeight / 1.2f);
            enemySpeed = 0.5f;
            bulletSpeed = 1f;
            bulletVisible = false;

            Random rnd = new Random();
            //player = new Vector2(rnd.Next(-10, windowWidth - 10), rnd.Next(-10, windowHeight - 10));

            Console.WriteLine("Start");
            Console.Title = "TestGame";
        }

        static public void Update()
        {
            Task.Run(() =>
            {
                Random rnd = new Random();
                Vector2 offSet = new Vector2(0, 0);

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
                        enemySpeed += 0.01f;
                    }

                    shootTick++;

                    if (bulletVisible == false && shootTick > 7 && !isDead)
                    {
                        CastBullet();
                        offSet = new Vector2(rnd.Next(-20, 20), rnd.Next(-10, 10));
                        shootTick = 0;
                    }
                }
            });

            while (run)
            {
                //? Calcular
                score = totalTicksCount;
                // Comprobar entrada del usuario
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.W:
                            player += new Vector2(0, -1 * playerSpeedVertical);
                            if (player.Y < 0) { player -= new Vector2(0, -1 * playerSpeedVertical); }
                            break;

                        case ConsoleKey.A:
                            player += new Vector2(-1 * playerSpeedHorizontal, 0);
                            if (player.X < 0) { player -= new Vector2(-1 * playerSpeedHorizontal, 0); }
                            break;

                        case ConsoleKey.S:
                            player += new Vector2(0, 1 * playerSpeedVertical);
                            if (player.Y > windowHeight - 1) { player -= new Vector2(0, 1 * playerSpeedVertical); }
                            break;

                        case ConsoleKey.D:
                            player += new Vector2(1 * playerSpeedHorizontal, 0);
                            if (player.X > windowWidth - 1) { player -= new Vector2(1 * playerSpeedHorizontal, 0); }
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

                if (!isDead)
                {
                    // Renderizar enemigo, jugador, bala y datos
                    RenderOn(new Vector2(windowWidth / 2 - 88 / 2, windowHeight / 2 - 18 / 2), map, true);

                    Console.ForegroundColor = ConsoleColor.Blue;
                    RenderOn(player, "i", true);
                    Console.ForegroundColor = ConsoleColor.Red;
                    RenderOn(enemy1Pos, "Ç", true);
                    RenderOn(bullet, "*", bulletVisible);
                    Console.ResetColor();

                    RenderOn(new Vector2(2, 1), $"Puntos: {score}", true);

                    if (isDebugEnabled)
                    {
                        RenderOn(new Vector2(0, windowHeight - 6), $"Ejecutando?: {run}", true);
                        RenderOn(new Vector2(0, windowHeight - 5), Convert.ToString(shootTick), true);
                        RenderOn(new Vector2(0, windowHeight - 4), $"Enemy position: {enemy1Pos}\nEnemy speed: {enemySpeed}", true);
                        RenderOn(new Vector2(0, windowHeight - 2), $"Bullet TRUNCATED Position{roundBulletPosition}", true);
                        RenderOn(new Vector2(0, windowHeight - 1), $"Player Position: {player}", true);
                    }

                }
                else if (isDead)
                {
                    scoreMsg = scoreMsg + Convert.ToString(score);

                    Console.Beep(262, 200);
                    Console.Beep(262 / 2, 500);

                    Thread.Sleep(100);
                    AnimatedText(deathMsg, new Vector2(windowWidth / 2 - deathMsg.Length / 2, windowHeight / 2 - 2), 50, true);
                    Thread.Sleep(100);
                    AnimatedText(scoreMsg, new Vector2(windowWidth / 2 - scoreMsg.Length / 2, windowHeight / 2), 50, true);
                    Thread.Sleep(100);
                    AnimatedText(restartMsg, new Vector2(windowWidth / 2 - restartMsg.Length / 2, windowHeight / 2 + 2), 25, true);

                    Console.ReadKey(true);
                    Console.Clear();

                    Environment.Exit(1);
                }
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

            Console.Beep(220, 100);

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
                    if (roundBulletPosition != player)
                    {
                        Thread.Sleep(15);

                        direction = Vector2.Normalize(direction);
                        Vector2 displacement = direction * bulletSpeed;
                        bullet += displacement;
                        roundBulletPosition = new Vector2((float)Math.Truncate(bullet.X), (float)Math.Truncate(bullet.Y));
                    }
                    else
                    {
                        // La tarea ha terminado, así que cancelamos el token.
                        bulletVisible = false;
                        stopBullet = true;
                        isDead = true;
                        //run = false;
                        cancellationTokenSource.Cancel();
                    }
                }
            }, cancellationTokenSource.Token);
        }

        static public void AnimatedText(string text, Vector2 position, int speed, bool showCursor)
        {
            string msgAnimation = string.Empty;

            if (showCursor) { Console.CursorVisible = true; } else { Console.CursorVisible = false; }

            for (int i = 0; i < text.Length; i++)
            {
                msgAnimation += text[i];
                RenderOn(position, msgAnimation, true);
                Thread.Sleep(speed);
            }

            Console.CursorVisible = false;
        }

        static public string CalculateMapSize(int _windowWidth, int _windowHeight, float percentageReduction)
        {
            string width = string.Empty;
            string height = string.Empty;
            float heightReal;
            string margin = string.Empty;
            string insideMargin = string.Empty;
            string map = string.Empty;

            heightReal = _windowHeight * percentageReduction / 2;

            for (int i = 0; i < _windowWidth * percentageReduction; i++)
            {
                width += '#';
            }

            for (int i = 0; i < _windowWidth / 2 - width.Length / 2; i++)
            {
                margin += ' ';
            }

            /* for (int i = 0; i < 4; i++)
            {
                insideMargin += ' ';
            } */

            for (int i = 0; i < 2; i++)
            {
                height += margin + "#" + "noo" + "#\n";
            }

            map += width + "\n";
            map += height;

            RenderOn(new Vector2(_windowWidth / 2 - width.Length / 2, _windowHeight / 2 - height.Length / 2), map, true); //!Debug

            RenderOn(new Vector2(0, _windowHeight - 5), $"_Width: {_windowWidth}", true);
            RenderOn(new Vector2(0, _windowHeight - 4), $"Width: {width.Length}", true);
            RenderOn(new Vector2(0, _windowHeight - 3), $"_Height: {_windowHeight}", true);
            RenderOn(new Vector2(0, _windowHeight - 2), $"Debug map: {_windowWidth * percentageReduction}", true);

            return map;
        }
    }
}