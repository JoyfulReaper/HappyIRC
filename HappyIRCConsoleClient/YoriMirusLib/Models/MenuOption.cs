using System;
using System.Collections.Generic;
using System.Text;

using HappyIRCConsoleClient.YoriMirusLib.Helpers;

namespace HappyIRCConsoleClient.YoriMirusLib.Models
{
    /// <summary>
    /// Class for making selections in the menu.
    /// </summary>
    public class MenuOption
    {
        /// <summary>
        /// Background color of the option when it is selected.
        /// </summary>
        public ConsoleColor SelectedBG    { get; private set; }
        /// <summary>
        /// Background color of the option when it isn't selected.
        /// </summary>
        public ConsoleColor NotSelectedBG { get; private set; }
        /// <summary>
        /// Foreground color of the option when it is selected.
        /// </summary>
        public ConsoleColor SelectedFG    { get; private set; }
        /// <summary>
        /// Background color of the option when it isn't selected.
        /// </summary>
        public ConsoleColor NotSelectedFG { get; private set; }
        
        /// <summary>
        /// CursorTop index for the option to be selected
        /// </summary>
        public int SelectTopIndex  { get; private set; }
        /// <summary>
        /// CursorLeft index for the option to be selected
        /// </summary>
        public int SelectLeftIndex { get; private set; }

        /// <summary>
        /// Text to display.
        /// </summary>
        public string Text { get; private set; }

        public MenuOption(string text, int selectTopIndex, int selectLeftIndex = 0, ConsoleColor selectedBG = ConsoleColor.Blue, ConsoleColor notSelectedBG = ConsoleColor.Black, ConsoleColor selectedFG = ConsoleColor.Gray , ConsoleColor notSelectedFG = ConsoleColor.Gray)
        {
            SelectedBG = selectedBG;
            SelectedFG = selectedFG;

            NotSelectedBG = notSelectedBG;
            NotSelectedFG = notSelectedFG;

            SelectTopIndex = selectTopIndex;
            SelectLeftIndex = selectLeftIndex;
            Text = text;
        }

        /// <summary>
        /// Sets the text that this option displays.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            Text = text;
        }
        /// <summary>
        /// Sets the text that this option displays only if the text is selected (cursorLeft == SelectedLeftIndex)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cursorLeft">CursorLeft index of the cursor.</param>
        /// <param name="cursorTop"></param>
        /// <returns>Whether the text was changed or not</returns>
        public bool SetText(string text, int cursorLeft, int cursorTop)
        {
            if (cursorLeft == SelectLeftIndex && cursorTop == SelectTopIndex)
            {
                Text = text;
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Sets the SelectLeftIndex and SelectTopIndex
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="topIndex"></param>
        public void ChangeSelectedIndex(int leftIndex, int topIndex)
        {
            SelectLeftIndex = leftIndex;
            SelectTopIndex = topIndex;
        }
        /// <summary>
        /// Changes the colors of the selection that are used when it is selected
        /// </summary>
        /// <param name="newFG">New foreground color</param>
        /// <param name="newBG">New background color</param>
        public void ChangeSelectedColors(ConsoleColor newFG, ConsoleColor newBG)
        {
            SelectedBG = newBG;
            SelectedFG = newFG;
        }
        /// <summary>
        /// Changes the colors of the selection that are used when it is not selected
        /// </summary>
        /// <param name="newFG">New foreground color</param>
        /// <param name="newBG">New background color</param>
        public void ChangeNotSelectedColors(ConsoleColor newFG, ConsoleColor newBG)
        {
            NotSelectedBG = newBG;
            NotSelectedFG = newFG;
        }
        /// <summary>
        /// Writes the selection into the center of the screen
        /// </summary>
        /// <param name="cursorLeftIndex">CursorLeft index of the cursor</param>
        /// <param name="cursorTopIndex">CursorTop index of the cursor</param>
        /// <param name="cursorTop"></param>
        public void WriteInCenter(int cursorLeftIndex, int cursorTopIndex)
        {
            ConsoleColor prevBG = Console.BackgroundColor;

            if(cursorLeftIndex == SelectLeftIndex && cursorTopIndex == SelectTopIndex)
            {
                Console.BackgroundColor = SelectedBG;
                ConsoleHelper.WriteInCenter(Text, SelectedFG, Console.CursorTop);
                Console.BackgroundColor = prevBG;
            }
            else
            {
                Console.BackgroundColor = NotSelectedBG;
                ConsoleHelper.WriteInCenter(Text, NotSelectedFG, Console.CursorTop);
                Console.BackgroundColor = prevBG;
            }
        }
        /// <summary>
        /// Writes the selection into the console and jumps to the next line
        /// </summary>
        /// <param name="cursorLeftIndex">CursorLeft index of the cursor</param>
        /// <param name="cursorTopIndex">CursorTop index of the cursor</param>
        public void Display(int cursorLeftIndex, int cursorTopIndex)
        {
            ConsoleColor prevFG = Console.ForegroundColor;
            ConsoleColor prevBG = Console.BackgroundColor;

            if(cursorLeftIndex == SelectLeftIndex && cursorTopIndex == SelectTopIndex)
            {
                Console.ForegroundColor = SelectedFG;
                Console.BackgroundColor = SelectedBG;
                Console.WriteLine(Text);
            }
            else
            {
                Console.ForegroundColor = NotSelectedFG;
                Console.BackgroundColor = NotSelectedBG;
                Console.WriteLine(Text);
            }
            Console.ForegroundColor = prevFG;
            Console.BackgroundColor = prevBG;
        }
    }
}
