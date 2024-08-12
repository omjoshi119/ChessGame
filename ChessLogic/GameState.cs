using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class GameState // UI WILL INTERACT WITH THIS. 
    {
        public Board Board {  get; } // IT WILL STORE THE CURRENT BOARD CONFIG
        public Player CurrentPlayer { get; private set; } // WHOSE TURN IT IS

        public Result Result { get; private set; } = null;

        private int noCaptureOrPawnMoves = 0; // FOR 50 MOVE RULE

        private string stateString;

        private readonly Dictionary<String, int> stateHistory = new Dictionary<String, int>(); // TRACKS HOW MANY TIMES A GIVEN STATE HAS OCCURED
        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;

            stateString = new StateString(CurrentPlayer, board).ToString();
            stateHistory[stateString] = 1;
        }

        public IEnumerable<Move> LegalMoves(Position pos) // returns all moves a piece at that position can make
        {
            if(Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer) return Enumerable.Empty<Move>();
            Piece piece = Board[pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Board);
            return moveCandidates.Where(move => move.IsLegal(Board)); //
            
        }

        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPosition(CurrentPlayer, null); // TO MAKE SURE THAT ENPASSANT ONLY HAPPENS ON THE IMMEDIATE NEXT MOVE AND CANNOT HAPPEN LATER
            bool captureOrPawn = move.Execute(Board);

            if (captureOrPawn) 
            { 
                noCaptureOrPawnMoves = 0; 
                stateHistory.Clear();
            }
            else noCaptureOrPawnMoves++;

            CurrentPlayer = CurrentPlayer.Opponent();
            UpdateStateString();
            CheckForGameOver();

        }

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            //COLLECT THE MOVES FOR EACH PIECE FOR A GIVEN PLAYER
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
            {
                Piece piece = Board[pos];
                return piece.GetMoves(pos, Board);
            });
            return moveCandidates.Where(move => move.IsLegal(Board)); // FILTER OUT THE LEGAL ONES FROM ALL POSSIBLE MOVES
        }

        private void CheckForGameOver() // WILL BE CALLED AFTER THE END OF EVERY TURN AFTER THE TURN HAS BEEN SWITCHED
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any()) // IF NO LEGAL MOVES FOR A PLAYER THEN WE MUST CHECK IF ITS CHECKMATE OR STALEMATE
            {
                if (Board.IsInCheck(CurrentPlayer)) Result = Result.Win(CurrentPlayer.Opponent()); // IF IN CHECK THEN CURR PLAYER IS CHECKMATED
                else Result = Result.Draw(EndReason.Stalemate); // ELSE IT IS STALEMATE
            }
            else if (Board.InsufficientMaterial())
            {
                Result = Result.Draw(EndReason.InsufficientMaterial);
            }
            else if (FiftyMoveRule()) Result = Result.Draw(EndReason.FiftyMoveRule);
            else if (ThreefoldRepetition()) Result = Result.Draw(EndReason.ThreefoldRepetition);
        }

        public bool IsGameOver()
        {
            return Result != null;
        }

        private bool FiftyMoveRule()
        {
            int fullmoves = noCaptureOrPawnMoves/2;
            return fullmoves == 50;
        }

        private void UpdateStateString()
        {
            stateString = new StateString(CurrentPlayer, Board).ToString();

            if (!stateHistory.ContainsKey(stateString)) stateHistory[stateString] = 1;
            else stateHistory[stateString]++;
        }

        private bool ThreefoldRepetition()
        {
            return stateHistory[stateString] == 3;
        }


    }
}
