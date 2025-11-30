using OpenCvSharp;
using System;
using System.Text;

/// <summary>
/// Converts video frames to ASCII art with color encoding.
/// Optimized for performance with cached constants and efficient pixel processing.
/// </summary>
class ASCIIConverter
{
    // ASCII characters from dark to bright (cached for performance)
    private const string ASCII_CHARS = " .'`^\",:;Il!i><~+_-?][}{1)(|\\/*tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";
    private static readonly int ASCII_CHARS_LENGTH = ASCII_CHARS.Length - 1;

    // ANSI color codes (cached constants)
    private const string RESET_ANSI = "\u001b[0m";
    private const string ANSI_PREFIX = "\u001b[38;2;";
    private const string ANSI_SUFFIX = "m";

    /// <summary>
    /// Converts a video frame to ASCII art with color mapping.
    /// </summary>
    /// <param name="frame">Input video frame (Mat)</param>
    /// <param name="targetWidth">Desired ASCII width</param>
    /// <returns>Tuple of (ASCII string, color array)</returns>
    public static (string ascii, int[,] colors) ConvertFrameToASCII(Mat frame, int targetWidth)
    {
        if (frame.Empty())
            throw new ArgumentException("Frame is empty");

        // Calculate dimensions maintaining aspect ratio
        double aspectRatio = (double)frame.Height / frame.Width;
        int targetHeight = (int)(targetWidth * aspectRatio * 0.5); // 0.5 accounts for character height

        // Process image: Resize → Grayscale → ASCII mapping
        string ascii = ProcessFrame(frame, targetWidth, targetHeight, out int[,] colorArray);

        return (ascii, colorArray);
    }

    /// <summary>
    /// Core frame processing: resizes, converts to grayscale, and generates ASCII.
    /// </summary>
    private static string ProcessFrame(Mat frame, int targetWidth, int targetHeight, out int[,] colorArray)
    {
        Mat resized = new Mat();
        Mat gray = new Mat();

        try
        {
            // Resize and convert to grayscale
            Cv2.Resize(frame, resized, new Size(targetWidth, targetHeight));
            Cv2.CvtColor(resized, gray, ColorConversionCodes.BGR2GRAY);

            colorArray = new int[targetHeight, targetWidth];
            StringBuilder asciiBuilder = new StringBuilder(targetHeight * (targetWidth + 1));

            // Process each row
            for (int y = 0; y < gray.Rows; y++)
            {
                for (int x = 0; x < gray.Cols; x++)
                {
                    byte grayValue = gray.At<byte>(y, x);
                    int charIndex = (grayValue * ASCII_CHARS_LENGTH) / 255;
                    asciiBuilder.Append(ASCII_CHARS[charIndex]);

                    // Extract and store RGB color
                    Vec3b bgr = resized.At<Vec3b>(y, x);
                    int r = bgr.Item2;
                    int g = bgr.Item1;
                    int b = bgr.Item0;
                    colorArray[y, x] = (r << 16) | (g << 8) | b;
                }
                asciiBuilder.AppendLine();
            }

            return asciiBuilder.ToString();
        }
        finally
        {
            resized?.Dispose();
            gray?.Dispose();
        }
    }

    /// <summary>
    /// Generates ANSI escape code for 24-bit RGB color.
    /// </summary>
    /// <param name="rgb">RGB color as 0xRRGGBB integer</param>
    /// <returns>ANSI escape sequence string</returns>
    public static string GetANSIColorCode(int rgb)
    {
        int r = (rgb >> 16) & 0xFF;
        int g = (rgb >> 8) & 0xFF;
        int b = rgb & 0xFF;

        return $"{ANSI_PREFIX}{r};{g};{b}{ANSI_SUFFIX}";
    }

    /// <summary>
    /// Returns ANSI reset code to restore default terminal colors.
    /// </summary>
    public static string ResetANSI() => RESET_ANSI;
}
