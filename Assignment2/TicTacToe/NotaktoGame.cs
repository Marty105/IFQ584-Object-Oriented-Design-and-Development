namespace TicTacToe;

/// <summary>
/// Notakto: both players play X on three 3x3 boards. A board with three in a
/// row is dead and can't be played on; whoever kills the last board loses.
/// </summary>
public sealed class NotaktoGame : Game, IGame
{
    public NotaktoGame(IPlayer playerOne, IPlayer playerTwo)
        : base(playerOne, playerTwo, new Board(3), new Board(3), new Board(3))
    {
    }

    public override GameType Type => GameType.Notakto;

    public bool IsBoardDead(int index)
    {
        throw new NotImplementedException();
    }

    public override bool IsLegal(Placement p)
    {
        throw new NotImplementedException();
    }

    public override void Apply(Placement p)
    {
        throw new NotImplementedException();
    }

    public override MoveOutcome Evaluate(Placement p)
    {
        throw new NotImplementedException();
    }
}
