namespace TicTacToe;

/// <summary>
/// A single move: the number being played, and the row and column to play it in.
/// The number is decided by the game (moves are numbered in order), not chosen
/// by the player.
/// </summary>
public readonly record struct Move(int Row, int Column, int Number);

/// <summary>
/// A participant in a game of Numerical Tic Tac Toe.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// The player's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Chooses the next move (a cell) for this player given the current board.
    /// The returned cell is not guaranteed to be legal; the caller (the game
    /// loop) is responsible for validating it against the board.
    /// </summary>
    /// <param name="board">The board as it currently stands.</param>
    Move GetMove(Board board);
}
