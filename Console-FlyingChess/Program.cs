using System;

namespace Console_FlyingChess
{
    static class Program
    {
        public static void Main()
        {
            #region 初始化窗口

            const int wide = 50;
            const int high = 31;
            InitConsole(wide, high);

            #endregion

            #region 场景选择

            SceneType currentScene = SceneType.Begin;
            while (true)
            {
                switch (currentScene)
                {
                    case SceneType.Begin:
                        Console.Clear();
                        BeginScene(wide, ref currentScene);
                        break;
                    case SceneType.Game:
                        Console.Clear();
                        GameScene(wide, high);
                        break;
                    case SceneType.End:
                        Console.Clear();
                        break;
                }
            }

            #endregion
            
            // ReSharper disable once FunctionNeverReturns
        }

        #region 输出窗口

        private static void InitConsole(int wide, int high)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(wide, high);
            Console.SetBufferSize(wide, high);
        }

        #endregion

        #region 开始窗口

        private static void BeginScene(int wide, ref SceneType currentScene)
        {
            Console.SetCursorPosition(wide / 2 - 3, 8);
            Console.Write("飞行棋");

            int selectedIndex = 1;
            bool quitBeginScene = false;

            while (true)
            {
                Console.ForegroundColor = selectedIndex == 1 ? ConsoleColor.Red : ConsoleColor.White;
                Console.SetCursorPosition(wide / 2 - 4, 13);
                Console.Write("开始游戏");

                Console.ForegroundColor = selectedIndex == 0 ? ConsoleColor.Red : ConsoleColor.White;
                Console.SetCursorPosition(wide / 2 - 4, 15);
                Console.Write("退出游戏");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        selectedIndex++;
                        if (selectedIndex > 1) selectedIndex = 1;
                        break;
                    case ConsoleKey.S:
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = 0;
                        break;
                    case ConsoleKey.J:

                        if (selectedIndex == 1)
                        {
                            currentScene = SceneType.Game;
                            quitBeginScene = true;
                        }
                        else
                        {
                            Environment.Exit(0);
                        }

                        break;
                }

                if (quitBeginScene)
                {
                    break;
                }
            }
        }

        #endregion

        #region 游戏窗口

        private static void GameScene(int wide, int high)
        {
            DrawWall(wide, high);
          
            Map map = new Map(14, 3, 83);
            map.Draw();
            
            while (true)
            {
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void DrawWall(int wide, int high)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            for (int i = 2; i < wide - 2; i += 2)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");

                Console.SetCursorPosition(i, high - 12);
                Console.Write("■");

                Console.SetCursorPosition(i, high - 7);
                Console.Write("■");

                Console.SetCursorPosition(i, high - 2);
                Console.Write("■");
            }

            for (int i = 0; i < high - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");

                Console.SetCursorPosition(wide - 2, i);
                Console.Write("■");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(2, high - 11);
            Console.Write("□:普通格子");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition(2, high - 10);
            Console.Write("‖:暂停，一回合不懂");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(26, high - 10);
            Console.Write("●:炸弹，倒退5格");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(2, high - 9);
            Console.Write("¤:随机，随机倒退，暂停，交换位置");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(2, high - 8);
            Console.Write("★:玩家");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(12, high - 8);
            Console.Write("▲:电脑");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition(22, high - 8);
            Console.Write("◎:玩家和电脑重合");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(2, high - 6);
            Console.Write("按任意键开始扔色子");
        }

        #endregion
    }

    #region 场景相关

    internal enum SceneType
    {
        Begin,
        Game,
        End
    }

    #endregion

    #region 格子相关

    public enum GridType
    {
        NormalGrid,
        BoomGrid,
        PauseGrid,
        RandomGrid
    }

    public struct Vector2
    {
        public int X;
        public int Y;

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public struct Grid
    {
        public GridType Type;
        public Vector2 Position;

        public Grid(int x, int y, GridType type)
        {
            Position.X = x;
            Position.Y = y;
            Type = type;
        }

        public void Draw()
        {
            Console.SetCursorPosition(Position.X, Position.Y);
            switch (Type)
            {
                case GridType.NormalGrid:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("□");
                    break;
                case GridType.BoomGrid:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("●");
                    break;
                case GridType.PauseGrid:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("‖");
                    break;
                case GridType.RandomGrid:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("¤");
                    break;
            }
        }
    }

    #endregion

    #region 地图相关

    struct Map
    {
        public Grid[] grids;

        public Map(int startX, int startY, int amount)
        {
            grids = new Grid[amount];
            Random random = new Random();

            int indexX = 0;
            int indexY = 0;
            int stepX = 2;

            for (int i = 0; i < grids.Length; i++)
            {
                int percent = random.Next(0, 101);
                if (percent < 85 || i == 0 || i == grids.Length - 1)
                {
                    grids[i].Type = GridType.NormalGrid;
                }
                else if (percent > 85 && percent < 90)
                {
                    grids[i].Type = GridType.BoomGrid;
                }
                else if (percent > 90 && percent < 95)
                {
                    grids[i].Type = GridType.PauseGrid;
                }
                else if (percent > 95 && percent < 100)
                {
                    grids[i].Type = GridType.RandomGrid;
                }

                grids[i].Position = new Vector2(startX, startY);

                if (indexX < 10)
                {
                    indexX += 1;
                    startX += stepX;
                }
                else
                {
                    indexY += 1;
                    startY += 1;
                    if (indexY < 2) continue;
                    indexX = 0;
                    indexY = 0;
                    stepX = -stepX;
                }
            }
        }

        public void Draw()
        {
            foreach (Grid grid in grids)
            {
                grid.Draw();
            }
        }
    }

    #endregion
}