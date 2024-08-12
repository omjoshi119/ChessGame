using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class Board // will store the actice pieces and will provide helper methods
    {
        private readonly Dictionary<Player, Position> pawnSkipPositions = new Dictionary<Player, Position> // TO STORE THE SQUARE THAT A PAWN SKIPPED WHEN MAKING DOUBLE MOVE --- USEFUL FOR EN PASSANT IMPLEMENTATION
        {
            {Player.White, null },
            {Player.Black, null }
        };
        private readonly Piece[,] pieces = new Piece[8, 8]; // rectangular board, it is private so we will access it using row,col as below:

        public Piece this[int row, int col]
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }
        public Piece this[Position pos] // we can even set using a given "Position"
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        public Position GetPawnSkipPosition(Player player)
        {
            //IF GIVEN PLAYER MOVED A PAWN TWO SQUARES IN PREV TURN, THIS RETURNS THE POSITION OF THE SKIPPED SQUARE
            return pawnSkipPositions[player];
        }

        public void SetPawnSkipPosition(Player player, Position pos)
        {
            //TAKES A PLAYER AND SKIPPED POSITION AND STORES IT IN DICTIONARY
            pawnSkipPositions[player] = pos;
        }

        public static Board Initial() // THIS METHOD SHOULD RETURN A BOARD WITH ALL THE PIECES SETUP CORRECTLY TO START A GAME
        {
            Board board = new Board();
            board.AddStartPieces();
            return board;
        }
        private void AddStartPieces()
        {
            //ADDING ALL THE BLACK PIECES FROM THE TOP LEFT
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);

            //ADDING ALL THE WHITE PIECES IN THE BOTTOM ROW
            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);

            for (int c = 0; c < 8; c++)
            {
                this[1, c] = new Pawn(Player.Black);
                this[6, c] = new Pawn(Player.White);
            }

        }

        public static bool IsInside(Position pos) // RETURNS TRUE IF THE POSITION IS INSIDE THE BOARD
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }
        public bool IsEmpty(Position pos) //RETURNS TRUE IF THERE IS NO PIECE ON THAT POSITION
        {
            return this[pos] == null;
        }

        public IEnumerable<Position> PiecePositions() // RETURNS ALL NON-EMPTY POSITIONS
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Position pos = new Position(r, c);
                    if (!IsEmpty(pos)) yield return pos;
                }
            }
        }

        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return PiecePositions().Where(pos => this[pos].Color == player); // RETURNS POSITIONS OF SAME COLOR PIECES
        }

        public bool IsInCheck(Player player) // SEE IF ANY OF THE OPPONENTS PIECE CAN CAPTURE OUR KING
        {
            return PiecePositionsFor(player.Opponent()).Any(pos =>
            {
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos, this);
            });
        }

        public Board Copy() // FILTER OUT ANY MOVES THAT LEAVE THE KING IN CHECK
        {
            Board copy = new Board();
            foreach (Position pos in PiecePositions())
            {
                copy[pos] = this[pos].Copy();
            }
            return copy;
        }

        public Counting CountPieces() // COUNTING OF ALL THE ACTIVE PIECES ON BOARD
        {
            Counting counting = new Counting();

            foreach (Position pos in PiecePositions())
            {
                Piece piece = this[pos];
                counting.Increment(piece.Color, piece.Type);
            }
            return counting;
        }

        public bool InsufficientMaterial()
        {
            Counting counting = CountPieces(); // COUNT THE CURRENT PIECES ON THE BOARD

            return IsKingKnightVsKing(counting) || IsKingVsKing(counting) || IsKingBishopVsKingBishop(counting) || IsKingBishopVsKing(counting);

        }

        private static bool IsKingVsKing(Counting counting) // TRUE IF ONLY 2 KINGS LEFT ON BOARD
        {
            return counting.TotalCount == 2;
        }

        private static bool IsKingBishopVsKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.WhitePieces(PieceType.Bishop) == 1 || counting.BlackPieces(PieceType.Bishop) == 1);
        }

        private static bool IsKingKnightVsKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.WhitePieces(PieceType.Knight) == 1 || counting.BlackPieces(PieceType.Knight) == 1);
        }

        private bool IsKingBishopVsKingBishop(Counting counting) // IF LAST TWO BISHOPS, ONE OF EACH PLAYER AND BOTH OF SAME COLOR TYPE EXIST THEN INSUFFICIENT MATERIAL
        {
            return counting.TotalCount == 4 && (FindPiece(Player.White,PieceType.Bishop).SquareColor() == FindPiece(Player.Black, PieceType.Bishop).SquareColor());
        }

        private Position FindPiece(Player color, PieceType type) // RETURNS FIRST INSTANCE OF PIECE OF A GIVEN TYPE AND COLOR
        {
            return PiecePositionsFor(color).First(pos => this[pos].Type == type);
        }

        private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)
        {
            if(IsEmpty(kingPos) || IsEmpty(rookPos)) return false;
            Piece king = this[kingPos];
            Piece rook = this[rookPos];

            return king.Type == PieceType.King && rook.Type == PieceType.Rook && !king.HasMoved && !rook.HasMoved;
        }

        public bool CastleRightKS(Player player) // WHETHER A PLAYER CAN CASTLE KS NOW OR IN THE FUTURE 
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),
                _ => false
            };
        }

        public bool CastleRightQS(Player player) // ABOVE BUT FOR QS    
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
                _ => false
            };
        }

        private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position skipPos)
        {
            foreach (Position pos in pawnPositions.Where(IsInside))
            {
                Piece piece = this[pos];
                if (piece == null || piece.Color != player || piece.Type != PieceType.Pawn) continue;

                EnPassant move = new EnPassant(pos,skipPos);
                if (move.IsLegal(this)) return true;
                
            }
            return false;
        }

        public bool CanCaptureEnPassant(Player player)
        {
            Position skipPos = GetPawnSkipPosition(player.Opponent());
            if (skipPos == null) return false;
            Position[] pawnPositions = player switch
            {
                Player.White => new Position[] { skipPos + Direction.SouthWest, skipPos + Direction.SouthEast }, // SEE IF PLAYER HAS A PAWN IN POSITION TO CAPTURE ENPASSANT
                Player.Black => new Position[] { skipPos + Direction.NorthWest, skipPos + Direction.NorthEast },
                _ => Array.Empty<Position>()
            };

            return HasPawnInPosition(player,pawnPositions, skipPos);
        }

    }
}
