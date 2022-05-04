using System;
using static Console_FlyingChess.PlayerType;

namespace Console_FlyingChess
{
    public static class Program
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
                        GameScene(wide, high, ref currentScene);
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

        private static void GameScene(int wide, int high, ref SceneType currentScene)
        {
            // ReSharper disable once TooWideLocalVariableScope

            bool gameOver;

            DrawWall(wide, high);

            Map map = new Map(14, 3, 83);
            map.Draw();

            Player player = new Player(0, PlayerType.Player);
            Player computer = new Player(0, Computer);
            DrawPlayer(player, computer, map);

            while (true)
            {
                gameOver = MoveNext(wide, high, ref player, ref computer, map, ref currentScene);
                if (gameOver) break;
                gameOver = MoveNext(wide, high, ref computer, ref player, map, ref currentScene);
                if (gameOver) break;
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

        private static void DrawPlayer(Player player, Player computer, Map map)
        {
            if (player.IndexMap == computer.IndexMap)
            {
                Console.SetCursorPosition(map.Grids[player.IndexMap].Position.X, map.Grids[player.IndexMap].Position.Y);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("◎");
            }
            else
            {
                player.Draw(map);
                computer.Draw(map);
            }
        }

        private static bool RandomDice(int wide, int high, ref Player player1, ref Player player2, Map map)
        {
            ClearInfo(wide, high);
            Console.ForegroundColor =
                player1.PlayerType == PlayerType.Player ? ConsoleColor.Cyan : ConsoleColor.Magenta;
            if (player1.Pause)
            {
                Console.SetCursorPosition(2, high - 6);
                Console.Write("处于暂停状态，{0}需要暂停一回合", player1.PlayerType == PlayerType.Player ? "你" : "电脑");
                Console.SetCursorPosition(2, high - 5);
                Console.Write("请按任意键，让{0}开始扔色子", player2.PlayerType == Computer ? "电脑" : "你");

                player1.Pause = false;
                return false;
            }

            Random random = new Random();
            int randomNum = random.Next(1, 7);
            player1.IndexMap += randomNum;
            if (player1.IndexMap >= map.Grids.Length - 1)
            {
                Console.SetCursorPosition(2, high - 6);
                Console.Write(player1.PlayerType == PlayerType.Player ? "恭喜你，你率先到达了终点" : "很遗憾，电脑率到达了终点");
                Console.SetCursorPosition(2, high - 5);
                Console.Write("请按任意键结束游戏");

                player1.IndexMap = map.Grids.Length - 1;
                return true;
            }

            Console.SetCursorPosition(2, high - 6);
            Console.Write("{0}扔出的点数为:{1}", player1.PlayerType == PlayerType.Player ? "你" : "电脑", randomNum);

            switch (map.Grids[player1.IndexMap].Type)
            {
                case GridType.NormalGrid:
                    Console.SetCursorPosition(2, high - 5);
                    Console.Write("{0}到了一个安全位置", player1.PlayerType == PlayerType.Player ? "你" : "电脑");
                    Console.SetCursorPosition(2, high - 4);
                    Console.Write("请按任意键，让{0}开始扔色子", player1.PlayerType == PlayerType.Player ? "电脑" : "你");
                    break;
                case GridType.BoomGrid:
                    player1.IndexMap -= 5;
                    if (player1.IndexMap <= 0) player1.IndexMap = 0;

                    Console.SetCursorPosition(2, high - 5);
                    Console.Write("{0}踩到了炸弹，退后5格", player1.PlayerType == PlayerType.Player ? "你" : "电脑");
                    Console.SetCursorPosition(2, high - 4);
                    Console.Write("请按任意键，让{0}开始扔色子", player1.PlayerType == PlayerType.Player ? "电脑" : "你");

                    break;
                case GridType.PauseGrid:
                    player1.Pause = true;

                    Console.SetCursorPosition(2, high - 5);
                    Console.Write("{0}到达了暂停点，你需要暂停一回合", player1.PlayerType == PlayerType.Player ? "你" : "电脑");
                    Console.SetCursorPosition(2, high - 4);
                    Console.Write("请按任意键，让{0}开始扔色子", player2.PlayerType == Computer ? "电脑" : "你");
                    break;
                case GridType.RandomGrid:

                    Console.SetCursorPosition(2, high - 5);
                    Console.Write("{0}踩到了时空隧道", player1.PlayerType == PlayerType.Player ? "你" : "电脑");

                    randomNum = random.Next(0, 3);
                    switch (randomNum)
                    {
                        case 0:
                            player1.IndexMap -= 5;

                            Console.SetCursorPosition(2, high - 4);
                            Console.Write("触发倒退5格");
                            break;
                        case 1:
                            player1.Pause = true;

                            Console.SetCursorPosition(2, high - 4);
                            Console.Write("触发暂停一回合");
                            break;
                        case 2:
                            (player1.IndexMap, player2.IndexMap) = (player2.IndexMap, player1.IndexMap);

                            Console.SetCursorPosition(2, high - 4);
                            Console.Write("惊喜，惊喜，双方交换位置");

                            Console.SetCursorPosition(2, high - 3);
                            Console.Write("请按任意键，让{0}开始扔色子", player1.PlayerType == PlayerType.Player ? "电脑" : "你");

                            break;
                    }

                    break;
            }

            return false;
        }

        private static void ClearInfo(int wide, int high)
        {
            Console.SetCursorPosition(2, high - 6);
            for (int i = 0; i < wide - 4; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(2, high - 5);
            for (int i = 0; i < wide - 4; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(2, high - 4);
            for (int i = 0; i < wide - 4; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(2, high - 3);
            for (int i = 0; i < wide - 4; i++)
            {
                Console.Write(" ");
            }
        }

        private static bool MoveNext(int wide, int high, ref Player player1, ref Player player2, Map map,
            ref SceneType currentScene)
        {
            Console.ReadKey(true);
            bool gameOver = RandomDice(wide, high, ref player1, ref player2, map);
            map.Draw();
            DrawPlayer(player1, player2, map);
            
            if (!gameOver) return false;
            Console.ReadKey(true);
            currentScene = SceneType.End;
            return true;
        }

        #endregion
    }

    #region 场景相关

    public enum SceneType
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

    public readonly struct Map
    {
        public readonly Grid[] Grids;

        public Map(int startX, int startY, int amount)
        {
            Grids = new Grid[amount];
            Random random = new Random();

            int indexX = 0;
            int indexY = 0;
            int stepX = 2;

            for (int i = 0; i < Grids.Length; i++)
            {
                int percent = random.Next(0, 101);
                if (percent < 85 || i == 0 || i == Grids.Length - 1)
                {
                    Grids[i].Type = GridType.NormalGrid;
                }
                else if (percent > 85 && percent < 90)
                {
                    Grids[i].Type = GridType.BoomGrid;
                }
                else if (percent > 90 && percent < 95)
                {
                    Grids[i].Type = GridType.PauseGrid;
                }
                else if (percent > 95 && percent < 100)
                {
                    Grids[i].Type = GridType.RandomGrid;
                }

                Grids[i].Position = new Vector2(startX, startY);

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
            foreach (Grid grid in Grids)
            {
                grid.Draw();
            }
        }
    }

    #endregion

    #region 玩家相关

    public enum PlayerType
    {
        Player,
        Computer
    }

    public struct Player
    {
        public readonly PlayerType PlayerType;
        public int IndexMap;
        public bool Pause;

        public Player(int indexMap, PlayerType playerType)
        {
            IndexMap = indexMap;
            PlayerType = playerType;
            Pause = false;
        }

        public void Draw(Map map)
        {
            Console.SetCursorPosition(map.Grids[IndexMap].Position.X, map.Grids[IndexMap].Position.Y);

            if (PlayerType == PlayerType.Player)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("★");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("▲");
            }
        }
    }

    #endregion
}