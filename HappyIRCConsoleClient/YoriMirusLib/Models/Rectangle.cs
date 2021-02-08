using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCConsoleClient.YoriMirusLib.Models
{
    public class Rectangle
    {
        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Creates a new rectangle object.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(int width, int height)
        {
            Height = height;
            Width = width;
        }

        /// <summary>
        /// Displays the rectangle to the console.
        /// </summary>
        /// <param name="cursorLeft">CursorLeft position of the top left corner of the rectangle, if this value is bigger than the window, nothing is displayed </param>
        /// <param name="cursorTop">CursorTop position of the top left corner of the rectangle, if this value is bigger than the window, nothing is displayed </param>
        /// <returns>Value whether the rectangle was successfully displayed, usually fails if the rectangle is too large, cursorLeft and cursorTop are too high or the screen was resized while displaying the rectangle</returns>
        public void Display(int cursorLeft, int cursorTop)
        {

            StringBuilder line = new StringBuilder();

            for (int i = 0; i < Width - 2; i++)
                line.Append('━');

            Console.SetCursorPosition(cursorLeft, cursorTop);

            //Writing top corner
            Console.Write('┏');
            Console.Write(line.ToString());
            Console.WriteLine('┓');

            //Writing left and right corners
            for (int i = 0; i < Height - 2; i++)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop + 1 + i);
                Console.Write('┃');
                Console.SetCursorPosition(cursorLeft + Width - 1, cursorTop + 1 + i);
                Console.Write('┃');
            }

            //Writing bottom corner
            Console.SetCursorPosition(cursorLeft, cursorTop + Height - 1);

            Console.Write('┗');
            Console.Write(line.ToString());
            Console.WriteLine('┛');
        }
    }
}
