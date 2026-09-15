namespace TicTacToe;

/// <summary>
/// Base class for the game's players. Holds the state and behaviour common to
/// every player — just a name — so concrete players only need to supply their
/// move-selection strategy in <see cref="GetMove"/>.
///
/// Players no longer own a set of numbers: numbers are played in order and
/// shared between the players (see <see cref="Game.NextNumber"/>), so a move is
/// only a choice of cell.
///
/// This class does not itself declare <see cref="IPlayer"/>; each concrete
/// player implements that interface, using the members provided here to satisfy
/// it.
/// </summary>
public abstract class PlayerBase
{
    /// <summary>
    /// The player's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initialises the shared player state. Called by subclass constructors.
    /// </summary>
    /// <param name="name">A display name for the player.</param>
    protected PlayerBase(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Chooses this player's next move (a cell) given the current board. Each
    /// concrete player (human, computer, ...) provides its own implementation.
    /// The returned move is validated by the game loop, not here.
    /// </summary>
    public abstract Move GetMove(Board board);

    public override string ToString() => Name;
}
