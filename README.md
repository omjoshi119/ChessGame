# ChessGame

## Overview

Experience the timeless game of chess with this 2-player game built in C#. Designed for local play, this game allows two players to compete against each other on a classic 8x8 chessboard. This game has all the standard chess rules enforced, including special moves. The user-friendly interface ensures a smooth gameplay experience, making it easy for anyone to enjoy a match.

## Features

- **Local Multiplayer**: Play against a friend on the same computer.
- **Intuitive Interface**: Easy-to-use controls for selecting and moving pieces. 
- **Real-Time Gameplay**: Players take turns in real-time with visual updates.
- **Standard Chess Rules**: Supports all the standard moves and rules of chess, including castling, en passant, and pawn promotion.
- **Move Validation**: Ensures that only legal moves can be made by highlighting them when a piece is selected.

## Getting Started

### Prerequisites

- **.NET Framework**: Ensure that you have the .NET Framework installed on your machine.
- **C# Compiler**: If you want to compile the game from source, you'll need a C# compiler like Visual Studio.

### Installation

- The project is divided into two folders: ChessUI and ChessLogic.
- The UI files can be found in the ChessUI folder.
- The gameplay-related files including the class files for the game pieces, possible moves etc. can be located in the ChessLogic folder.
- The game application (.exe) can be accessed through the Game folder.

### How to Play

1. **Launch the Game**: Run the application.
2. **Start a New Game**: The game will automatically load a new chessboard.
3. **Player Turns**: Player 1 (White) moves first. Click on a piece to select it and then click on the desired destination square to move it.
4. **Win Conditions**: The game follows the standard rules of chess for check, checkmate, and stalemate. The game ends when one player is checkmated or a draw is reached.

## Game Controls

- **Select Piece**: Click on the piece you want to move.
- **Move Piece**: Click on the target square to move the selected piece.

## Rules

The game follows the official rules of chess and these special moves/features:
- Pawns can promote to any piece when they reach the opposite end of the board.
- En passant captures are supported.
- Castling (Kingside and Queenside) is allowed if all conditions are met.
- A draw can occur by stalemate, threefold repetition, the fifty-move rule or due to insufficient material.

## Troubleshooting

If you encounter any issues:
- Ensure you have the latest version of the .NET Framework installed.
- Make sure your system meets the basic requirements for running C# applications.
- Check for any compiler errors if building from source.

## Future Enhancements

Planned features and improvements include:
- **AI Opponent**: Adding a computer opponent for solo play using Stockfish.
- **Online Multiplayer**: Enabling gameplay over a network.
- **Improved Graphics**: Enhancing the visual presentation of the board and pieces.

## Contributing

If you have any suggestions or would like to contribute to the project, feel free to fork the repository and submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments
- OttoBotCode (YouTube)
- Inspired by the classic game of chess.
- Developed with the support of C# and .NET Framework communities.

---



