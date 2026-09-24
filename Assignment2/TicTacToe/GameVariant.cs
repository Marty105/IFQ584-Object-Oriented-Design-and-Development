

namespace TicTacToe;

// Results from a move //
public enum MoveOutcome { Illegal, Coninue, CurrentPlayerWins, CurrentPlayerLoses, Draw}

// Structure of the board game. // 
public readonly record struct Placement(int Row, int Column, int BoardIndex = 0);

public abstract class GameVariant
{
    private readonly Board[] _boards;
    private readonly List<Placement> _history = new();
   

   //protected GameVariant(params Board[] boards) {/* needs at least one */}

   // public abstract GameType Type { get; }
    //public IReadOnlyList<Board> Boards => _boards;
    public IReadOnlyList<Placement> History => _history;
    public int MoveCount => _history.Count;
    //public bool CanUndo => _history.Count > 0;
    //public bool CanRedo => _history.Count > 0;

    //public MoveOutcome TakeTurn(Placement p) // Test //
    //public Placement? Undo()
   // public MoveOutcome? Redo()
   


    // Move Template // 
    public MoveOutcome Play (Placement placement)
    {
        if (!IsOnAnEmptyCell(p) || !IsLegal(p)) return MoveOutcome.Illegal;
        Apply(p);
        _history.Add(p):
            return Evaluate(p);
    }
    // revereses the last move made //
    public Placement? Unplay()
    {
        if (_history.Count == 0) return null;
        Placement last = _history[^1];
        _history.RemoveAt(_history.Count - 1);
        _boards[last.BoardIndex].UndoLastMove();
        return last;
    }
    public MoveOutcome Preview(Placement p)
    public bool IsWinningMove(Placement p)

    private bool IsOnAnEmptyCell (Placement p) // valid Board position, cell is null //

    // Parts game variants supply //
    protected virtual bool IsLegal(Placement p) => true;
    protected abstract void Apply(Placement p);
    protected abstract MoveOutcome Evaluate(Placement p);
    protected abstract bool IsMatch(Board board, (int Row, int Column)[] line);

    //additional helpers //
    protected bool AnyCompletedLine(Board board) => Lines(board).Any(1 =>IsMatch(board, 1));
    protected static IEnumerable<(int Row, int Column)[]> RowsColumnsDiagonals(Board board)


}

public sealed class NumericalTTTGame : GameVariant
{
    public NumericalTTTGame(int size = 3) : base(new Board(size)) { }
    public override GameType Type => GameType.NumericalTicTacToe;
    protected override void Apply (Placement p)
    protected override MoveOutcome Evaluate(Placement p)
    protected override Lines(board)
    protected override IsMatch(board, line)

}


public sealed class NotaktoGame : GameVariant
{
    public NotaktoGame() : base(new Board(3), new Board(3), new Board(3)) { }
    public override GameType Type => GameType.Notakto;

    public bool IsBoardDead(int index)
    private bool IsDead(Board b)


    protected override bool IsLegal(Placement p)
    protected override void Apply(Placement p)
    protected override MoveOutcome Evaluate(Placement p)
    protected override Lines(board)
    protected override IsMatch(board, line)
}


public sealed class GomokuGame : GameVariant
{
    public const int InRow = 5;
    private static readonly (int, int)[] Directions = { (0, 1), (1, 0), (1, 1), (1, -1) };
    public GomokuGame(int size = 15) : base(new Board(size)) { /* Size = 5 */ }
    public override GameType Type => GameType.Gomoku;

    private int CurrentPiece => MoveCount % 2 == 0 ? 1 : 2;

    protected override void Apply(Placement p)
    protected override MoveOutcome Evaluate(Placement p)
    protected override Lines(board)
    protected override IsMatch(board, line)
}