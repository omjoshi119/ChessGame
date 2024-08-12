using System;
using System.Text;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessLogic;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8]; // HIGHLIGHTS POSSIBLE POSITIONS FOR THE PIECES
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>(); // WHEN THE PLAYER CLICKS ON THE PIECE, THAT POSITION GETS STORED IN THE SELECTEDPOS. THEN WE ASK THE GAMESTATE WHICH MOVES THE SELECTED PIECE CAN MAKE. THESE WILL BE STORED IN moveCache using "TO" POSITION AS THE KEY. WE SHOW THEM ON SCREEN USING HIGHLIGHT. WHEN PLAYER CLICKS ON ONE OF THE HIGHLIGHTS, WE GET THE CORRESPONDING MOVE FROM MOVECACHE AND EXECUTE IT.


        private GameState gameState; // a gamestate instance that we initialise in the constructor
        private Position selectedPos = null;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            gameState = new GameState(Player.White, Board.Initial()); // white player starts the game and we also pass the initial board
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
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
                    highlights[r, c] = highlight; // HIGHLIGHTS ARE TRANSPARENT BY DEFAULT SO THEY WONT BE VISIBLE UNLESS WE NEED THEM
                    HighlightGrid.Children.Add(highlight);
                }
            }
        }

        private void DrawBoard(Board board) // sets the source of all image controls so that they match the pieces on board
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = board[r, c];
                    pieceImages[r, c].Source = Images.GetImage(piece);
                }
            }
        }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMenuOnScreen()) return; // IF PLAYER CLICKS ON THE BOARD WHEN THERE IS MENU ON THE SCREEN, DO NOTHING

            Point point = e.GetPosition(BoardGrid); // THE COORDINATES OF THIS POINT IS GIVEN IN PIXELS BUT WE WANT TO KNOW THE SQUARE. HENCE WE CREATE A HELPER "ToSquarePosition"
            Position pos = ToSquarePosition(point);
            if (selectedPos == null) OnFromPositionSelected(pos); // IF THERE IS NO PIECE ON THAT POSITION
            else OnToPositionSelected(pos);
        }

        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMoves(pos); // WILL BE EMPTY IF PLAYER CLICKED ON EMPTY SQUARE, OPPONENT PIECE OR ON A PIECE THAT CANNOT MOVE
            if (moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighlights();
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighlights();

            if (moveCache.TryGetValue(pos, out Move move))
            {
                if(move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPos, move.ToPos);// PAWN PROMOTION, CAN'T EXECUTE IMMEDIATELY, WE NEED INTERMEDIATE STEPS, HENCE HANDLEPROMOTION METHOD TO FIRST TAKE PAWN FORWARD, SHOW PROMOTION MENU, SELECT PIECE, REMOVE MENU AND SHOW THE PROMOTED PIECE
                }
                else
                {
                    HandleMove(move); // TRY TO GET THE MOVE WITH "TO" POSITION EQUAL TO CLICKED POSITION
                }
                
            }
        }

        private void HandlePromotion(Position from, Position to)
        {
            pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn); // AS AN INTERMEDIATE STEP SHOW THE PAWN AT THE "TO" POSITION 
            pieceImages[from.Row, from.Column].Source = null; // AND REMOVE IT FROM THE "FROM" POSITION

            PromotionMenu promMenu = new PromotionMenu(gameState.CurrentPlayer);
            MenuContainer.Content = promMenu;

            promMenu.PieceSelected += type => // AFTER PIECE IS SELECTED
            {
                MenuContainer.Content = null; // REMOVE THE MENU
                Move promMove = new PawnPromotion(from, to, type); // EXECUTE THE PROMOTION MOVE
                HandleMove(promMove);
            };

        }

        private void HandleMove(Move moves)
        {
            gameState.MakeMove(moves);
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer); // TO CHANGE THE COLOR OF THE CURSOR AFTER MOVE IS MADE
            if(gameState.IsGameOver()) ShowGameOver(); // SHOW GAME OVER AFTER SUCH A MOVE
        }

        private Position ToSquarePosition(Point point) // RETURNS THE ROW AND COL FOR A GIVEN POINT
        {
            double squareSize = BoardGrid.ActualWidth / 8; // WE MUST FIND HOW LARGE A SQUARE IS AT THE CURRENT WINDOW SIZE
            int row = (int)(point.Y / squareSize);
            int col = (int)(point.X / squareSize);
            return new Position(row, col);
        }

        private void CacheMoves(IEnumerable<Move> moves) // TAKES THE COLLECTION OF LEGAL MOVES FOR THE PIECE AND STORES THEM IN CACHE
        {
            moveCache.Clear(); // EMPTY IT FIRST
            foreach (Move move in moves)
            {
                moveCache[move.ToPos] = move; // THE "TO" WILL BE THE KEY AND THE "MOVE" WILL BE THE VALUE
            }
        }

        private void ShowHighlights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125); // SEMI GREEN COLOR

            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHighlights()
        {
            foreach(Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
            }
        }

        private void SetCursor(Player player) // SHOULD BE CALLED IN THE CONSTRUCTOR WHEN THE GAME STARTS
        {
            if (player == Player.White) Cursor = ChessCursors.WhiteCursor;
            else Cursor = ChessCursors.BlackCursor;
        }

        private bool IsMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }

        private void ShowGameOver()
        {
            GameOverMenu gameOverMenu = new GameOverMenu(gameState);
            MenuContainer.Content = gameOverMenu; // THIS MAKES THE MENU SHOW UP ON SCREEN

            gameOverMenu.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    MenuContainer.Content = null;
                    RestartGame();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            };
        }

        private void RestartGame()
        {
            selectedPos = null; // SINCE WE CAN ALSO RESTART WHEN A PIECE IS SELECTED, WE NEED TO UNSELECT THAT WHEN RESTARTING
            HideHighlights();
            moveCache.Clear();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }

        private void Window_Keydown(object sender, KeyEventArgs e)
        {
            if (!IsMenuOnScreen() && e.Key == Key.Escape) ShowPauseMenu();
        }

        private void ShowPauseMenu()
        {
            PauseMenu pauseMenu = new PauseMenu();
            MenuContainer.Content = pauseMenu; // SHOW THE PAUSE MENU

            pauseMenu.OptionSelected += option =>
            {
                MenuContainer.Content = null; // MAKES THE MENU DISAPPEAR -- FOR BOTH CASES , RESUME AND RESTART

                if (option == Option.Restart) RestartGame(); // IF CLICKED RESTART THEN RESTART THE GAME
            };
        }
    }
}