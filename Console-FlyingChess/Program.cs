using System;

namespace Console_FlyingChess
{
    internal static class Program
    {
        public static void Main()
        {
            #region 初始化窗口
             int wide = 50;
             int high = 30;
            InitConsole(wide,high);

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
                        break;
                    case SceneType.End:
                        Console.Clear();
                        break;
                }
            }
            
            #endregion
            
            
            // ReSharper disable once FunctionNeverReturns
        }

        private static void InitConsole(int wide,int high)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(wide,high);
            Console.SetBufferSize(wide,high);
        }

        private enum SceneType
        {
            Begin,
            Game,
            End
        }

        private static void BeginScene(int wide, ref SceneType currentScene)
        {
            
            Console.SetCursorPosition(wide/2-3,8);
            Console.Write("飞行棋");
            
            int selectedIndex=1;
            bool quitBeginScene=false;
            
            while (true)
            {
                
                Console.ForegroundColor = selectedIndex == 1 ? ConsoleColor.Red : ConsoleColor.White;
                Console.SetCursorPosition(wide/2-4,13);
                Console.Write("开始游戏");
            
                Console.ForegroundColor = selectedIndex == 0 ? ConsoleColor.Red : ConsoleColor.White;
                Console.SetCursorPosition(wide/2-4,15);
                Console.Write("退出游戏");
                
                switch (Console.ReadKey(true).Key)
                {
                    case  ConsoleKey.W:
                        selectedIndex++;
                        if (selectedIndex > 1) selectedIndex = 1;
                        break;  
                    case  ConsoleKey.S:
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = 0;
                        break;    
                    case  ConsoleKey.J:

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
    }
}