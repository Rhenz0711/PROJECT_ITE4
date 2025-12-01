using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loading_Login.MenuScreen
{
    class Intermediate
    {
        // Var of Sales_Trans
        private static int[] pointer_pos = { 15, 17, 19, 21, 32, 34, 36, 38, 49, 51, 53 };

        static readonly int left = 7, top = 3;

        #region Student Profile
        public static void Stud_Profile()
        {
            Program.PlayAudio(Resources.Basic_IntermediateMenuAudio());
            PrintBG();

            Console.SetCursorPosition(left, top);
            Console.Write("Input number of Students: ");

            int num = Convert.ToInt32(HandleInputError("Input number of Students: "));

            string[] snumber = new string[num];
            string[] mname = new string[num];
            string[] fname = new string[num];
            string[] lname = new string[num];
            string[] bday = new string[num];
            string[] course = new string[num];
            string[] section = new string[num];
            string[] municipality = new string[num];

            // Input loop
            for (int i = 0; i < num; i++)
            {
                Console.Clear();
                PrintBG();
                Console.SetCursorPosition(left, top);
                Console.WriteLine($"Student #{i + 1}");

                Console.SetCursorPosition(left, Console.CursorTop + 1);
                Console.Write("Student Number (e.g. B2025-0504): ");
                snumber[i] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("First Name  (e.g. Juan): ");
                fname[i] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Middle Name (e.g. Dela): ");
                mname[i] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Last Name  (e.g. Cruz): ");
                lname[i] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Birthday (MM/DD/YYYY): ");
                bday[i] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Course (e.g. BSIT): ");
                course[i] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Section (e.g. BSIT1-2B): ");
                section[i] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Municipality (e.g. Taytay): ");
                municipality[i] = Console.ReadLine();
            }

            // Output table
            Console.Clear();
            PrintBG();
            Console.SetCursorPosition(left, top);

            string line = new string('*', 150);

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(line);

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(
                "{0,-20} | {1,-40} | {2,-15} | {3,-30} | {4,-10} | {5,-30}",
                "Student Number", "Full Name", "Birthday", "Course", "Section", "Municipality");

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(line);

            for (int i = 0; i < num; i++)
            {
                string currentMname = mname[i] ?? "";
                string finalMName;

                if (currentMname.Trim().Length == 0)
                {
                    finalMName = "";
                }
                else if (currentMname.Contains(" "))
                {
                    int pos = currentMname.IndexOf(" ");
                    char firstInitial = currentMname[0];
                    char secondInitial = currentMname[pos + 1];
                    finalMName = firstInitial.ToString().ToUpper() + "." +
                                 secondInitial.ToString().ToUpper() + ".";
                }
                else
                {
                    char firstInitial = currentMname[0];
                    finalMName = firstInitial.ToString().ToUpper() + ".";
                }

                string fullname = string.Format("{0} {1} {2}", fname[i], finalMName, lname[i]);

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.WriteLine(
                    "{0,-20} | {1,-40} | {2,-15} | {3,-30} | {4,-10} | {5,-30}",
                    snumber[i], fullname, bday[i], course[i], section[i], municipality[i]);
            }

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(line);

            Menu.ExitOption(left, Console.CursorTop);
        }

        #endregion

        #region Grade Computation
        public static void Grade_Comp()
        {
            Program.PlayAudio(Resources.Basic_IntermediateMenuAudio());
            PrintBG();

            int num;
            string? fn, mn, ln;

            Console.SetCursorPosition(left, top);
            Console.Write("Input First Name: ");
            fn = Console.ReadLine();

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Input Middle Name: ");
            mn = Console.ReadLine();

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Input Last Name: ");
            ln = Console.ReadLine();

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write("Input Number of Subjects: ");

            num = Convert.ToInt32(HandleInputError("Input number of Subjects: "));

            string[] subject = new string[num];
            double[] p = new double[num];
            double[] m = new double[num];
            double[] f = new double[num];
            double[] ave = new double[num];
            double GenAve, total = 0;

            // SUBJECT INPUT LOOP
            for (int a = 0; a < num; a++)
            {
                Console.Clear();
                PrintBG();
                Console.SetCursorPosition(left, top);
                Console.WriteLine($"Subject #{a + 1}");

                Console.SetCursorPosition(left, Console.CursorTop + 1);
                Console.Write("Input Subject Name : ");
                subject[a] = Console.ReadLine();

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Input Prelim Grade : ");
                p[a] = HandleInputError("Input Prelim Grade: ");

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Input Midterm Grade: ");
                m[a] = HandleInputError("Input Midterm Grade: ");

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.Write("Input Finals Grade : ");
                f[a] = HandleInputError("Input Finals Grade: ");

                ave[a] = (p[a] + m[a] + f[a]) / 3;
                total += ave[a];
            }

            GenAve = total / num;

        //Process
            Console.Clear();
            PrintBG();

            int tableWidth = 80; // width of the banner
            int leftPad = 10;
            int topPad = 3;

            string line = new string('*', tableWidth);

            // ***** white line *****
            Console.ResetColor();
            Console.SetCursorPosition(leftPad, topPad);
            Console.WriteLine(line);

            // ***** red student name *****
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Green;
            if (mn.Contains(" "))
                Console.WriteLine($"Student Name: {fn} {mn[0]}{mn[mn.IndexOf(" ") + 1]}. {ln}\n");
            else
                Console.WriteLine($"Student Name: {fn} {mn[0]}. {ln}\n");
            Console.ResetColor();

            // ***** white line *****
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.WriteLine(line);

            // ***** red header *****
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0,-20}{1,-10}{2,-10}{3,-10}{4,-10}",
                "Subject", "Prelim", "Midterm", "Finals", "Average");
            Console.ResetColor();

            // ***** white line *****
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.WriteLine(line);

            // ***** red rows *****
            for (int a = 0; a < num; a++)
            {
                Console.SetCursorPosition(leftPad, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0,-20}{1,-10}{2,-10}{3,-10}{4,-10:n}",
                    subject[a], p[a], m[a], f[a], ave[a]);
                Console.ResetColor();
            }

            // ***** white line *****
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.WriteLine(line);

            // ***** red general average *****
            Console.SetCursorPosition(leftPad, Console.CursorTop + 1);
            Console.Write("Your General Average is: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{GenAve:n}");
            Console.ResetColor();


            Menu.ExitOption(left, Console.CursorTop);
        }
        #endregion

        #region Sales Transaction

        public static void Sales_Trans()
        {
            Program.PlayAudio(Resources.Basic_IntermediateMenuAudio());

            int customerNo = 001;
            bool StoreOpen = true;


            while (StoreOpen)
            {

                List<string> orderNames = new List<string>();
                List<int> orderPrices = new List<int>();
                List<int> orderQtys = new List<int>();

                string? finishOrder;

                do
                {
                    Console.Clear();

                    PrintBG();
                    PrintFoodMenu();

                    Console.SetCursorPosition(10, 30);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Customer No: " + customerNo);
                    Console.ResetColor();

                    Console.SetCursorPosition(10, Console.CursorTop + 2);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("USE ARROW UP AND DOWN TO CHOOSE YOUR ITEMS");

                    Menu.PrintPointer(pointer_pos, 0);
                    int food_select = Menu.MenuSelect(pointer_pos);


                    string itemName = "";
                    int itemPrice = 0, quant;

                    switch (food_select)
                    {
                        case 0:
                            itemName = "Footlong";
                            itemPrice = 65;
                            break;
                        case 1:
                            itemName = "Cheeseburger";
                            itemPrice = 50;
                            break;
                        case 2:
                            itemName = "Chickenburger";
                            itemPrice = 85;
                            break;
                        case 3:
                            itemName = "Siomai";
                            itemPrice = 25;
                            break;
                        case 4:
                            itemName = "Siomai w/ Rice";
                            itemPrice = 40;
                            break;
                        case 5:
                            itemName = "Tapa w/ Rice";
                            itemPrice = 100;
                            break;
                        case 6:
                            itemName = "Sisig w/ Rice";
                            itemPrice = 90;
                            break;
                        case 7:
                            itemName = "Burger w/ Rice";
                            itemPrice = 79;
                            break;
                        case 8:
                            itemName = "Iced Tea";
                            itemPrice = 85;
                            break;
                        case 9:
                            itemName = "Coke";
                            itemPrice = 70;
                            break;
                        case 10:
                            itemName = "Rootbeer";
                            itemPrice = 80;
                            break;
                    }

                    Console.SetCursorPosition(10, 35);
                    Console.ResetColor();
                    Console.Write("Quantity for ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(itemName + ": ");
                    Console.ResetColor();
                    Program.EnterAudio();


                    while (!int.TryParse(Console.ReadLine(), out quant))
                    {

                        Console.SetCursorPosition(10, Console.CursorTop);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Invalid!");
                        Console.ResetColor();
                        Console.SetCursorPosition(10, Console.CursorTop + 1);
                        Console.Write("Quantity for ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(itemName + ": ");
                        Console.ResetColor();
                        Program.EnterAudio();
                    }


                    orderNames.Add(itemName);
                    orderPrices.Add(itemPrice);
                    orderQtys.Add(quant);

                    Console.SetCursorPosition(10, Console.CursorTop);
                    Console.Write("Would you like another purchase? (Y/N): ");
                    Program.EnterAudio();
                    finishOrder = Console.ReadLine();

                } while (finishOrder.ToUpper() == "Y");

                Payment(PrintReceipt(orderNames, orderPrices, orderQtys, customerNo));

                Console.Write("\nPress ENTER for the Next Customer or 'Q' to Quit: ");
                string next = Console.ReadLine();


                if (next.ToUpper() == "Q")
                    StoreOpen = false;
                else
                    customerNo++;
            }

            Menu.ExitOption(left, Console.CursorTop);
        }

        static void PrintFoodMenu()
        {
            int topPos = 0;
            string[] menutitle = File.ReadAllLines(Resources.FoodMenu());

            ConsoleColor[] colors = { ConsoleColor.DarkRed, ConsoleColor.DarkGreen, ConsoleColor.Cyan };
            // menuLines = start of line in the console
            // boxLines = start of line after the menuLines > topGap
            int menuLines = 1, boxLines = 0;

            foreach (var line in menutitle)
            {
                if (menuLines >= 7)
                {
                    if (boxLines >= 0 && boxLines <= 7)
                    {
                        Console.ForegroundColor = colors[0];
                    }
                    else if (boxLines >= 17 && boxLines <= 24)
                    {
                        Console.ForegroundColor = colors[1];
                    }
                    else if (boxLines >= 34 && boxLines <= 41)
                    {
                        Console.ForegroundColor = colors[2];
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    boxLines++;
                }

                Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, topPos);
                Console.WriteLine(line);
                topPos++;
                menuLines++;
            }
        }

        static int PrintReceipt(List<string> orderNames, List<int> orderPrices, List<int> orderQtys, int customerNo)
        {
            Console.Clear();
            PrintBG();

            Console.SetCursorPosition(7, 3);
            Console.WriteLine("**************************************************");
            Console.SetCursorPosition(7, Console.CursorTop);
            Console.WriteLine("RECEIPT - Customer No. " + customerNo.ToString("D2"));
            Console.SetCursorPosition(7, Console.CursorTop);
            Console.WriteLine("**************************************************");
            Console.SetCursorPosition(7, Console.CursorTop);
            Console.WriteLine("{0,-20}|{1,-5}|{2,-10}|{3,-10}", "Item Name", "Qty", "Price", "Total");
            Console.SetCursorPosition(7, Console.CursorTop);
            Console.WriteLine("--------------------------------------------------");

            int totalprice = 0;

            for (int i = 0; i < orderNames.Count; i++)
            {
                int overallorder = orderPrices[i] * orderQtys[i];
                totalprice = totalprice + overallorder;

                Console.SetCursorPosition(7, Console.CursorTop);
                Console.WriteLine("{0,-20}|{1,-5}|{2,-10}|{3,-10}", orderNames[i], orderQtys[i], orderPrices[i], overallorder);
            }

            Console.SetCursorPosition(7, Console.CursorTop);
            Console.WriteLine("--------------------------------------------------");
            Console.SetCursorPosition(7, Console.CursorTop);
            Console.WriteLine("TOTAL PRICE: " + totalprice);
            Console.SetCursorPosition(7, Console.CursorTop);
            Console.WriteLine("**************************************************");


            return totalprice;
        }

        static void Payment(int totalprice)
        {
            Console.Write("\n\n\nInput Cash: ");
            double cashpaid = HandleInputError("Input Cash: ", false);

            while (cashpaid < totalprice)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Insufficient Cash!");
                Console.ResetColor();
                Console.WriteLine("Needed: " + totalprice);
                Console.WriteLine("You gave: " + cashpaid);
                Console.Write("\nPress R to go back to Food Menu: ");

                string choice = Console.ReadLine().ToUpper();
                Program.EnterAudio();

                if (choice != "R")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Order Cancelled..");
                    Console.ResetColor();

                    Menu.ExitOption(left, Console.CursorTop);
                }

                Sales_Trans();
            }
            double cashchange = cashpaid - totalprice;

            Console.Write("Change: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(cashchange);
            Console.ResetColor();

            Menu.ExitOption(left, Console.CursorTop);
        }
        #endregion

        #region Print BGIMAGE
        static void PrintBG()
        {
            //Display BgImage
            //Console.SetCursorPosition(0, 0);
            var processor = new Processor(Resources.Intermediate(), null, asciiWidth: Console.WindowWidth);
            processor.PlayAsASCII(verticalCenter: false);
            Console.SetCursorPosition(0, 0);
        }
        #endregion

        #region InputErrorHandler
        static double HandleInputError(string prompt, bool reset = true)
        {
            double num;
            while (!double.TryParse(Console.ReadLine(), out num))
            {
                if (reset)
                {
                    Console.Clear();
                    PrintBG();
                }

                Console.SetCursorPosition(left, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid!");
                Console.ResetColor();
                Console.SetCursorPosition(left, Console.CursorTop + 1);
                Console.Write(prompt);
            }
            return num;
        }
        #endregion
    }
}


