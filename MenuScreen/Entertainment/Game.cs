using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loading_Login.MenuScreen.Entertainment
{
    /// <summary>
    /// A console-based driving game where the player navigates a car through a shifting road.
    /// </summary>
    class Game
    {
        #region Constants
        private const int GAME_WIDTH = 48;
        private const int GAME_HEIGHT = 30;
        private const int ROAD_WIDTH = 10;
        private const int RENDER_DELAY_MS = 33;
        private const int ROAD_SHIFT_PROBABILITY = 5;
        private const int ROAD_SHIFT_THRESHOLD = 4;
        private const char GRASS_CHAR = '.';
        private const char ROAD_CHAR = ' ';
        private const char CAR_STOPPED = 'X';
        private const char CAR_LEFT = '<';
        private const char CAR_RIGHT = '>';
        private const char CAR_STRAIGHT = '^';
        #endregion

        #region Fields
        private static int _windowWidth;
        private static int _windowHeight;
        private static int _leftPadding;
        private static int _topPadding;
        private static char[,] _scene;
        private static int _score = 0;
        private static int _carPosition;
        private static int _carVelocity;
        private static bool _gameRunning;
        private static bool _keepPlaying = true;
        private static bool _consoleSizeError = false;
        private static int _previousRoadUpdate = 0;
        private static Random _rng = new Random();
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the driving game loop.
        /// </summary>
        public static void DriveGame()
        {
            _keepPlaying = true;
            Console.CursorVisible = false;
            try
            {
                InitializeGame();
                ShowLaunchScreen();
                while (_keepPlaying)
                {
                    Program.StopPreviousAudio();
                    Program.PlayAudio(Resources.LoadingAudio());
                    InitializeScene();
                    while (_gameRunning)
                    {
                        if (!IsConsoleValidSize())
                        {
                            _consoleSizeError = true;
                            _keepPlaying = false;
                            break;
                        }
                        HandleInput();
                        Update();
                        Render();
                        if (_gameRunning)
                            Thread.Sleep(RENDER_DELAY_MS);
                    }
                    if (_keepPlaying)
                        ShowGameOverScreen();
                }
                DisplayExitMessage();
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }
        #endregion

        #region Private Methods - Initialization

        /// <summary>
        /// Initializes the game environment and console settings.
        /// </summary>
        private static void InitializeGame()
        {
            _windowWidth = Console.WindowWidth;
            _windowHeight = Console.WindowHeight;
            // Calculate centering padding
            _leftPadding = Math.Max(0, (_windowWidth - GAME_WIDTH) / 2);
            _topPadding = Math.Max(0, (_windowHeight - GAME_HEIGHT) / 2);
            // .NET Framework does not support OperatingSystem.IsWindows()
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (_windowWidth < GAME_WIDTH)
                    _windowWidth = Console.WindowWidth = GAME_WIDTH + 1;
                if (_windowHeight < GAME_HEIGHT)
                    _windowHeight = Console.WindowHeight = GAME_HEIGHT + 1;
            }
        }

        /// <summary>
        /// Initializes a new game scene with the road and starting position.
        /// </summary>
        private static void InitializeScene()
        {
            _gameRunning = true;
            _carPosition = GAME_WIDTH / 2;
            _carVelocity = 0;
            _score = 0;
            int leftEdge = (GAME_WIDTH - ROAD_WIDTH) / 2;
            int rightEdge = leftEdge + ROAD_WIDTH + 1;
            _scene = new char[GAME_HEIGHT, GAME_WIDTH];
            for (int i = 0; i < GAME_HEIGHT; i++)
            {
                for (int j = 0; j < GAME_WIDTH; j++)
                {
                    _scene[i, j] = (j < leftEdge || j > rightEdge) ? GRASS_CHAR : ROAD_CHAR;
                }
            }
        }
        #endregion

        #region Private Methods - Rendering

        /// <summary>
        /// Displays the launch screen with game instructions.
        /// </summary>
        private static void ShowLaunchScreen()
        {
            Console.Clear();
            // Calculate center position
            int centerX = (_windowWidth - 30) / 2; // Approximate center for ~30 char text
            int centerY = (_windowHeight - 5) / 2;
            string[] lines = new[]
            {
                "This is a driving game.",
                "",
                "Stay on the road!",
                "",
                "Use A, D or <-, -> to control your vehicle.",
                "",
                "Press [enter] to start..."
            };
            // Display each line centered
            for (int i = 0; i < lines.Length; i++)
            {
                int lineX = Math.Max(0, (Console.WindowWidth - lines[i].Length) / 2);
                Console.SetCursorPosition(lineX, centerY + i);
                Console.Write(lines[i]);
            }
            WaitForKeypress(new[] { ConsoleKey.Enter, ConsoleKey.Escape });
        }

        /// <summary>
        /// Renders the scoreboard in the bottom right corner.
        /// </summary>
        private static void RenderScoreboard()
        {
            string scoreText = "Score: " + _score;
            int scoreY = Math.Min(_topPadding + GAME_HEIGHT, Console.WindowHeight - 1);
            int scoreX = Math.Max(0, Console.WindowWidth - scoreText.Length - 2);
            Console.SetCursorPosition(scoreX, scoreY);
            Console.Write(scoreText);
        }

        /// <summary>
        /// Renders the current game scene to the console.
        /// </summary>
        private static void Render()
        {
            StringBuilder sb = new StringBuilder((_leftPadding + GAME_WIDTH) * GAME_HEIGHT);
            for (int i = GAME_HEIGHT - 1; i >= 0; i--)
            {
                sb.Append(new string(' ', _leftPadding)); // Add left padding
                for (int j = 0; j < GAME_WIDTH; j++)
                {
                    sb.Append(GetCharAtPosition(i, j));
                }
                if (i > 0)
                    sb.AppendLine();
            }
            Console.SetCursorPosition(0, _topPadding);
            Console.Write(sb.ToString());
            // Render scoreboard after game scene
            RenderScoreboard();
        }

        /// <summary>
        /// Gets the character to render at a specific scene position.
        /// </summary>
        private static char GetCharAtPosition(int row, int col)
        {
            if (row == 1 && col == _carPosition)
            {
                return !_gameRunning ? CAR_STOPPED :
                    _carVelocity < 0 ? CAR_LEFT :
                    _carVelocity > 0 ? CAR_RIGHT :
                    CAR_STRAIGHT;
            }
            return _scene[row, col];
        }

        /// <summary>
        /// Displays the game over screen and prompts for replay.
        /// </summary>
        private static void ShowGameOverScreen()
        {
            Console.Clear();
            string[] lines = new[]
            {
                "Game Over",
                "",
                "Score: " + _score,
                "",
                "Press [Y] or [Enter] to Play Again",
                "Press [N] or [Escape] to Exit"
            };
            int centerY = Math.Max(0, (_windowHeight - lines.Length) / 2);
            for (int i = 0; i < lines.Length; i++)
            {
                int centerX = Math.Max(0, (_windowWidth - lines[i].Length) / 2);
                Console.SetCursorPosition(centerX, centerY + i);
                Console.Write(lines[i]);
            }
            ConsoleKey key = WaitForKeypress(new[] { ConsoleKey.Y, ConsoleKey.N, ConsoleKey.Enter, ConsoleKey.Escape });
            _keepPlaying = (key == ConsoleKey.Y || key == ConsoleKey.Enter);
        }

        /// <summary>
        /// Displays the exit message based on the reason for closing.
        /// </summary>
        private static void DisplayExitMessage()
        {
            Console.Clear();
            if (_consoleSizeError)
            {
                Console.WriteLine("Console/Terminal window is too small.");
                Console.WriteLine("Minimum size is {0} width x {1} height.", GAME_WIDTH, GAME_HEIGHT);
                Console.WriteLine("Increase the size of the console window.");
            }
            Program.StopPreviousAudio();
            Console.SetCursorPosition((Console.WindowWidth / 2) - 15, Console.WindowHeight / 2);
            Console.WriteLine("Drive was closed.");
            Menu.ExitOption((Console.WindowWidth / 2) - 15, Console.CursorTop);
        }
        #endregion

        #region Private Methods - Input & Logic

        /// <summary>
        /// Handles player input for car movement and game control.
        /// </summary>
        private static void HandleInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        _carVelocity = -1;
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        _carVelocity = 1;
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        _carVelocity = 0;
                        break;
                    case ConsoleKey.Escape:
                        _gameRunning = false;
                        _keepPlaying = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Updates game state: moves scene, shifts road, moves car, and checks collisions.
        /// </summary>
        private static void Update()
        {
            MoveSceneUp();
            UpdateRoadShift();
            MoveCarAndCheckCollision();
            _score++;
        }

        /// <summary>
        /// Moves all scene content up by one row.
        /// </summary>
        private static void MoveSceneUp()
        {
            for (int i = 0; i < GAME_HEIGHT - 1; i++)
            {
                for (int j = 0; j < GAME_WIDTH; j++)
                {
                    _scene[i, j] = _scene[i + 1, j];
                }
            }
        }

        /// <summary>
        /// Updates the road position at the bottom of the scene.
        /// </summary>
        private static void UpdateRoadShift()
        {
            // Determine road shift direction
            int roadUpdate = (_rng.Next(ROAD_SHIFT_PROBABILITY) < ROAD_SHIFT_THRESHOLD)
                ? _previousRoadUpdate
                : _rng.Next(3) - 1;
            // Prevent road from shifting out of bounds
            if (roadUpdate == -1 && _scene[GAME_HEIGHT - 1, 0] == ROAD_CHAR)
                roadUpdate = 1;
            if (roadUpdate == 1 && _scene[GAME_HEIGHT - 1, GAME_WIDTH - 1] == ROAD_CHAR)
                roadUpdate = -1;
            // Shift the road left or right
            ShiftRoadRow(roadUpdate);
            _previousRoadUpdate = roadUpdate;
        }

        /// <summary>
        /// Shifts the bottom road row left or right.
        /// </summary>
        private static void ShiftRoadRow(int direction)
        {
            if (direction == -1)
            {
                for (int i = 0; i < GAME_WIDTH - 1; i++)
                    _scene[GAME_HEIGHT - 1, i] = _scene[GAME_HEIGHT - 1, i + 1];
                _scene[GAME_HEIGHT - 1, GAME_WIDTH - 1] = GRASS_CHAR;
            }
            else if (direction == 1)
            {
                for (int i = GAME_WIDTH - 1; i > 0; i--)
                    _scene[GAME_HEIGHT - 1, i] = _scene[GAME_HEIGHT - 1, i - 1];
                _scene[GAME_HEIGHT - 1, 0] = GRASS_CHAR;
            }
        }

        /// <summary>
        /// Moves the car based on velocity and checks for collision.
        /// </summary>
        private static void MoveCarAndCheckCollision()
        {
            _carPosition += _carVelocity;
            // Check bounds and collision with grass
            if (_carPosition < 0 || _carPosition >= GAME_WIDTH || _scene[1, _carPosition] != ROAD_CHAR)
            {
                _gameRunning = false;
            }
        }

        /// <summary>
        /// Waits for the player to press one of the specified keys.
        /// </summary>
        private static ConsoleKey WaitForKeypress(ConsoleKey[] validKeys)
        {
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (validKeys.Contains(key))
                    return key;
            }
        }

        /// <summary>
        /// Checks if the console window is large enough for the game.
        /// </summary>
        private static bool IsConsoleValidSize()
        {
            return Console.WindowHeight >= GAME_HEIGHT && Console.WindowWidth >= GAME_WIDTH;
        }
        #endregion
    }
}
