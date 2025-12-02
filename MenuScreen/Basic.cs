using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loading_Login.MenuScreen
{
class Basic
    {
        static readonly int left = 7, top = 3;

        #region Swapping
        public static void Swapping()
        {
            Program.PlayAudio(Resources.Basic_IntermediateMenuAudio());
            PrintBG();
            double N1, N2, N3;

            //Input
            Console.SetCursorPosition(left, top);
            Console.Write("Input First Number: ");
            Program.EnterAudio();

            N1 = HandleInputError("Input First Number: ");

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Input Second Number: ");
            Program.EnterAudio();

            N2 = HandleInputError("Input Second Number: ");

            //Computation
            N3 = N1;
            N1 = N2;
            N2 = N3;

            //Output
            Console.WriteLine();
            Console.WriteLine();

            ResultColored("First Number: ", N1);
            ResultColored("Second Number: ", N2);

            Menu.ExitOption(left, Console.CursorTop);
        }

        static void ResultColored(string message, double result)
        {
            Console.SetCursorPosition(left, Console.CursorTop+1);
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(result);
            Console.ResetColor();
        }
        #endregion

        #region MDAS
        public static void MDAS()
        {
            Program.PlayAudio(Resources.Basic_IntermediateMenuAudio());
            double N1 = 0, N2 = 0, Result = 0, num;
            string Operation;
            

            // Input
            PrintBG();
            Console.SetCursorPosition(left, top);
            Console.Write("Input First Number: ");
            Program.EnterAudio();

            N1 = HandleInputError("Input First Number: ");

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Enter Operation (+, -, *, /): ");
            Operation = Console.ReadLine();
            Program.EnterAudio();


            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Input Second Number: ");
            Program.EnterAudio();

            N2 = HandleInputError("Input Second Number: ");

            // Operation
            switch (Operation)
            {
                case "+":
                    Result = N1 + N2;
                    break;

                case "-":
                    Result = N1 - N2;
                    break;

                case "*":
                    Result = N1 * N2;
                    break;

                case "/":
                    if (N2 == 0)
                    {
                        Console.Clear();
                        Console.SetCursorPosition(left, top);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Cannot Divide by Zero");

                        Console.SetCursorPosition(left, Console.CursorTop + 2);
                        Console.Write("Click Enter To Restart_");
                        Console.ReadKey();
                        Console.ResetColor();
                        Program.EnterAudio();
                        Program.StopPreviousAudio();

                        MDAS();

                    }
                    Result = N1 / N2;
                    break;

                default:
                    Console.Clear();
                    Console.SetCursorPosition(left, top);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Invalid Operation");

                    Console.SetCursorPosition(left, Console.CursorTop + 2);
                    Console.Write("Click Enter To Restart_");
                    Console.ReadKey();
                    Console.ResetColor(); 
                    Program.EnterAudio();
                    Program.StopPreviousAudio();

                    MDAS();
                    break;
            }

            // Output
            Console.SetCursorPosition(left, Console.CursorTop + 2);
            Console.Write("Result: ");

            Console.ForegroundColor = ConsoleColor.Green;   
            Console.Write("{0:n2}", Result);
            Console.ResetColor();                          

            Menu.ExitOption(left, Console.CursorTop);
        }

        #endregion

        #region HighestNumber
        public static void Highest_Number()
        {
            Program.PlayAudio(Resources.Basic_IntermediateMenuAudio());
            double N1, N2, N3;

            PrintBG();

            // Input
            Console.SetCursorPosition(left, top);
            Console.Write("Input First Number: ");
            Program.EnterAudio();

            N1 = HandleInputError("Input First Number: ");

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Input Second Number: ");
            Program.EnterAudio();

            N2 = HandleInputError("Input Second Number: ");


            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Input Third Number: ");
            Program.EnterAudio();

            N3 = HandleInputError("Input Third Number: "); ;

            double highest;

            if (N1 >= N2 && N1 >= N3)
                highest = N1;
            else if (N2 >= N1 && N2 >= N3)
                highest = N2;
            else
                highest = N3;

            Console.SetCursorPosition(left, Console.CursorTop + 3);
            Console.Write("The Highest Number is: ");

            // color only the result
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(highest);
            Console.ResetColor();

            Menu.ExitOption(left, Console.CursorTop);
        }
        #endregion

        static double HandleInputError(string prompt)
        {
            double num;
            while (!double.TryParse(Console.ReadLine(), out num))
            {
                Console.Clear();
                PrintBG();
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid!");
                Console.ResetColor();
                Console.SetCursorPosition(left, Console.CursorTop + 1);
                Console.Write(prompt);
            }
            return num;
        }

        #region Print BGIMAGE
        static void PrintBG()
        {
            //Display BgImage
            var processor = new Processor(Resources.Basic(), null, asciiWidth: Console.WindowWidth);
            processor.PlayAsASCII(verticalCenter: false);
            Console.SetCursorPosition(0, 0);
        }
        #endregion
    }
}
