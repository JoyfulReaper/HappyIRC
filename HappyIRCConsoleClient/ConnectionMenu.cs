﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HappyIRCConsoleClient.YoriMirusLib.Models;
using HappyIRCConsoleClient.YoriMirusLib.Helpers;
using HappyIRCConsoleClient.YoriMirusLib.Enums;

namespace HappyIRCConsoleClient
{
    class ConnectionMenu: MenuBase
    {
        /// <summary>
        /// Name of the server the client connects to.
        /// </summary>
        private string serverName;
        /// <summary>
        /// Username after connecting to the server.
        /// </summary>
        private string userName;

        public ConnectionMenu()
        {
            /* NOTE: I will have to rewrite this in the future.
             * The ConsoleImplementation.dll file is from another project of mine, not exactly made for this project.
             * I will make classes better suited for this project in the near future, right now I'm just experimenting.
             */

            //Set up the window size
            WindowHeight = 18;
            WindowWidth = 50;
            Console.SetWindowSize(WindowWidth, WindowHeight);

            cursorRefresh = true;
            redisplayMenu = true;
            shutdown = false;

            serverName = "";
            userName = "";

            MenuCursor = new ConsoleCursor(0, 2);
            MenuOptions = new List<MenuOption>()
            {
                new MenuOption("", 0), //Server name selection
                new MenuOption("", 1), //User name selection
                new MenuOption("Connect to server", 2)
            };

            Start();
        }
        public override void Start()
        {
            while (!shutdown)
            {
                ReadInput();
            }
        }
        private void ReadInput()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);

                    switch (cki.Key)
                    {
                        //Keys for cursor movement
                        case ConsoleKey.UpArrow:
                            MenuCursor.MoveCursor(CursorMovementDirection.Up);
                            cursorRefresh = true;
                            break;
                        case ConsoleKey.DownArrow:
                            MenuCursor.MoveCursor(CursorMovementDirection.Down);
                            cursorRefresh = true;
                            break;

                        //Keys for inputting text
                        case ConsoleKey.Delete:
                            if (MenuCursor.TopIndex == 0)
                                serverName = "";
                            else if (MenuCursor.TopIndex == 1)
                                userName = "";
                            cursorRefresh = true;
                            break;
                        case ConsoleKey.Spacebar:
                            if (MenuCursor.TopIndex == 0)
                                serverName += " ";
                            else if (MenuCursor.TopIndex == 1)
                                userName += " ";
                            cursorRefresh = true;
                            break;
                        case ConsoleKey.Backspace:
                            if (MenuCursor.TopIndex == 0)
                                serverName = serverName.Remove(serverName.Length - 1);
                            else if (MenuCursor.TopIndex == 1)
                                userName = userName.Remove(userName.Length - 1);
                            cursorRefresh = true;
                            break;
                        default:
                            //Get the inputted character, convert to string and get rid of whitespaces (in case user inputted Del, Home or something else)
                            string input = cki.KeyChar.ToString().Trim();
                            if (MenuCursor.TopIndex == 0)
                                serverName += input;
                            else if (MenuCursor.TopIndex == 1)
                                userName += input;
                            cursorRefresh = true;
                            break;
                    }
                }
                Display();

                System.Threading.Thread.Sleep(50);
            }
        }


        public override void Display()
        {
            if (redisplayMenu)
                DisplayFrame();
            if (cursorRefresh)
                DisplaySelection();
        }
        protected override void DisplayFrame()
        {
            //Display border of the screen
            Rectangle r = new Rectangle(WindowWidth - 1, WindowHeight - 1);
            r.Display(0, 0);

            //HappyIRC text
            Console.SetCursorPosition(0, 2);
            ConsoleHelper.WriteInCenter("HappyIRC");

            //Make a line below the "HappyIRC" text.
            //TODO: Simplify code by making a new method in the dll file.
            Console.SetCursorPosition(0, 4);
            Console.Write('┠');
            ConsoleHelper.FillALine("━", 1);
            Console.SetCursorPosition(Console.WindowWidth - 2, 4);
            Console.WriteLine('┫');

            //"Connect to a server" text
            Console.SetCursorPosition(0, 6);
            ConsoleHelper.WriteInCenter("Connect to a server.");

            //Text next to the input fields.
            Console.SetCursorPosition(3, 8);
            Console.WriteLine("Server name: ");

            Console.SetCursorPosition(3, 10);
            Console.WriteLine("User name: ");

            redisplayMenu = false;
        }
        protected override void DisplaySelection()
        {
            //CursorLeft is 16 because there is the text "Server name: " before it, which also starts on the CursorLeft position 2.
            Console.SetCursorPosition(16, 8);
            MenuOptions[0].SetText(serverName.PadRight(Console.WindowWidth - 19));
            MenuOptions[0].Display(MenuCursor.LeftIndex, MenuCursor.TopIndex);

            Console.SetCursorPosition(16, 10);
            MenuOptions[1].SetText(userName.PadRight(Console.WindowWidth - 19));
            MenuOptions[1].Display(MenuCursor.LeftIndex, MenuCursor.TopIndex);

            Console.SetCursorPosition(0, 14);
            MenuOptions[2].WriteInCenter(MenuCursor.LeftIndex, MenuCursor.TopIndex);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            cursorRefresh = false;
        }
    }
}
