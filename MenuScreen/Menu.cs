using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Loading_Login.MenuScreen
{

    class Menu
    {
        static int LeftwindowWidth = (Console.WindowWidth - 53) / 2;
        static int RightwindowWidth = (Console.WindowWidth + 53) / 2;
        static int indexcursor = 0;
        static int MenuPointerIndex = 0;

        private static int[][] cursor_Position =
        {
            new int[] { 10, 17, 24, 31 }, // Main Menu Pointer Position
            new int[] { 10, 17, 24, 31 }, // Basic Menu Pointer Position
            new int[] { 10, 17, 24, 31 },  // Intermediate Menu Pointer Position
            new int[] { 10, 17, 24 }  // Entertainent Menu Pointer Position
        };

        // Exit Cursor Pos
        private static int[] exitCursor_Pos = { Console.WindowHeight / 2, Console.WindowHeight / 2 + 3};
        private static readonly int exit_LeftwindowWidth = (Console.WindowWidth - 50) / 2;
        private static readonly int exit_RightwindowWidth = (Console.WindowWidth + 50) / 2;


        public static void DisplayMainMenu()
        {
            Console.Clear();
            Program.StopPreviousAudio();
            Thread.Sleep(2000);
            Program.PlayAudio(Resources.MenuAudio());

            ProcessMenu(Resources.MenuTxt(), cursorPos: cursor_Position[0], isMain: true);
            MenuPointerIndex = indexcursor;  // Store which menu was selected
            indexcursor = 0;  // Reset for the submenu
            Sub_Menu();
        }


        static void ProcessMenu(string menutxt, int[] cursorPos = null, bool isMain = true)
        {
            Console.Clear();
            //Display BgImage
            var processor = new Processor(Resources.MenuBg(), null, asciiWidth: Console.WindowWidth);
            processor.PlayAsASCII(verticalCenter: false);
            //Display Menu
            DisplayMenuOptions(menutxt);

            string message = "USE ARROW UP AND DOWN KEYS";
            Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop + 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);

            // Initial Pointer
            PrintPointer(cursorPos, indexcursor);
            int cursorIndex = MenuSelect(cursorPos);



            // Sub Menu
            if (!isMain)
            {
                Console.Clear();

                // Return To Main Menu
                if (cursorIndex == cursorPos.Length - 1)
                {
                    //Reset to defaults
                    LeftwindowWidth = (Console.WindowWidth - 50) / 2;
                    RightwindowWidth = (Console.WindowWidth + 50) / 2;
                    Program.EnterAudio();
                    indexcursor = 0;
                    DisplayMainMenu();
                }
                
                indexcursor = 0;

                // Sub Menu Select
                switch (MenuPointerIndex)
                {
                    //Basic Menu Select
                    case 0:
                        //Plays Loading Screen
                        Program.PlayLoadingScreen(Resources.LoadingScreen2(), Resources.LoadingAuth(), Resources.LoginTxt(), width: 70, targetFps: 30, textColor: 0x07C801);
                        Program.StopPreviousAudio();

                        //Display BgImage
                        processor = new Processor(Resources.Basic(), null, asciiWidth: Console.WindowWidth);
                        processor.PlayAsASCII();
                        Console.SetCursorPosition(0, 0);

                        switch (cursorIndex)
                        {
                            case 0:
                                Basic.Swapping();
                                break;
                            case 1:
                                Basic.MDAS();
                                break;
                            case 2:
                                Basic.Highest_Number();
                                break;
                        }
                        break;

                    // Intermediate Menu Select
                    case 1:
                        //Plays Loading Screen
                        Program.PlayLoadingScreen(Resources.LoadingScreen2(), Resources.LoadingAuth(), Resources.LoginTxt(), width: 70, targetFps: 30, textColor: 0x07C801);
                        Program.StopPreviousAudio();
                        Console.Clear();

                        switch (cursorIndex)
                        {
                            case 0:
                                Intermediate.Stud_Profile();
                                break;
                            case 1:
                                Intermediate.Grade_Comp();
                                break;
                            case 2:
                                Intermediate.Sales_Trans();
                                break;
                        }
                        break;

                    //Entertainment Menu Select
                    case 2:
                        //Plays Loading Screen
                        Program.PlayLoadingScreen(Resources.LoadingScreen2(), Resources.LoadingAuth(), Resources.LoginTxt(), width: 70, targetFps: 30, textColor: 0x07C801);
                        Program.StopPreviousAudio();
                        

                        switch (cursorIndex)
                        {
                            case 0:
                                Entertainment.Game.DriveGame();
                                break;
                            case 1:
                                Entertainment.Free_Style.TowerGame();
                                break;
                        }
                        break;
                }
            }
            Console.Clear();
        }

        static void Sub_Menu()
        {
            switch (MenuPointerIndex) 
            {
                case 0:
                    indexcursor = 0;
                    ProcessMenu(Resources.BasicMenu(), cursorPos: cursor_Position[1], isMain: false);
                    break;
                case 1:
                    LeftwindowWidth -= 7;
                    RightwindowWidth += 7;
                    indexcursor = 0;
                    ProcessMenu(Resources.IntermediateMenu(), cursorPos: cursor_Position[2], isMain: false);
                    break;
                case 2:
                    indexcursor = 0;
                    ProcessMenu(Resources.EntertainmentMenu(), cursorPos: cursor_Position[3], isMain: false);
                    break;
                case 3:
                    Credits._Credits();
                    return;
            }

        }

        public static void DisplayMenuOptions(string menu)
        {
            int topPos = 0;
            string[] menutitle = File.ReadAllLines(menu);

            ConsoleColor[] colors = { ConsoleColor.White, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.Cyan };

            int startLine = 1, colorIndex = 0, boxLines = 0;

            foreach (var line in menutitle)
            {
                if (startLine >= 9)
                {
                    if (boxLines % 7 == 0)
                    {
                        Console.ForegroundColor = colors[colorIndex];
                        colorIndex++;
                    }
                    boxLines++;
                }

                Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, topPos);
                Console.WriteLine(line);
                topPos++;
                startLine++;
            }
        }

        public static int MenuSelect(int[] cursorPos, bool twoPointer = true)
        {
            Console.CursorVisible = false;

            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        Program.EnterAudio();
                        RemovePreviousPointer(cursorPos, indexcursor);
                        indexcursor = (indexcursor == 0) ? cursorPos.Length - 1 : indexcursor - 1;
                        //Prints Pointer
                        PrintPointer(cursorPos, indexcursor, twoPointer);
                        break;
                    case ConsoleKey.DownArrow:
                        Program.EnterAudio();
                        RemovePreviousPointer(cursorPos, indexcursor);
                        indexcursor = (indexcursor == cursorPos.Length - 1) ? 0 : indexcursor + 1;
                        //Prints Pointer
                        PrintPointer(cursorPos, indexcursor, twoPointer);
                        break;
                }
            }
            while (key != ConsoleKey.Enter);
            Program.EnterAudio();
            return indexcursor;
        }
        #region Pointers
        public static void RemovePreviousPointer(int[] cursorPos, int indexcursor)
        {
            //Removes Previous Pointer
            for (int i = 0; i < 2; i++)
            {
                //Remove Horizontally
                Console.SetCursorPosition(LeftwindowWidth + i, cursorPos[indexcursor]);
                Console.Write(' ');
                Console.SetCursorPosition(RightwindowWidth + i, cursorPos[indexcursor]);
                Console.Write(' ');

                //Remove Vertically
                Console.SetCursorPosition(LeftwindowWidth + i, cursorPos[indexcursor] + 1);
                Console.Write(' ');
                Console.SetCursorPosition(RightwindowWidth + i, cursorPos[indexcursor] + 1);
                Console.Write(' ');
            }
        }
        public static void PrintPointer(int[] cursorPos, int indexcursor, bool twoPointer = true)
        {

            //Set Color for Pointer
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(LeftwindowWidth, cursorPos[indexcursor]);
            Console.Write("->");
            Console.SetCursorPosition(RightwindowWidth, cursorPos[indexcursor]);
            Console.Write("<-");

            if (twoPointer)
            {
                Console.SetCursorPosition(LeftwindowWidth, cursorPos[indexcursor]+1);
                Console.Write("->");
                Console.SetCursorPosition(RightwindowWidth, cursorPos[indexcursor] + 1);
                Console.Write("<-");
            }
        }
        #endregion

        public static void ExitOption(int leftIndent, int top)
        {
            //EXIT
            Console.SetCursorPosition(leftIndent, top + 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Click Enter To Proceed_");
            Console.ResetColor();
            Console.ReadKey();
            Program.EnterAudio();

            Program.StopPreviousAudio();


            //Reset variables
            indexcursor = 0;
            Console.Clear();

            string returnMessage = "Return to Main Menu?";
            string exitMessage = "Exit the Program";

            Console.SetCursorPosition((Console.WindowWidth - returnMessage.Length )/ 2, Console.WindowHeight / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(returnMessage);

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition((Console.WindowWidth - exitMessage.Length )/ 2, Console.CursorTop + 2);
            Console.Write("Exit the Program;");

            //Initial Pointer
            PrintPointer(exitCursor_Pos, 0, false);
            int index_exitCursorPos = MenuSelect(exitCursor_Pos, false);

            if (index_exitCursorPos == 0)
            {
                Menu.DisplayMainMenu();
            }
            else
            {
                Credits._Credits();
                return;
            }
        }
    }
}
