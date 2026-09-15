namespace TicTacToe;

/// <summary>
/// Coordinates a game of Numerical Tic Tac Toe between two players on a board.
/// </summary>
public class Game
{
    /// <summary>
    /// The two players in the game. Player one owns the odd numbers and moves
    /// first; player two owns the evens.
    /// </summary>
    public IPlayer PlayerOne { get; }
    public IPlayer PlayerTwo { get; }

    /// <summary>
    /// The board the game is played on.
    /// </summary>
    public Board Board { get; }

    /// <summary>
    /// Creates a new game between two players on a board of the given size.
    /// </summary>
    /// <param name="playerOne">The first player.</param>
    /// <param name="playerTwo">The second player.</param>
    /// <param name="boardSize">The number of cells along one side of the board.</param>
    public Game(IPlayer playerOne, IPlayer playerTwo, int boardSize)
    {
        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
        Board = new Board(boardSize);
    }
}
