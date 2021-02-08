using System;
using System.Text;

namespace HappyIRCConsoleClient.YoriMirusLib.Helpers
{
    /// <summary>
    /// Contains methods to make creating menus easier
    /// </summary>
    public static class ConsoleHelper
    {
        #region WriteLine methods
        /// <summary>
        /// Writes text into the console on a specific position with a specific color.
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="fontColor">Color of the text</param>
        /// <param name="cursorLeft">Starting CursorLeft position</param>
        /// <param name="cursorTop">Starting CursorTop position</param>
        public static void WriteLine(string text, ConsoleColor fontColor)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = fontColor;
            Console.WriteLine(text);
            Console.ForegroundColor = previousColor;
        }
        #endregion
        #region WriteInCenter methods
        /// <summary>
        /// Writes text in the center of the screen on the current line and jumps to the next one.
        /// </summary>
        /// <param name="text"></param>
        public static void WriteInCenter(string text) 
            => WriteInCenter(text, Console.ForegroundColor, Console.CursorTop);
        /// <summary>
        /// Writes text in the center of the screen on the current line and jumps to the next one.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        public static void WriteInCenter(string text, ConsoleColor fontColor)
            => WriteInCenter(text, fontColor, Console.CursorTop);
        /// <summary>
        /// Writes text into the center of the console in a specific color and jumps to the next line. After writing returns back to the previous color.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        /// <param name="cursorTop"></param>
        public static void WriteInCenter(string text, ConsoleColor fontColor, int cursorTop)
        {
            ConsoleColor previousColor = Console.ForegroundColor;

            Console.ForegroundColor = fontColor;
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, cursorTop);

            Console.WriteLine(text);

            Console.ForegroundColor = previousColor;
        }

        #endregion
        #region FillALine methods

        /// <summary>
        /// Fills one line with text
        /// </summary>
        /// <param name="text"></param>
        public static void FillALine(string text) => FillALine(text, 0, 0, Console.ForegroundColor);

        /// <summary>
        /// Fills one line with text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        public static void FillALine(string text, ConsoleColor fontColor = ConsoleColor.Gray) => FillALine(text, 0, 0, fontColor);

        /// <summary>
        /// Fills one line with text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        /// <param name="margin">How many spaces to leave out on the right and left side of the line</param>
        public static void FillALine(string text, int margin, ConsoleColor fontColor = ConsoleColor.Gray)
            => FillALine(text, margin, margin, fontColor);

        /// <summary>
        /// Fills one line with text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        /// <param name="leftMargin">How many spaces to leave on the left side of the line.</param>
        /// <param name="rightMargin">How many spaces to leave on the right side of the line.</param>
        public static void FillALine(string text, int leftMargin, int rightMargin, ConsoleColor fontColor = ConsoleColor.Gray)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            StringBuilder textToDisplay = new StringBuilder();

            int appendCount = (Console.WindowWidth - leftMargin - rightMargin) / text.Length;

            //Add +1 to the condition field in case (Console.WindowWidth - leftMargin - rightMargin) % text.Length > 0
            for (int i = 0; i < appendCount + 1; i++)
            {
                textToDisplay.Append(text);
            }
            textToDisplay.Remove(Console.WindowWidth - leftMargin - rightMargin - 1, text.Length);

            Console.SetCursorPosition(leftMargin, Console.CursorTop);
            Console.ForegroundColor = fontColor;
            Console.WriteLine(textToDisplay.ToString());
            Console.ForegroundColor = previousColor;
        }

        /// <summary>
        /// Fills one line with text with unique character on the edges of the line.
        /// </summary>
        /// <param name="centerText"></param>
        /// <param name="leftEdge"></param>
        /// <param name="rightEdge"></param>
        /// <param name="margin"></param>
        /// <param name="fontColor"></param>
        public static void FillALine(string centerText, char leftEdge, char rightEdge, int margin, ConsoleColor fontColor = ConsoleColor.Gray)
        {
            int top = Console.CursorTop;
            MakeEdges(leftEdge.ToString(), rightEdge.ToString(), margin);

            Console.SetCursorPosition(0, top);
            FillALine(centerText, margin + 1);
        }
        #endregion
        #region MakeEdges methods

        /// <summary>
        /// Writes text into the edges of the console and jumps to the next line.
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void MakeEdges(string text, ConsoleColor fontColor = ConsoleColor.Gray) => MakeEdges(text, text, 0, 0, fontColor);
        /// <summary>
        /// Writes text into the edges of the console and jumps to the next line.
        /// </summary>
        /// <param name="leftEdge"></param>
        /// <param name="rightEdge"></param>
        /// <param name="margin"></param>
        public static void MakeEdges(string leftEdge, string rightEdge, int margin) => MakeEdges(leftEdge, rightEdge, margin, margin);
        /// <summary>
        /// Writes text into the edges of the console and jumps to the next line.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void MakeEdges(string leftEdge, string rightEdge, int leftMargin, int rightMargin, ConsoleColor fontColor = ConsoleColor.Gray)
        {
            ConsoleColor previousColor = Console.ForegroundColor;

            if (leftEdge.Length + rightEdge.Length > Console.WindowWidth)
                throw new ArgumentOutOfRangeException("text", "Text is too large to fit inside the line.");

            Console.ForegroundColor = fontColor;
            Console.SetCursorPosition(leftMargin, Console.CursorTop);
            Console.Write(leftEdge);

            Console.SetCursorPosition(Console.WindowWidth - rightEdge.Length - rightMargin, Console.CursorTop);

            Console.WriteLine(rightEdge);
            Console.ForegroundColor = previousColor;
        }

        #endregion
        #region MakeFrame methods

        /// <summary>
        /// Fills the edges of the window with a specific character. Used for making menus.
        /// </summary>
        /// <param name="leftRightEdge"></param>
        /// <param name="topBottomEdge"></param>
        public static void MakeFrame(char leftRightEdge, char topBottomEdge) => MakeFrame(leftRightEdge, topBottomEdge, Console.ForegroundColor);
        /// <summary>
        /// Fills the edges of the window with a specific character. Used for making menus.
        /// </summary>
        /// <param name="leftRightEdge"></param>
        /// <param name="topBottomEdge"></param>
        /// <param name="frameColor"></param>
        public static void MakeFrame(char leftRightEdge, char topBottomEdge, ConsoleColor frameColor)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = frameColor;

            FillALine(topBottomEdge.ToString());
            for(int i = 1; i < Console.WindowHeight - 2; i++)
            {
                MakeEdges(leftRightEdge.ToString());
            }
            FillALine(topBottomEdge.ToString());

            Console.ForegroundColor = previousColor;
        }

        #endregion
    }
}
