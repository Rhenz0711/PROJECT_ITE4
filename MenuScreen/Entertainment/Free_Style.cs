using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loading_Login.MenuScreen.Entertainment
{
    class Free_Style
    {
        static Exception exception = null;
        static int disks;
        static int minimumNumberOfMoves;
        static List<int>[] stacks;
        static int moves;
        static int? source;
        static State state;
        static int consoleWidth;
        static int consoleHeight;

        public static void TowerGame()
        {
            try
            {
                Program.MenuAudio();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

            Menu:
                Console.CursorVisible = false;
                Console.Clear();
                UpdateConsoleSize();
                DisplayMenu();

            GetEnter:
                Console.CursorVisible = false;
                var k1 = Console.ReadKey(true).Key;
                if (k1 == ConsoleKey.Enter) { }
                else if (k1 == ConsoleKey.Escape) return;
                else goto GetEnter;

                Console.Clear();
                UpdateConsoleSize();
                DisplayDiskSelection();

            GetDiskCount:
                Console.CursorVisible = false;
                var k2 = Console.ReadKey(true).Key;

                if (k2 == ConsoleKey.D3 || k2 == ConsoleKey.NumPad3) disks = 3;
                else if (k2 == ConsoleKey.D4 || k2 == ConsoleKey.NumPad4) disks = 4;
                else if (k2 == ConsoleKey.D5 || k2 == ConsoleKey.NumPad5) disks = 5;
                else if (k2 == ConsoleKey.D6 || k2 == ConsoleKey.NumPad6) disks = 6;
                else if (k2 == ConsoleKey.D7 || k2 == ConsoleKey.NumPad7) disks = 7;
                else if (k2 == ConsoleKey.D8 || k2 == ConsoleKey.NumPad8) disks = 8;
                else if (k2 == ConsoleKey.Escape) return;
                else goto GetDiskCount;

                Restart:
                state = State.ChooseSource;
                minimumNumberOfMoves = (int)Math.Pow(2, disks) - 1;

                stacks = new List<int>[3];
                stacks[0] = new List<int>();
                stacks[1] = new List<int>();
                stacks[2] = new List<int>();

                for (int i = disks; i > 0; i--)
                    stacks[0].Add(i);

                moves = 0;
                source = null;

                Console.Clear();
                UpdateConsoleSize();

                while (stacks[2].Count != disks)
                {
                    Render();

                    var k = Console.ReadKey(true).Key;

                    if (k == ConsoleKey.Escape) return;
                    else if (k == ConsoleKey.D1 || k == ConsoleKey.NumPad1) HandleStackButtonPress(0);
                    else if (k == ConsoleKey.D2 || k == ConsoleKey.NumPad2) HandleStackButtonPress(1);
                    else if (k == ConsoleKey.D3 || k == ConsoleKey.NumPad3) HandleStackButtonPress(2);
                    else if (k == ConsoleKey.End) goto Menu;
                    else if (k == ConsoleKey.R) goto Restart;
                }

                state = State.Win;
                Render();

            GetEnterOrEscape:
                Console.CursorVisible = false;
                var k3 = Console.ReadKey(true).Key;
                if (k3 == ConsoleKey.Enter) goto Menu;
                else if (k3 == ConsoleKey.Escape) return;
                else goto GetEnterOrEscape;
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                Console.CursorVisible = true;
                Console.ResetColor();
                Console.Clear();
                Console.SetCursorPosition((Console.WindowWidth/2) - 10, Console.WindowHeight/2);
                Console.WriteLine(exception != null ? exception.ToString() : "Tower Of Hanoi was closed.");

                Menu.ExitOption((Console.WindowWidth / 2) - 10, Console.CursorTop);
            }
        }

        static void UpdateConsoleSize()
        {
            consoleWidth = Console.WindowWidth;
            consoleHeight = Console.WindowHeight;
        }

        static void CenterWriteLine(string text)
        {
            int centerX = (consoleWidth - text.Length) / 2;
            centerX = Math.Max(0, centerX);
            Console.SetCursorPosition(centerX, Console.CursorTop);
            Console.WriteLine(text);
        }

        static void CenterWrite(string text)
        {
            int centerX = (consoleWidth - text.Length) / 2;
            centerX = Math.Max(0, centerX);
            Console.SetCursorPosition(centerX, Console.CursorTop);
            Console.Write(text);
        }

        static void DisplayMenu()
        {
            Console.WriteLine();
            CenterWriteLine("Tower Of Hanoi");
            Console.WriteLine();
            CenterWriteLine("This is a puzzle game where you need to");
            CenterWriteLine("move all the disks in the left stack to");
            CenterWriteLine("the right stack. You can only move one");
            CenterWriteLine("disk at a time from one stack to another");
            CenterWriteLine("stack, and you may never place a disk on");
            CenterWriteLine("top of a smaller disk on the same stack.");
            Console.WriteLine();
            CenterWriteLine("[enter] to continue");
            CenterWrite("[escape] exit game");
        }

        static void DisplayDiskSelection()
        {
            Console.WriteLine();
            CenterWriteLine("Tower Of Hanoi");
            Console.WriteLine();
            CenterWriteLine("The more disks, the harder the puzzle.");
            Console.WriteLine();
            CenterWriteLine("Select the number of disks:");
            CenterWriteLine("[3] 3 disks");
            CenterWriteLine("[4] 4 disks");
            CenterWriteLine("[5] 5 disks");
            CenterWriteLine("[6] 6 disks");
            CenterWriteLine("[7] 7 disks");
            CenterWriteLine("[8] 8 disks");
            CenterWrite("[escape] exit game");
        }

        static void HandleStackButtonPress(int stack)
        {
            if (!source.HasValue && stacks[stack].Count > 0)
            {
                source = stack;
                state = State.ChooseTarget;
            }
            else if (source.HasValue &&
                     (stacks[stack].Count == 0 ||
                     stacks[source.Value][stacks[source.Value].Count - 1] <
                     stacks[stack][stacks[stack].Count - 1]))
            {
                int disk = stacks[source.Value][stacks[source.Value].Count - 1];
                stacks[stack].Add(disk);
                stacks[source.Value].RemoveAt(stacks[source.Value].Count - 1);

                source = null;
                moves++;
                state = State.ChooseSource;
            }
            else if (source == stack)
            {
                source = null;
                state = State.ChooseSource;
            }
            else if (stacks[stack].Count != 0)
            {
                state = State.InvalidTarget;
            }
        }

        static void Render()
        {
            Console.CursorVisible = false;
            Console.Clear();
            UpdateConsoleSize();

            Console.WriteLine();
            CenterWriteLine("Tower Of Hanoi");
            Console.WriteLine();
            CenterWriteLine("Minimum Moves: " + minimumNumberOfMoves);
            Console.WriteLine();
            CenterWriteLine("Moves: " + moves);
            Console.WriteLine();

            for (int i = disks - 1; i >= 0; i--)
            {
                int centerX = (consoleWidth - GetLineWidth(i)) / 2;
                centerX = Math.Max(0, centerX);
                Console.SetCursorPosition(centerX, Console.CursorTop);

                for (int j = 0; j < stacks.Length; j++)
                {
                    Console.Write("  ");
                    RenderDisk(stacks[j].Count > i ? (int?)stacks[j][i] : null);
                }
                Console.WriteLine();
            }

            string towerBase = new string('─', disks) + "┴" + new string('─', disks);
            string baseDisplay = "  " + towerBase + "  " + towerBase + "  " + towerBase;
            CenterWriteLine(baseDisplay);

            int centerXBase = (consoleWidth - GetBelowBaseWidth()) / 2;
            centerXBase = Math.Max(0, centerXBase);
            Console.SetCursorPosition(centerXBase, Console.CursorTop);
            Console.Write("  ");
            Console.Write(RenderBelowBase(0));
            Console.Write("  ");
            Console.Write(RenderBelowBase(1));
            Console.Write("  ");
            Console.Write(RenderBelowBase(2));
            Console.WriteLine();

            Console.WriteLine();

            switch (state)
            {
                case State.ChooseSource:
                    CenterWriteLine("[1], [2], or [3] select source stack");
                    CenterWriteLine("[R] restart current puzzle");
                    CenterWriteLine("[end] back to menu");
                    CenterWrite("[escape] exit game");
                    break;

                case State.InvalidTarget:
                    CenterWriteLine("You may not place a disk on top of a");
                    CenterWriteLine("smaller disk on the same stack.");
                    Console.WriteLine();
                    goto case State.ChooseTarget;

                case State.ChooseTarget:
                    CenterWriteLine("[1], [2], or [3] select target stack");
                    CenterWriteLine("[home] restart current puzzle");
                    CenterWriteLine("[end] back to menu");
                    CenterWrite("[escape] exit game");
                    break;

                case State.Win:
                    CenterWriteLine("You solved the puzzle!");
                    CenterWriteLine("[enter] return to menu");
                    CenterWrite("[escape] exit game");
                    break;
            }
        }

        static int GetLineWidth(int row)
        {
            int width = 0;
            for (int j = 0; j < stacks.Length; j++)
            {
                width += 2; // "  "
                width += (disks * 2) + 1; // disk width + center pipe
            }
            return width;
        }

        static int GetBelowBaseWidth()
        {
            int width = 2; // "  "
            width += RenderBelowBase(0).Length;
            width += 2; // "  "
            width += RenderBelowBase(1).Length;
            width += 2; // "  "
            width += RenderBelowBase(2).Length;
            return width;
        }

        static void RenderDisk(int? disk)
        {
            if (!disk.HasValue)
            {
                Console.Write(new string(' ', disks) + "│" + new string(' ', disks));
                return;
            }

            int d = disk.Value;
            Console.Write(new string(' ', disks - d));

            ConsoleColor color = ConsoleColor.Red;
            switch (d)
            {
                case 1: color = ConsoleColor.Red; break;
                case 2: color = ConsoleColor.Green; break;
                case 3: color = ConsoleColor.Blue; break;
                case 4: color = ConsoleColor.Magenta; break;
                case 5: color = ConsoleColor.Cyan; break;
                case 6: color = ConsoleColor.DarkYellow; break;
                case 7: color = ConsoleColor.White; break;
                case 8: color = ConsoleColor.DarkGray; break;
            }

            Console.BackgroundColor = color;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(new string(' ', d));
            Console.Write("│");
            Console.Write(new string(' ', d));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(new string(' ', disks - d));
        }

        static string RenderBelowBase(int stack)
        {
            if (source.HasValue && source.Value == stack)
                return new string('^', disks - 1) + "[" + (stack + 1) + "]" + new string('^', disks - 1);

            return new string(' ', disks - 1) + "[" + (stack + 1) + "]" + new string(' ', disks - 1);
        }

        enum State
        {
            ChooseSource,
            ChooseTarget,
            InvalidTarget,
            Win
        }
    }
}