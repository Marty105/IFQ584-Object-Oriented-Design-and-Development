namespace TicTacToe;

// Experimental: placement kinds for tile-based vs line-based games. Not used yet. //
public interface IPlacement
{
    
}

public class Tile : IPlacement
{
    
}

public class Line : IPlacement
{
    
}

public interface ITileGame : IGame
{
    public bool Move(int x, int y);
}

public interface ILineGame
{
    
}
