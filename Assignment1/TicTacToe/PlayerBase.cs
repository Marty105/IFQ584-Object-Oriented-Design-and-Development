namespace TicTacToe;

/// <summary>
/// Base class for the game's players. Holds the state and behaviour common to
/// every player — a name, and the pool of numbers they own and spend as they
/// play — so concrete players only need to supply their move-selection strategy
/// in <see cref="GetMoveAsync"/>.
///
/// This class does not itself declare <see cref="IPlayer"/>; each concrete
/// player implements that interface, using the members provided here to satisfy
/// it.
/// </summary>
public abstract class PlayerBase
{
    /// <summary>
    /// The numbers this player has not played yet, kept in ascending order.
    /// </summary>
    private readonly List<int> _remaining;

    /// <summary>
    /// The player's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Which half of the numbers this player owns (the odds or the evens).
    /// </summary>
    public Parity Numbers { get; }

    /// <summary>
    /// The numbers this player still has left to play, in ascending order.
    /// </summary>
    public IReadOnlyList<int> Remaining => _remaining;

    /// <summary>
    /// Initialises the shared player state. Called by subclass constructors.
    /// </summary>
    /// <param name="name">A display name for the player.</param>
    /// <param name="numbers">Which half of the numbers this player owns.</param>
    /// <param name="boardSize">
    /// The number of cells along one side of the board, which fixes the range of
    /// numbers in play (1..boardSize^2).
    /// </param>
    protected PlayerBase(string name, Parity numbers, int boardSize)
    {
        Name = name;
        Numbers = numbers;
        _remaining = NumbersFor(numbers, boardSize).ToList();
    }

    /// <summary>
    /// The full set of numbers belonging to one side on a board of the given
    /// size: every number from 1 to boardSize^2 with a matching parity. On a 3x3
    /// board that is 1, 3, 5, 7, 9 for the odds and 2, 4, 6, 8 for the evens.
    /// </summary>
    public static IEnumerable<int> NumbersFor(Parity parity, int boardSize)
    {
        int first = parity == Parity.Odd ? 1 : 2;

        for (int number = first; number <= boardSize * boardSize; number += 2)
        {
            yield return number;
        }
    }

    /// <summary>
    /// True if this player still holds the given number.
    /// </summary>
    public bool HasNumber(int number) => _remaining.Contains(number);

    /// <summary>
    /// Spends one of this player's numbers, so it cannot be played again.
    /// Returns false if the player did not hold it.
    /// </summary>
    public bool UseNumber(int number) => _remaining.Remove(number);

    /// <summary>
    /// Chooses this player's next move given the current board. Each concrete
    /// player (human, computer, ...) provides its own implementation. The
    /// returned move is validated by the game loop, not here.
    /// </summary>
    public abstract Move GetMove(Board board);

    public override string ToString() => Name;
}
