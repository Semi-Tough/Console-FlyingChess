using System;

namespace Console_FlyingChess
{
    internal static class Program
    {
        public static void Main()
        {
            #region 初始化窗口

            const int wide = 50;
            const int high = 30;
            InitConsole(wide,high);

            #endregion

        }

        private static void InitConsole(int wide,int high)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(wide,high);
            Console.SetBufferSize(wide,high);
        }
    }
}