using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ChessLogic;
using ChessLogic.Enum;

namespace ChessUI
{
    /// <summary>
    /// This client is "dumb": it holds no GameState and enforces no rules
    /// It renders whatever board the server sends (<see cref="ApplyState"/>)
    /// forwards raw from/to clicks (<see cref="ChessClient.SendMove"/>).
    /// The server validates and echoes the authoritative state back. Move
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];

        private readonly MenuController menus;
        private readonly ChessClient client = new ChessClient();
        private ChessServer server;

        private Player myColor = Player.None;
        private Player currentTurn = Player.White;
        private bool gameOver;
        private Position selectedPos = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();

            menus = new MenuController(MenuContainer, () => RestartGame());

            client.ConnectionEstablished += payload => Dispatcher.Invoke(() =>
            {
                myColor = System.Enum.Parse<Player>(payload);
                Title = $"Chess — you are {myColor}";
            });

            client.ConnectionFailed += msg => Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"Connection failed: {msg}", "Chess");
                ShowConnectMenu();
            });

            client.MoveRejected += msg => Dispatcher.Invoke(() =>
            {
                selectedPos = null;
                Title = $"Chess — you are {myColor} — {msg}";
            });
            client.StateUpdated += dto => Dispatcher.Invoke(() => ApplyState(dto));

            ShowConnectMenu();
        }

        private void ShowConnectMenu()
        {
            NetworkConfig config = NetworkConfig.Load();

            menus.ShowUserPrompt(
                client.UserName, client.LocalIp, config.Port, config.LocalIp, onHost: () =>
                {
                    if (server == null)
                    {
                        try
                        {
                            server = new ChessServer(config.Port);
                            server.Start();
                        }
                        catch (System.Net.Sockets.SocketException ex)
                        {
                            server = null;
                            MessageBox.Show($"Could not host on port {config.Port}: {ex.Message}", "Chess");
                            ShowConnectMenu();
                            return;
                        }
                    }
                    client.Connect(config.LocalIp, config.Port);
                }, onJoin: ip => client.Connect(ip, config.Port));
        }

        private void InitializeBoard()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Image image = new Image();
                    pieceImages[r, c] = image;
                    PieceGrid.Children.Add(image);

                    Rectangle highlight = new Rectangle();
                    highlights[r, c] = highlight;
                    HighlightGrid.Children.Add(highlight);
                }
            }
        }

        private void ApplyState(GameStateDto state)
        {
            selectedPos = null;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    // e.g. "White_Pawn", or null
                    string cell = state.Board[r][c]; 
                    if (cell == null)
                    {
                        pieceImages[r, c].Source = null;
                        continue;
                    }

                    string[] parts = cell.Split('_');
                    Player color = System.Enum.Parse<Player>(parts[0]);
                    PieceType type = System.Enum.Parse<PieceType>(parts[1]);
                    pieceImages[r, c].Source = Images.GetImage(color, type);
                }
            }

            currentTurn = System.Enum.Parse<Player>(state.CurrentPlayer);
            SetCursor(currentTurn);

            gameOver = state.IsGameOver;
            if (gameOver)
            {
                string text = string.IsNullOrEmpty(state.Winner) || state.Winner == Player.None.ToString()
                    ? $"Game over — draw ({state.Reason})"
                    : $"Game over — {state.Winner} wins ({state.Reason})";
                MessageBox.Show(text, "Game Over");
            }
        }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (menus.IsOpen || gameOver) return;

            if (myColor == Player.None) return;
            if (currentTurn != myColor) return;

            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(point);

            if (selectedPos == null)
            {
                selectedPos = pos;
            }
            else
            {
                client.SendMove(selectedPos.Row, selectedPos.Column, pos.Row, pos.Column);
                selectedPos = null;
            }
        }

        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int column = (int)(point.X / squareSize);
            return new Position(row, column);
        }

        private void RestartGame()
        {
            myColor = Player.None;
            currentTurn = Player.White;
            gameOver = false;
            selectedPos = null;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    pieceImages[r, c].Source = null;
                }
            }
            ShowConnectMenu();
        }

        private void SetCursor(Player player)
        {
            Cursor = player == Player.White ? ChessCursors.WhiteCursor : ChessCursors.BlackCursor;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (menus.IsOpen) return;

            if (e.Key == Key.Escape)
            {
                menus.ShowPause();
            }
        }
    }
}