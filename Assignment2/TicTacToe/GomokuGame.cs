namespace TicTacToe;

/// <summary>
/// Gomoku: players take turns placing their own stones on a large board; the
/// first to get <see cref="InRow"/> of their stones in a line wins.
/// </summary>
public sealed class GomokuGame : Game, IGame
{
    public const int InRow = 5;

    public GomokuGame(IPlayer playerOne, IPlayer playerTwo, int size = 15)
        : base(playerOne, playerTwo, new Board(size))
    {
    }

    public override GameType Type => GameType.Gomoku;

    private int CurrentPiece => MoveCount % 2 == 0 ? 1 : 2;

    public bool IsLegal(Placement p)
    {
        throw new NotImplementedException();
    }

    public MoveOutcome PlayMove(Placement p)
    {
        throw new NotImplementedException();
    }

    private static bool IsMatch(Board board, (int Row, int Column)[] line)
    {
        throw new NotImplementedException();
    }
}
