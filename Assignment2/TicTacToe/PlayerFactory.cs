namespace TicTacToe;

/// <summary>The kinds of player the game can create.</summary>
public enum PlayerKind
{
    /// <summary>A human who types moves at the console.</summary>
    Human,

    /// <summary>The computer opponent.</summary>
    Computer
}

/// <summary>
/// Creates <see cref="IPlayer"/> instances (the Factory pattern). The game loop
/// asks for a player by <see cref="PlayerKind"/> and name and gets back an
/// <see cref="IPlayer"/>, without naming the concrete <see cref="Player"/> or
/// <see cref="Computer"/> types itself. Concentrating construction here means a
/// new kind of player (say a networked or smarter AI opponent) is added in one
/// place, and callers are decoupled from how players are built.
/// </summary>
public static class PlayerFactory
{
    /// <summary>
    /// Creates a player of the given kind with the given name.
    /// </summary>
    /// <param name="kind">Which kind of player to create.</param>
    /// <param name="name">The player's display name.</param>
    /// <returns>The new player, as an <see cref="IPlayer"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If <paramref name="kind"/> is not a known <see cref="PlayerKind"/>.
    /// </exception>
    public static IPlayer Create(PlayerKind kind, string name) => kind switch
    {
        PlayerKind.Human => new Player(name),
        PlayerKind.Computer => new Computer(name),
        _ => throw new ArgumentOutOfRangeException(
            nameof(kind), kind, "Unknown player kind.")
    };
}
