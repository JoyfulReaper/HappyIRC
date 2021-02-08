using HappyIRCConsoleClient.YoriMirusLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCConsoleClient.YoriMirusLib.Models
{
    /// <summary>
    /// Base class for all menus that use the ConsoleCursor class to navigate among MenuOptions
    /// </summary>
    public abstract class MenuBase
    {
        public ConsoleCursor MenuCursor { get; protected set; }
        public List<MenuOption> MenuOptions { get; protected set; }

        protected bool redisplayMenu;
        protected bool shutdown;
        protected bool cursorRefresh;

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public abstract void Start();
        public abstract void Display();
        protected abstract void DisplayFrame();
        protected abstract void DisplaySelection();

        /// <summary>
        /// Sets the window size of the console to the menu window size.
        /// </summary>
        protected void SetWindowSize()
        {
            Console.SetWindowSize(WindowWidth , WindowHeight);
        }
    }
}
