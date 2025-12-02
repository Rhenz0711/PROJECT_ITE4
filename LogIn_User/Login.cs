using Loading_Login.MenuScreen;
using NAudio.Wave;
using System;
using System.Collections.Generic;

class Login
{
    private static bool ifErrorOnce = false;
    private static bool showCredentialActivated = false;
    private static int tries;

    public static bool ShowLoginPage()
    {
        bool authenticated = false;

        Console.Clear();

        // UI Login txt
        PrintLogin();

        horizontalCenter("Type 67 on Password to get Account Credentials.", ConsoleColor.Red);
        horizontalCenter(">>>Click ENTER to Submit<<<", ConsoleColor.Green);

        if (ifErrorOnce)
            Invalid();

        if (showCredentialActivated)
            ShowCredentials();

        //Block User
        if (tries == 3)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Program.PlayLoadingScreen(Resources.BlockedScreen(), Resources.BlockedAudio(), Resources.BlockedAccess(), width: 50, targetFps: 12, textColor: 0xFC0202, sleep: 0);
            Program.PlayLoadingScreen(Resources.BlockedScreen(), Resources.BlockedAudio(), Resources.BlockedAccess(), width: 50, targetFps: 12, textColor: 0xFC0202, sleep: 0);
            Thread.Sleep(2000);
            return false;
        }

        // Below: Get input
        Console.SetCursorPosition((Console.WindowWidth / 2) - 15, 21); // E-mail box line position
        string? user = Console.ReadLine();
        Program.EnterAudio();
        Console.SetCursorPosition((Console.WindowWidth / 2) - 15, Console.CursorTop + 4); // Password box line position
        string? password = Console.ReadLine();
        Program.EnterAudio();

        // Correct Input
        if (user?.ToLower() == "nimda" && password?.ToLower() == "321nimda")
        {
            Program.PlayLoadingScreen(Resources.LoadingScreen(), Resources.LoadingAuth(), Resources.LoginTxt(), width: 70, targetFps: 60, textColor: 0x07C801);
            Menu.DisplayMainMenu();
            return true;
        }

        // Invalid Inputs
        if (tries < 3)
        {
            tries++;
            ifErrorOnce = true;
            Console.ForegroundColor = ConsoleColor.Red;

            if (password == "67")
            {
                showCredentialActivated = true;
            }

            ShowLoginPage();
        }

        return authenticated;
    }

    // Centers any list of text lines vertically and horizontally in the console
    public static void PrintLogin()
    {
        Console.SetCursorPosition(0, 0);

        string[] loginArt = File.ReadAllLines(Resources.LoginUI());
        int lineStart = 5;

        foreach (var line in loginArt)
        {
            Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, lineStart);
            Console.WriteLine(line);
            lineStart++;
        }
    }

    //Center Text Horizontally
    static void horizontalCenter(string message, ConsoleColor textColor = ConsoleColor.White)
    {
        Console.WriteLine();
        Console.WriteLine();
        int width = (Console.WindowWidth / 2) - message.Length / 2;
        for (int i = 0; i < width; i++)
        {
            Console.Write(" ");
        }
        Console.ForegroundColor = textColor;
        Console.WriteLine(message);
        Console.ResetColor();

    }
    static void ShowCredentials()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 15);
        Console.WriteLine("User: admin\nPassword:admin123\nBUT BACKWARSS");
    }
    static void Invalid()
    {
        int validTries = 3 - tries;
        string message = "Invalid Username and Password. You have " + validTries  + " tries left";
        Console.SetCursorPosition((Console.WindowWidth - message.Length)/2, Console.CursorTop + 2); // Invalid box line position
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(message);
        //Console.SetCursorPosition(11, 11); // E-mail box line position
    }


}
