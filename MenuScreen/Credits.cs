using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loading_Login.MenuScreen
{
class Credits
    {
        public static void _Credits()
        {
            Thread.Sleep(1000);
            Console.Clear();
            Program.StopPreviousAudio();

            Program.PlayAudio(Resources.LoadingAudio());
            Console.CursorVisible = false;
            int topPos = (Console.WindowHeight / 2) - 5;
            string[] menutitle = File.ReadAllLines(Resources.CreditsTxt());

            ConsoleColor[] colors = { ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.DarkGreen, ConsoleColor.DarkRed, ConsoleColor.White };
            int colorIndex = 0, boxLines = 1;

            foreach (var line in menutitle)
            {
                if (boxLines == 40)
                {
                    SpecialAudio();
                    Thread.Sleep(1000);
                }

                if (boxLines % 10 == 0)
                {
                    colorIndex++;
                    Console.ForegroundColor = colors[colorIndex];

                    Thread.Sleep(3000);
                    Console.Clear();
                    topPos = (Console.WindowHeight / 2) - 5;
                }

                Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, topPos);
                Console.WriteLine(line);
                topPos++;
                boxLines++;
            }
            Thread.Sleep(3000);

            Console.Clear();
            string message = "Thank You.";
            Console.SetCursorPosition((Console.WindowWidth / 2) - message.Length, Console.WindowHeight / 2);
            Console.WriteLine(message);

            Thread.Sleep(7000);
            Environment.Exit(0);

        }

        static void SpecialAudio()
        {
            AudioFileReader audioFileRead = new AudioFileReader(Resources.Clap());
            WaveOutEvent outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFileRead);
            outputDevice.Play();
        }

    }
}
