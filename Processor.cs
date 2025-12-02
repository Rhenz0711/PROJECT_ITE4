using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Manages video playback and ASCII rendering with synchronized animation overlay.
/// Optimized for frame timing accuracy and minimal memory allocation per frame.
/// </summary>
class Processor
{
    #region Private Fields

    private readonly string _videoPath;
    private readonly int _asciiWidth;
    private readonly int _targetFps;
    private readonly string _loadingFile;
    private readonly int _textColor;
    private readonly string[][] _loadingKeyframes;

    private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
    private static readonly string[] VideoExtensions = { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm", ".m4v", ".gif" };

    // Pre-cached display parameters to avoid recalculation every frame
    private int _maxLoadingWidth;
    private int _maxLoadingHeight;
    private double _frameDelayMs;

    // ANSI Control Codes (constants for performance)
    private const string CURSOR_HOME = "\u001b[H";      // Move cursor to home (0,0)
    private const string CLEAR_SCREEN = "\u001b[2J";    // Clear entire screen

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes processor with video and animation settings.
    /// Loads keyframes once to avoid disk I/O during playback.
    /// </summary>
    public Processor(string videoPath, string loadingFile = null, int asciiWidth = 100, int targetFps = 15, int textColor = 0xFFFFF)
    {
        _videoPath = videoPath;
        _asciiWidth = asciiWidth;
        _targetFps = targetFps;
        _loadingFile = loadingFile;
        _textColor = textColor;

        // Only load keyframes if a file was provided
        if (!string.IsNullOrEmpty(_loadingFile))
            _loadingKeyframes = LoadingKeyFrames(_loadingFile);
        else
            _loadingKeyframes = new string[0][];

        CacheAnimationDimensions();
        _frameDelayMs = 1000.0 / _targetFps;
    }


    #endregion

    #region Public Methods

    /// <summary>
    /// Main playback loop: reads frames and renders with synchronized animation.
    /// </summary>
    public void PlayAsASCII(bool verticalCenter = true)
    {
        string fileExtension = Path.GetExtension(_videoPath).ToLower();

        // Check if it's an image file
        if (ImageExtensions.Contains(fileExtension))
        {
            PlayImageAsASCII(verticalCenter);
        }
        // Check if it's a video file
        else if (VideoExtensions.Contains(fileExtension))
        {
            PlayVideoAsASCII(verticalCenter);
        }
        else
        {
            throw new Exception($"Unsupported file format: {fileExtension}");
        }
    }

    #endregion

    #region Private Methods
    private void PlayImageAsASCII(bool verticalCenter)
    {
        try
        {
            Mat image = Cv2.ImRead(_videoPath);

            if (image.Empty())
                throw new Exception("Cannot open image file");

            Console.Write(CLEAR_SCREEN + CURSOR_HOME);

            var (asciiArt, colorArray) = ASCIIConverter.ConvertFrameToASCII(image, _asciiWidth);
            DisplayVideoAndLoading(asciiArt, colorArray, 0, verticalCenter);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error playing image: {ex.Message}");
        }
    }

    private void PlayVideoAsASCII(bool verticalCenter)
    {
        using (var video = new VideoCapture(_videoPath))
        {
            if (!video.IsOpened())
                throw new Exception("Cannot open video file");

            Console.Write(CLEAR_SCREEN + CURSOR_HOME);

            Mat frame = new Mat();
            int frameNumber = 0;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                while (video.Read(frame))
                {
                    var (asciiArt, colorArray) = ASCIIConverter.ConvertFrameToASCII(frame, _asciiWidth);
                    Console.Write(CURSOR_HOME);
                    DisplayVideoAndLoading(asciiArt, colorArray, frameNumber, verticalCenter);

                    SynchronizeFrameTiming(stopwatch, frameNumber);
                    frameNumber++;
                }
            }
            finally
            {
                stopwatch.Stop();
                frame?.Dispose();
            }
        }
    }
    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Pre-calculates animation dimensions to avoid repeated LINQ queries per frame.
    /// </summary>
    private void CacheAnimationDimensions()
    {
        if (_loadingKeyframes.Length == 0)
        {
            _maxLoadingWidth = 0;
            _maxLoadingHeight = 0;
            return;
        }

        _maxLoadingWidth = _loadingKeyframes
            .SelectMany(frame => frame)
            .DefaultIfEmpty("")
            .Max(line => line.Length);

        _maxLoadingHeight = _loadingKeyframes
            .Select(frame => frame.Length)
            .DefaultIfEmpty(0)
            .Max();
    }

    /// <summary>
    /// Synchronizes actual frame timing with target FPS.
    /// Avoids repeated Stopwatch calls by calculating expected vs actual time.
    /// </summary>
    private void SynchronizeFrameTiming(Stopwatch stopwatch, int frameNumber)
    {
        long elapsedMs = stopwatch.ElapsedMilliseconds;
        long targetMs = (long)((frameNumber + 1) * _frameDelayMs);
        long sleepTime = targetMs - elapsedMs;

        if (sleepTime > 0)
            Thread.Sleep((int)sleepTime);
    }

    /// <summary>
    /// Renders video frame and animation overlay with color support.
    /// Builds entire output in StringBuilder before writing to avoid multiple console writes.
    /// </summary>
    private void DisplayVideoAndLoading(string ascii, int[,] colorArray, int frameNumber, bool verticalCenter)
    {
        int consoleWidth = Console.WindowWidth;
        int asciiHeight = colorArray.GetLength(0);
        int asciiWidth = colorArray.GetLength(1);

        StringBuilder output = new StringBuilder();
        RenderCenteredVideo(output, ascii, colorArray, consoleWidth, asciiHeight, asciiWidth, verticalCenter);

        if (_loadingKeyframes.Length > 0)
        {
            // Spacing between video and animation
            output.AppendLine();
            output.AppendLine();
            // Draw animation overlay
            int keyframeIndex = (frameNumber / 30) % _loadingKeyframes.Length;
            string[] currentLoadingArt = _loadingKeyframes[keyframeIndex];
            RenderCenteredAnimation(output, currentLoadingArt, consoleWidth);
        }

        Console.Write(CURSOR_HOME + output.ToString());
    }


    /// <summary>
    /// Renders the video portion with colors, centered horizontally and vertically.
    /// </summary>
    private void RenderCenteredVideo(StringBuilder output, string ascii, int[,] colorArray,
                                      int consoleWidth, int asciiHeight, int asciiWidth, bool verticalCenter)
    {
        if(verticalCenter)
        {
            // Vertical padding
            int asciiTopPad = Math.Max((Console.WindowHeight - asciiHeight) / 2, 0);
            for (int i = 0; i < asciiTopPad; i++)
                output.AppendLine();
        }

        // Horizontal padding
        int videoPad = Math.Max((consoleWidth - asciiWidth) / 2, 0);
        var lines = ascii.Split(Environment.NewLine);

        // Render each row with colors
        for (int row = 0; row < asciiHeight && row < lines.Length; row++)
        {
            output.Append(new string(' ', videoPad));

            string line = lines[row];
            for (int col = 0; col < asciiWidth; col++)
            {
                int color = colorArray[row, col];
                char c = col < line.Length ? line[col] : ' ';

                // Skip rendering black pixels (0,0,0 RGB)
                int r = (color >> 16) & 0xFF;
                if (r == 0 && (color & 0xFF00FF) == 0)
                {
                    output.Append(' ');
                }
                else
                {
                    string ansiCode = ASCIIConverter.GetANSIColorCode(color);
                    output.Append(ansiCode + c + ASCIIConverter.ResetANSI());
                }
            }
            output.AppendLine();
        }
    }

    /// <summary>
    /// Renders the animation overlay, perfectly centered with color.
    /// </summary>
    private void RenderCenteredAnimation(StringBuilder output, string[] loadingArt, int consoleWidth)
    {
        int blockPad = Math.Max((consoleWidth - _maxLoadingWidth) / 2, 0);
        string ansiCode = ASCIIConverter.GetANSIColorCode(_textColor);

        for (int i = 0; i < _maxLoadingHeight; i++)
        {
            string line = i < loadingArt.Length ? loadingArt[i] : "";

            // Center line within maxLoadingWidth
            int linePad = Math.Max((_maxLoadingWidth - line.Length) / 2, 0);
            int trailingPad = _maxLoadingWidth - linePad - line.Length;

            string centeredLine = new string(' ', linePad) + line + new string(' ', trailingPad);
            string finalLine = new string(' ', blockPad) + centeredLine;

            output.Append(ansiCode + finalLine + ASCIIConverter.ResetANSI());
            output.AppendLine();
        }
    }

    /// <summary>
    /// Loads animation keyframes from text file.
    /// Keyframes are separated by "====" delimiter.
    /// </summary>
    private static string[][] LoadingKeyFrames(string filename)
    {
        var frames = new List<List<string>>();
        var currentFrame = new List<string>();

        foreach (var line in File.ReadLines(filename))
        {
            if (line.Trim() == "====")
            {
                if (currentFrame.Count > 0)
                {
                    frames.Add(new List<string>(currentFrame));
                    currentFrame.Clear();
                }
            }
            else
            {
                currentFrame.Add(line);
            }
        }

        // Add last frame if exists
        if (currentFrame.Count > 0)
            frames.Add(currentFrame);

        return frames.Select(f => f.ToArray()).ToArray();
    }

    #endregion

}