using OpenCvSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NAudio.Wave;
using Loading_Login.MenuScreen;
using OpenCvSharp.Internal.Vectors;

/// <summary>
/// Main entry point for ASCII video player application.
/// Handles initialization, ANSI color support, and audio/video coordination.
/// </summary>
/// 

class Program
{
    #region Constants

    private const int DEFAULT_ASCII_WIDTH = 120;
    private const int DEFAULT_TARGET_FPS = 15;
    private const int DEFAULT_TEXT_COLOR = 0xFFFFF; // White
    private const int LOADING_TEXT_COLOR = 0x07C801; // Green

    #endregion

    //Audio Controls
    private static WaveOutEvent? outputDevice;
    private static AudioFileReader? audioFileRead;

    
    #region Main Entry Point

    /// <summary>
    /// Application entry point. Initializes console and starts login flow.
    /// </summary>
    static void Main(string[] args)
    {

        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            //// Get ASCII width from user
            //int width = GetASCIIWidth();

            Console.Clear();

            // Enable ANSI color support in Windows Terminal
            EnableAsciiColor();

            PlayLoadingScreen(Resources.LoadingScreen(), Resources.LoadingAudio(), Resources.LoadingTxt(), DEFAULT_ASCII_WIDTH, DEFAULT_TARGET_FPS, LOADING_TEXT_COLOR);

            //Run LoginPage first, then display menu if authenticated
            if (Login.ShowLoginPage())
            {
                MenuAudio();
                Menu.DisplayMainMenu();
            }

        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {ex}");    
            Console.ResetColor();
        }
    }

    #endregion


    #region Public Methods

    public static void PlayLoadingScreen(string videoFile, string audioFile, string textFile, int width, int targetFps, int textColor = DEFAULT_TEXT_COLOR, int sleep = 1000)
    {
        Console.Clear();
        Thread.Sleep(sleep);

        StopPreviousAudio();
        PlayAudio(audioFile);
        var processor = new Processor(videoFile, textFile, width, targetFps, textColor);

        processor.PlayAsASCII();
    }
    #endregion

    #region Audio Control Methods
    public static void PlayAudio(string audioFile)
    {
        try
        {
            audioFileRead = new AudioFileReader(audioFile);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFileRead);
            outputDevice.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Video/Audio Playback failed: {ex.Message}");
        }
    }

    public static void StopPreviousAudio()
    {
        // Stop and dispose previous audio if any
        if (outputDevice != null)
        {
            outputDevice.Stop();
            outputDevice.Dispose();
            outputDevice = null;
        }
        if (audioFileRead != null)
        {
            //Console.WriteLine("Detectedddddd");
            audioFileRead.Dispose();
            audioFileRead = null;
        }
    }

    public static void EnterAudio()
    {
        AudioFileReader audioFileRead = new AudioFileReader(Resources.EnterSound());
        WaveOutEvent outputDevice = new WaveOutEvent();
        outputDevice.Init(audioFileRead);
        outputDevice.Play();
    }

    public static void MenuAudio()
    {
        StopPreviousAudio();

        audioFileRead = new AudioFileReader(Resources.MenuAudio());
        outputDevice = new WaveOutEvent();
        outputDevice.Init(audioFileRead);
        outputDevice.Play();
    }

    #endregion


    #region Win32 P/Invoke

    /// <summary>
    /// Windows API interop for console ANSI color support.
    /// Provides access to console mode functions on Windows.
    /// </summary>
    private static class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hHandle, uint dwMode);
    }

    /// <summary>
    /// Enables ANSI escape sequence support in Windows Terminal.
    /// Required for 24-bit color output on Windows.
    /// </summary>
    private static void EnableAsciiColor()
    {
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        var handle = Win32.GetStdHandle(STD_OUTPUT_HANDLE);

        if (Win32.GetConsoleMode(handle, out uint mode))
        {
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            Win32.SetConsoleMode(handle, mode);
        }
    }
    #endregion
}

#region File Resources
static class Resources
{
    private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;
    private const string SOURCE_FOLDER = "src";

    /// <summary>Builds full resource path from folder and filename.</summary>
    private static string GetResourcePath(string filename) => Path.Combine(BasePath, SOURCE_FOLDER, filename);

    // Video resources
    public static string LoadingScreen() => GetResourcePath("LoadingScreen.mp4");
    public static string LoadingScreen2() => GetResourcePath("LoadingScreen2.mp4");

    public static string BlockedScreen() => GetResourcePath("BlockedAccess.gif");

    // Audio resources
    public static string LoadingAudio() => GetResourcePath("LoadingScreen.mp3");
    public static string BlockedAudio() => GetResourcePath("BlockedAccess.mp3");
    public static string MenuAudio() => GetResourcePath("menu.mp3");
    public static string LoadingAuth() => GetResourcePath("loading.mp3");
    public static string Clap() => GetResourcePath("clap.mp3");


    // Audio - Basic
    public static string Basic_IntermediateMenuAudio() => GetResourcePath("basic.mp3");



    public static string EnterSound() => GetResourcePath("Enter.mp3");

    // Text/Animation resources
    public static string LoadingTxt() => GetResourcePath("LoadingScreen.txt");
    public static string LoginTxt() => GetResourcePath("Accessed.txt");
    public static string LoginUI() => GetResourcePath("LoginUI.txt");
    public static string BlockedAccess() => GetResourcePath("BlockedAccess.txt");
    public static string MenuTxt() => GetResourcePath("menu.txt");
    public static string CreditsTxt() => GetResourcePath("Credits.txt");



    //Sub-Menu Txt Resources
    public static string BasicMenu() => GetResourcePath("BasicMenu.txt");
    public static string IntermediateMenu() => GetResourcePath("IntermediateMenu.txt");
    public static string EntertainmentMenu() => GetResourcePath("EntertainmentMenu.txt");
    public static string Credits() => GetResourcePath("LoadingScreen.txt");

    //Image Resources
    public static string MenuBg() => GetResourcePath("menu.png");
    public static string LoginBg() => GetResourcePath("login.png");

    //Image-Sub2 MENUS
    public static string Basic() => GetResourcePath("basic.png");
    public static string Intermediate() => GetResourcePath("intermediate.png");


    //Intermediate
    public static string FoodMenu() => GetResourcePath("sales_trans.txt");

}
#endregion